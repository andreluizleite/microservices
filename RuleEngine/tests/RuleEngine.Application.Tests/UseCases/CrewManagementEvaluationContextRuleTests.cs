using RuleEngine.Application.Evaluators;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;
using Xunit;

namespace RuleEngine.Application.Tests.UseCases
{
    public class CrewManagementEvaluationContextRuleTests
    {
        [Fact]
        public void Should_Apply_Credit_When_Y_Hot_Precedes_C_Leg()
        {
            // Arrange
            var hot = new GroundActivity
            {
                ServiceTypeCode = "Y",
                ScheduledStart = new DateTime(2025, 6, 10, 8, 0, 0)
            };

            var leg = new Leg
            {
                ServiceTypeCode = "C",
                ScheduledStart = new DateTime(2025, 6, 10, 9, 0, 0),
                FlightNumber = 123
            };

            var context = new CrewManagementEvaluationContext
            {
                Assignments = new List<Assignment> { hot, leg }
            };

            var rule = new CalculationRule<CrewManagementEvaluationContext, decimal>(
                name: "HotToFlightTimeCredit",
                expressionString: @"
                    Assignments.Any(ServiceTypeCode == ""Y"") && 
                    Assignments.Any(ServiceTypeCode == ""C"") ?
                    Convert.ToDecimal(
                        (Assignments.First(ServiceTypeCode == ""C"").ScheduledStart - 
                         Assignments.First(ServiceTypeCode == ""Y"").ScheduledStart).TotalHours * 0.5
                    )
                    : Convert.ToDecimal(0)"
            );

            rule.Compile();
            var evaluator = new CalculationEvaluator<CrewManagementEvaluationContext, decimal>();

            // Act
            var result = evaluator.Evaluate(rule, context);

            // Assert
            Assert.Equal(0.5m, result); // 1 hour * 0.5
        }

        [Fact]
        public void Should_Return_Zero_If_Leg_Not_Present()
        {
            // Arrange
            var hot = new GroundActivity
            {
                ServiceTypeCode = "Y",
                ScheduledStart = new DateTime(2025, 6, 10, 8, 0, 0)
            };

            var context = new CrewManagementEvaluationContext
            {
                Assignments = new List<Assignment> { hot }
            };

            var rule = new CalculationRule<CrewManagementEvaluationContext, decimal>(
                name: "HotToFlightTimeCredit",
                expressionString: @"
                    Assignments.Any(ServiceTypeCode == ""Y"") && 
                    Assignments.Any(ServiceTypeCode == ""C"") ?
                    Convert.ToDecimal(
                        (Assignments.First(ServiceTypeCode == ""C"").ScheduledStart - 
                         Assignments.First(ServiceTypeCode == ""Y"").ScheduledStart).TotalHours * 0.5
                    )
                    : Convert.ToDecimal(0)"
            );

            rule.Compile();
            var evaluator = new CalculationEvaluator<CrewManagementEvaluationContext, decimal>();

            // Act
            var result = evaluator.Evaluate(rule, context);

            // Assert
            Assert.Equal(0m, result);
        }
    }
}
