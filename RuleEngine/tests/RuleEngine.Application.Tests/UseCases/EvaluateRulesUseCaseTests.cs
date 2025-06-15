using RuleEngine.Application.Evaluators;
using RuleEngine.Application.Services;
using RuleEngine.Application.UseCases;
using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;
using Xunit;

namespace RuleEngine.Application.Tests.UseCases
{
    public class EvaluateRulesUseCaseTests
    {
        public class Employee
        {
            public int Age { get; set; }
            public string Department { get; set; }
            public bool IsManager { get; set; }
        }

        [Fact]
        public void Should_Evaluate_Simple_Rules_And_Return_Success()
        {
            // Arrange
            var rules = new List<Rule<Employee>>
            {
                Rule<Employee>.FromExpression("AgeRule", "Age >= 30"),
                Rule<Employee>.FromExpression("DeptRule", "Department == \"Engineering\"")
            };

            var input = new Employee { Age = 35, Department = "Engineering" };

            var compiler = new RuleCompilerService<Employee>();
            var evaluator = new ObjectRuleEvaluator<Employee>();
            var useCase = new EvaluateRulesUseCase<Employee>(compiler, evaluator);

            // Act
            var result = useCase.Execute(rules, input);

            // Assert
            Assert.True(result.Success);
            Assert.Empty(result.FailedRules);
        }

        [Fact]
        public void Should_Evaluate_Simple_Rules_And_Return_FailedRules()
        {
            // Arrange
            var rules = new List<Rule<Employee>>
            {
                Rule<Employee>.FromExpression("AgeRule", "Age >= 40"),
                Rule<Employee>.FromExpression("DeptRule", "Department == \"Engineering\"")
            };

            var input = new Employee { Age = 30, Department = "Marketing" };

            var compiler = new RuleCompilerService<Employee>();
            var evaluator = new ObjectRuleEvaluator<Employee>();
            var useCase = new EvaluateRulesUseCase<Employee>(compiler, evaluator);

            // Act
            var result = useCase.Execute(rules, input);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("AgeRule", result.FailedRules);
            Assert.Contains("DeptRule", result.FailedRules);
        }

        [Fact]
        public void Should_Evaluate_CalculationRule_And_Return_Decimal_Result()
        {
            // Arrange
            var rule = new CalculationRule<Employee, decimal>(
                "HotCredit",
                "Convert.ToDecimal(Age * 0.25)"
            );

            rule.Compile();

            var input = new Employee { Age = 40 };
            var evaluator = new CalculationEvaluator<Employee, decimal>();

            // Act
            var result = evaluator.Evaluate(rule, input);

            // Assert
            Assert.Equal(10.0m, result);
        }
    }
}
