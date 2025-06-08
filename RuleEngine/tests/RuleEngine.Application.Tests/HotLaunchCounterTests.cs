using RuleEngine.Application.Evaluators;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Entities;
namespace RuleEngine.Application.Tests
{
    public class HotLaunchCounterTests
    {
        [Fact]
        public void Execute_ShouldApplyCounter_WhenRuleMatches()
        {
            // Arrange
            var assignments = new List<Assignment>
            {
                new Leg
                {
                    FlightNumber = 123,
                    ActualStart = DateTime.UtcNow,
                    ActualEnd = DateTime.UtcNow.AddHours(1),
                    ServiceTypeCode = "FLT"
                }
            };

            var context = new AssignmentContext
            {
                Assignments = assignments,
                CounterType = "IsHotLaunch"
            };

            var rule = Rule<AssignmentContext>.Create(
                "ValidLegsExist",
                ctx => ctx.Assignments.OfType<Leg>().Any()
            );

            var rules = new List<Rule<AssignmentContext>> { rule };
            var evaluator = new ObjectRuleEvaluator<AssignmentContext>();
            var counter = new HotLaunchCounter(rules, evaluator);

            // Act
            counter.Execute(context);

            // Assert
            var leg = assignments[0] as Leg;
            Assert.Single(leg.CounterValues);
            Assert.Equal("IsHotLaunch", leg.CounterValues[0].CounterTypeSystemName);
            Assert.Equal(1, leg.CounterValues[0].CounterValue_);
        }
    }
}
