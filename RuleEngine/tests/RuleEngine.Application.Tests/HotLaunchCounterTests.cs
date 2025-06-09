using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;

namespace RuleEngine.Application.Tests;

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

        var rule = Rule<CrewManagementEvaluationContext>.Create(
            "ValidLegsExist",
            ctx => ctx.Assignments.OfType<Leg>().Any()
        );

        var rules = new List<Rule<CrewManagementEvaluationContext>> { rule };
        var evaluator = new ObjectRuleEvaluator<CrewManagementEvaluationContext>();
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

        counter.Execute(context);

        Assert.True(actionInvoked);
        var leg = context.Assignments[0] as Leg;
        Assert.Contains(leg.CounterValues, c => c.CounterTypeSystemName == "CustomAction" && c.CounterValue_ == 99);
    }

    [Fact]
    public void Execute_ShouldApplyCounter_WhenExpressionRuleMatches()
    {
        var context = CreateContext("IsLongFlight");

        var rule = Rule<CrewManagementEvaluationContext>.FromExpression(
            "ComplexFlightRule",
            "Legs.Any(FlightNumber > 100 && ServiceTypeCode == \"FLT\")"

        );


        var rules = new List<Rule<CrewManagementEvaluationContext>> { rule };
        var evaluator = new ExpressionRuleEvaluator<CrewManagementEvaluationContext>(ctx => new Dictionary<string, object>
        {
            { "Legs", ctx.Assignments.OfType<Leg>().ToList() },
            { "Assignments", ctx.Assignments }
        });
        var counter = new HotLaunchCounter(rules, evaluator);

        counter.Execute(context);

        var leg = context.Assignments[0] as Leg;
        Assert.Single(leg.CounterValues);
        Assert.Equal("IsLongFlight", leg.CounterValues[0].CounterTypeSystemName);
        Assert.Equal(1, leg.CounterValues[0].CounterValue_);
    }
}
