using Microsoft.AspNetCore.Mvc;
using Moq;
using RuleEngine.Api.Controllers;
using RuleEngine.Application.DTOs;
using RuleEngine.Application.Interfaces;
using RuleEngine.Domain.CrewManagement.Entities;

namespace RuleEngine.Api.Tests
{
    public class CrewRuleControllerTests
    {
        [Fact]
        public async Task Evaluate_ShouldReturnSuccess_WhenValidRuleAndInput()
        {
            // Arrange
            var dto = new RuleTreeDto
            {
                Name = "HotToFlightTimeCredit",
                Type = Application.Enums.RuleTreeNodeType.Rule,
                Expression = @"
                    Assignments.Any(ServiceTypeCode == ""Y"") && 
                    Assignments.Any(ServiceTypeCode == ""C"") ?
                    Convert.ToDecimal(
                        (Assignments.First(ServiceTypeCode == ""C"").ScheduledStart - 
                         Assignments.First(ServiceTypeCode == ""Y"").ScheduledStart).TotalHours * 0.5
                    )
                    : Convert.ToDecimal(0)"
            };

            var mockService = new Mock<IRulePersistenceService>();
            mockService.Setup(s => s.LoadAsync("credit-rule")).ReturnsAsync(dto);

            var controller = new CrewRuleController(mockService.Object);

            var request = new RuleEvaluationRequest
            {
                RuleId = "credit-rule",
                Input = new Dictionary<string, object>
                {
                    {
                        "Assignments", new List<Assignment>
                        {
                            new GroundActivity
                            {
                                ServiceTypeCode = "Y",
                                ScheduledStart = new DateTime(2025, 6, 10, 8, 0, 0)
                            },
                            new Leg
                            {
                                ServiceTypeCode = "C",
                                ScheduledStart = new DateTime(2025, 6, 10, 9, 0, 0)
                            }
                        }
                    }
                }
            };

            // Act
            var result = await controller.Evaluate(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RuleEvaluationResponse>(ok.Value);
            Assert.True(response.Success);
            Assert.Equal(0.5m, response.Result);
        }

        [Fact]
        public async Task Evaluate_ShouldReturnBadRequest_WhenRuleFailsToCompile()
        {
            // Arrange
            var dto = new RuleTreeDto
            {
                Name = "InvalidRule",
                Type = Application.Enums.RuleTreeNodeType.Rule,
                Expression = "Assignments[Invalid syntax"
            };

            var mockService = new Mock<IRulePersistenceService>();
            mockService.Setup(s => s.LoadAsync("invalid-rule")).ReturnsAsync(dto);

            var controller = new CrewRuleController(mockService.Object);

            var request = new RuleEvaluationRequest
            {
                RuleId = "invalid-rule",
                Input = new Dictionary<string, object>
                {
                    { "Assignments", new List<Assignment>() }
                }
            };

            // Act
            var result = await controller.Evaluate(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<RuleEvaluationResponse>(badRequest.Value);
           
            Assert.False(response.Success);
            Assert.NotNull(response.Error);
        }

        [Fact]
        public async Task Evaluate_ShouldReturnNotFound_WhenRuleDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<IRulePersistenceService>();
            mockService.Setup(s => s.LoadAsync("missing-rule")).ReturnsAsync((RuleTreeDto?)null);

            var controller = new CrewRuleController(mockService.Object);

            var request = new RuleEvaluationRequest
            {
                RuleId = "missing-rule",
                Input = new Dictionary<string, object>()
            };

            // Act
            var result = await controller.Evaluate(request);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Rule with ID 'missing-rule' not found.", notFound.Value);
        }
    }
}
