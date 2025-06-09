using RuleEngine.Application.Services;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;

namespace RuleEngine.Application.Tests;

public class HotLaunchCounterTests
{
    private RuleEvaluationContext CreateContext(string counterType)
    {
        return new RuleEvaluationContext
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
    public void Execute_ShouldApplyCounter_WhenRuleMatches()
    {
        var context = CreateContext("IsHotLaunch");

        var rule = Rule<RuleEvaluationContext>.Create(
            "ValidLegsExist",
            ctx => ctx.Assignments.OfType<Leg>().Any()
        );

        var rules = new List<Rule<RuleEvaluationContext>> { rule };
        var evaluator = new ObjectRuleEvaluator<RuleEvaluationContext>();
        var counter = new HotLaunchCounter(rules, evaluator);

        counter.Execute(context);

        var leg = context.Assignments[0] as Leg;
        Assert.Single(leg.CounterValues);
        Assert.Equal("IsHotLaunch", leg.CounterValues[0].CounterTypeSystemName);
        Assert.Equal(1, leg.CounterValues[0].CounterValue_);
    }

    [Fact]
    public void Execute_ShouldInvokeAction_WhenRuleMatches()
    {
        var context = CreateContext("ActionHotLaunch");

        bool actionInvoked = false;

        var rule = Rule<RuleEvaluationContext>.Create(
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

        var rules = new List<Rule<RuleEvaluationContext>> { rule };
        var evaluator = new ObjectRuleEvaluator<RuleEvaluationContext>();
        var counter = new HotLaunchCounter(rules, evaluator);

        counter.Execute(context);

        Assert.True(actionInvoked);
        var leg = context.Assignments[0] as Leg;
        Assert.Contains(leg.CounterValues, c => c.CounterTypeSystemName == "CustomAction" && c.CounterValue_ == 99);
    }

    [Fact]
    public void Execute_ShouldApplyCounter_WhenExpressionRuleMatches()
    {
        var context = CreateContext("IsLongFlight");

        var rule = Rule<RuleEvaluationContext>.FromExpression(
            "ComplexFlightRule",
            "Legs.Any(FlightNumber > 100 && ServiceTypeCode == \"FLT\")"

        );


        var rules = new List<Rule<RuleEvaluationContext>> { rule };
        var evaluator = new ExpressionRuleEvaluator<RuleEvaluationContext>();
        var counter = new HotLaunchCounter(rules, evaluator);

        counter.Execute(context);

        var leg = context.Assignments[0] as Leg;
        Assert.Single(leg.CounterValues);
        Assert.Equal("IsLongFlight", leg.CounterValues[0].CounterTypeSystemName);
        Assert.Equal(1, leg.CounterValues[0].CounterValue_);
    }
}
