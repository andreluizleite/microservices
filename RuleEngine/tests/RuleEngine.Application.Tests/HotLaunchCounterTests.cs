using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;
namespace RuleEngine.Application.Tests
{
    public class HotLaunchCounterTests
    {
        private CrewManagementEvaluationContext CreateContext(string counterType)
        {
            return new CrewManagementEvaluationContext
            {
                Assignments = new List<Assignment>
                {
                    new Leg
                    {
                        FlightNumber = 123,
                        ServiceTypeCode = "Y",
                        ActualStart = DateTime.UtcNow,
                        ActualEnd = DateTime.UtcNow.AddHours(2)
                    },
                    new Leg
                    {
                        FlightNumber = 456,
                        ServiceTypeCode = "FLT",
                        ActualStart = DateTime.UtcNow.AddHours(2),
                        ActualEnd = DateTime.UtcNow.AddHours(5)
                    }
                },
                CounterType = counterType
            };
        }

        [Fact]
        public void Should_Apply_Counter_When_Group_Rules_Match()
        {
            var rule1 = Rule<CrewManagementEvaluationContext>.Create(
                "HasHotLeg",
                ctx => ctx.Assignments.OfType<Leg>().Any(l => l.ServiceTypeCode == "Y")
            );

            var rule2 = Rule<CrewManagementEvaluationContext>.Create(
                "HasCommercialFlight",
                ctx => ctx.Assignments.OfType<Leg>().Any(l => l.ServiceTypeCode == "FLT")
            );

            var group = RuleGroup<CrewManagementEvaluationContext>.And("HotLaunchAndFlight", rule1, rule2);
            var counter = new HotLaunchCounter(group);

            var context = CreateContext("HotAndCommercial");
            counter.Execute(context);

            var leg = context.Assignments[0] as Leg;
            Assert.Single(leg.CounterValues);
            Assert.Equal("HotAndCommercial", leg.CounterValues[0].CounterTypeSystemName);
            Assert.Equal(1, leg.CounterValues[0].CounterValue_);
        }
    }
}