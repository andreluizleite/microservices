using Microsoft.AspNetCore.Mvc;
using Moq;
using RuleEngine.Api.Controllers;
using RuleEngine.Application.DTOs;
using RuleEngine.Application.Enums;
using RuleEngine.Application.Interfaces;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Enums;
using Xunit;

namespace RuleEngine.Api.Tests
{
    public class RuleControllerTests
    {
        [Fact]
        public async Task GetRule_ShouldReturnOk_WhenRuleExists()
        {
            var mockService = new Mock<IRulePersistenceService>();
            var dto = new RuleTreeDto
            {
                Name = "Root",
                Type = RuleTreeNodeType.Group,
                Operator = LogicalOperator.And,
                Children = new List<RuleTreeDto>()
            };

            mockService.Setup(s => s.LoadAsync("test-id")).ReturnsAsync(dto);
            var compiler = new RuleCompilerService<Dictionary<string, object>>();
            var controller = new RuleController(mockService.Object, compiler);

            var result = await controller.GetRule("test-id");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<RuleTreeDto>(okResult.Value);
            Assert.Equal("Root", returnedDto.Name);
        }

        [Fact]
        public async Task GetRule_ShouldReturnNotFound_WhenRuleDoesNotExist()
        {
            var mockService = new Mock<IRulePersistenceService>();
            mockService.Setup(s => s.LoadAsync("missing-id")).ReturnsAsync((RuleTreeDto?)null);

            var compiler = new RuleCompilerService<Dictionary<string, object>>();
            var controller = new RuleController(mockService.Object, compiler);

            var result = await controller.GetRule("missing-id");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SaveRule_ShouldReturnOk()
        {
            var mockService = new Mock<IRulePersistenceService>();
            var dto = new RuleTreeDto
            {
                Name = "SaveTest",
                Type = RuleTreeNodeType.Rule,
                Expression = "1 == 1"
            };

            var compiler = new RuleCompilerService<Dictionary<string, object>>();
            var controller = new RuleController(mockService.Object, compiler);

            var result = await controller.SaveRule("save-id", dto);

            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.SaveAsync("save-id", dto), Times.Once);
        }

        [Fact]
        public async Task SaveRule_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var mockService = new Mock<IRulePersistenceService>();
            var compiler = new RuleCompilerService<Dictionary<string, object>>();
            var controller = new RuleController(mockService.Object, compiler);
            controller.ModelState.AddModelError("Name", "Required");

            var dto = new RuleTreeDto
            {
                Name = "",
                Type = RuleTreeNodeType.Rule,
                Expression = "1 == 1"
            };

            var result = await controller.SaveRule("bad-id", dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<SerializableError>(badRequest.Value);
            Assert.True(errors.ContainsKey("Name"));
        }


        private class PersonContext
        {
            public int Age { get; set; }
        }

        [Fact]
        public async Task EvaluateRule_ShouldReturnSuccess_WhenRuleIsValid()
        {
            var dto = new RuleTreeDto
            {
                Name = "AdultCheck",
                Type = RuleTreeNodeType.Rule,
                Expression = "Age >= 18"
            };

            var mockService = new Mock<IRulePersistenceService>();
            mockService.Setup(s => s.LoadAsync("check-age")).ReturnsAsync(dto);

            var compiler = new RuleCompilerService<Dictionary<string, object>>();
            var controller = new RuleController(mockService.Object, compiler);

            var request = new RuleEvaluationRequest
            {
                RuleId = "check-age",
                Input = new Dictionary<string, object>
        {
            { "Age", 30 }
        }
            };

            var result = await controller.EvaluateRule(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RuleEvaluationResponse>(okResult.Value);

            Assert.True(response.Success);
            Assert.Empty(response.FailedRules);
        }

        [Fact]
        public async Task EvaluateRule_ShouldReturnFailed_WhenRuleFails()
        {
            var dto = new RuleTreeDto
            {
                Name = "UnderageCheck",
                Type = RuleTreeNodeType.Rule,
                Expression = "Age >= 18"
            };

            var mockService = new Mock<IRulePersistenceService>();
            mockService.Setup(s => s.LoadAsync("underage")).ReturnsAsync(dto);

            var compiler = new RuleCompilerService<Dictionary<string, object>>();
            var controller = new RuleController(mockService.Object, compiler);

            var request = new RuleEvaluationRequest
            {
                RuleId = "underage",
                Input = new Dictionary<string, object>
        {
            { "Age", 15 }
        }
            };

            var result = await controller.EvaluateRule(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RuleEvaluationResponse>(okResult.Value);

            Assert.False(response.Success);
            Assert.Contains("UnderageCheck", response.FailedRules);
        }

    }
}
