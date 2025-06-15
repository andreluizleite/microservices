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
            var controller = new RuleController(mockService.Object);

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

            var controller = new RuleController(mockService.Object);

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

            var controller = new RuleController(mockService.Object);

            var result = await controller.SaveRule("save-id", dto);

            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.SaveAsync("save-id", dto), Times.Once);
        }

        [Fact]
        public async Task SaveRule_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            var mockService = new Mock<IRulePersistenceService>();
            var controller = new RuleController(mockService.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var dto = new RuleTreeDto
            {
                Name = "",
                Type = RuleTreeNodeType.Rule,
                Expression = "1 == 1"
            };

            var result = await controller.SaveRule("bad-id", dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Name", badRequest.Value?.ToString());
        }

        private class PersonContext
        {
            public int Age { get; set; }
        }

        [Fact]
        public async Task GetRule_ShouldAllow_ParsingAndEvaluating_AfterLoading()
        {
            var dto = new RuleTreeDto
            {
                Name = "AgeRule",
                Type = RuleTreeNodeType.Rule,
                Expression = "Age >= 18"
            };

            var mockService = new Mock<IRulePersistenceService>();
            mockService.Setup(s => s.LoadAsync("age-rule")).ReturnsAsync(dto);

            var controller = new RuleController(mockService.Object);
            var result = await controller.GetRule("age-rule");

            var ok = Assert.IsType<OkObjectResult>(result);
            var ruleDto = Assert.IsType<RuleTreeDto>(ok.Value);

            var ruleNode = RuleTreeParser.Parse<PersonContext>(dto);
            var compiler = new RuleCompilerService<PersonContext>();
            compiler.Compile(ruleNode);

            var input = new PersonContext { Age = 25 };
            var passed = ruleNode.Evaluate(input);

            Assert.True(passed);
        }
    }
}
