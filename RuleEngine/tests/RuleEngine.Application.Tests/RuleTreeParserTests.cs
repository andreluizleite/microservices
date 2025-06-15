// File: RuleTreeParserTests.cs
using RuleEngine.Application.DTOs;
using RuleEngine.Application.Enums;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Enums;
using RuleEngine.Domain.Core.Rules;
using Xunit;

namespace RuleEngine.Application.Tests
{
    public class RuleTreeParserTests
    {
        public class DummyContext
        {
            public int Score { get; set; }
        }

        [Fact]
        public void Should_Parse_And_Evaluate_RuleGroup_From_DTO()
        {
            var dto = new RuleTreeDto
            {
                Type = RuleTreeNodeType.Group,
                Name = "MainGroup",
                Operator = LogicalOperator.And,
                Children = new List<RuleTreeDto>
                {
                    new RuleTreeDto
                    {
                        Type = RuleTreeNodeType.Rule,
                        Name = "MinScore",
                        Expression = "Score >= 50"
                    },
                    new RuleTreeDto
                    {
                        Type = RuleTreeNodeType.Rule,
                        Name = "MaxScore",
                        Expression = "Score <= 100"
                    }
                }
            };

            var ruleTree = RuleTreeParser<DummyContext>.Parse(dto);

            // Compile expression-based rules
            var compiler = new RuleCompilerService<DummyContext>();
            compiler.Compile(ruleTree);

            var context = new DummyContext { Score = 75 };
            var result = ruleTree.Evaluate(context);

            Assert.True(result);
        }
    }
}
