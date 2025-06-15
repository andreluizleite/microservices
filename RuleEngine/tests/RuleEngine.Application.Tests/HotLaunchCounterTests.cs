using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;
using Xunit;

namespace RuleEngine.Application.Tests
{
    public class HotLaunchCounterTests
    {
        // Helper method to generate a context with a single Leg assignment
        private CrewManagementEvaluationContext CreateContext(string counterType)
        {
            return new CrewManagementEvaluationContext
            {
                Assignments = new List<Assignment>
                {
                    new Leg
                    {
                        FlightNumber = 123,
                        ActualStart = DateTime.UtcNow,
                        ActualEnd = DateTime.UtcNow.AddHours(1),
                        ServiceTypeCode = "FLT"
                    }
                },
                CounterType = counterType
            };
        }

        [Fact]
        public void Should_Apply_Counter_Using_StronglyTyped_Rule()
        {
            // Arrange: Create a rule using a strongly typed predicate
            var context = CreateContext("IsHotLaunch");

            var rule = Rule<CrewManagementEvaluationContext>.Create(
                "ValidLegsExist",
                ctx => ctx.Assignments.OfType<Leg>().Any()
            );

            var rules = new List<Rule<CrewManagementEvaluationContext>> { rule };
            var evaluator = new ObjectRuleEvaluator<CrewManagementEvaluationContext>();
            var counter = new HotLaunchCounter(rules, evaluator);

            // Act
            counter.Execute(context);

            // Assert: Ensure a counter value was added to the Leg
            var leg = (Leg)context.Assignments[0];
            Assert.Single(leg.CounterValues);
            Assert.Equal("IsHotLaunch", leg.CounterValues[0].CounterTypeSystemName);
            Assert.Equal(1, leg.CounterValues[0].CounterValue_);
        }

        [Fact]
        public void Should_Invoke_Action_When_Rule_Matches()
        {
            // Arrange: Create a rule with an action that adds a custom counter value
            var context = CreateContext("ActionHotLaunch");

            bool actionInvoked = false;

            var rule = Rule<CrewManagementEvaluationContext>.Create(
                "ActionRule",
                ctx => ctx.Assignments.OfType<Leg>().Any(),
                action: ctx =>
                {
                    actionInvoked = true;
                    foreach (var leg in ctx.Assignments.OfType<Leg>())
                    {
                        leg.CounterValues.Add(new CounterValue("CustomAction", 99));
                    }
                });

            var rules = new List<Rule<CrewManagementEvaluationContext>> { rule };
            var evaluator = new ObjectRuleEvaluator<CrewManagementEvaluationContext>();
            var counter = new HotLaunchCounter(rules, evaluator);

            // Act
            counter.Execute(context);

            // Assert: Ensure action was invoked and the counter was added
            Assert.True(actionInvoked);
            var leg = (Leg)context.Assignments[0];
            Assert.Contains(leg.CounterValues, c => c.CounterTypeSystemName == "CustomAction" && c.CounterValue_ == 99);
        }

        [Fact]
        public void Should_Apply_Counter_When_Expression_Rule_Matches()
        {
            // Arrange: Create a rule using a dynamic expression string
            var context = CreateContext("IsLongFlight");

            var rule = Rule<CrewManagementEvaluationContext>.FromExpression(
                "ComplexFlightRule",
                "Legs.Any(FlightNumber > 100 && ServiceTypeCode == \"FLT\")"
            );

            var rules = new List<Rule<CrewManagementEvaluationContext>> { rule };

            var evaluator = new ObjectRuleEvaluator<CrewManagementEvaluationContext>();
            var counter = new HotLaunchCounter(rules, evaluator);

            // Act
            counter.Execute(context);

            // Assert: Ensure dynamic rule was evaluated and counter added
            var leg = (Leg)context.Assignments[0];
            Assert.Single(leg.CounterValues);
            Assert.Equal("IsLongFlight", leg.CounterValues[0].CounterTypeSystemName);
            Assert.Equal(1, leg.CounterValues[0].CounterValue_);
        }
    }
}
