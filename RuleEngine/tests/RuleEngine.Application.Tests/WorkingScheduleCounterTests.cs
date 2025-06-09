using RuleEngine.Application.Evaluators;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.WorkingSchedules.Entities;
using RuleEngine.Domain.WorkingSchedules.Entities.Evaluation;
using Xunit;

namespace RuleEngine.Application.Tests;

public class WorkingScheduleCounterTests
{
    private WorkingScheduleEvaluationContext CreateContext(string counterType)
    {
        return new WorkingScheduleEvaluationContext
        {
            Shifts = new List<WorkShift>
            {
                new WorkShift
                {
                    ShiftId = "SHIFT1",
                    AssignedTo = "User123"
                }
            },
            CounterType = counterType
        };
    }

    [Fact]
    public void Execute_ShouldApplyCounter_WhenRuleMatches()
    {
        var context = CreateContext("BasicShift");

        var rule = Rule<WorkingScheduleEvaluationContext>.Create(
            "ValidShiftsExist",
            ctx => ctx.Shifts.Any()
        );

        var rules = new List<Rule<WorkingScheduleEvaluationContext>> { rule };
        var evaluator = new ObjectRuleEvaluator<WorkingScheduleEvaluationContext>();
        var counter = new WorkingHoursCounter(rules, evaluator);

        counter.Execute(context);

        var shift = context.Shifts.First();
        Assert.Single(shift.CounterValues);
        Assert.Equal("BasicShift", shift.CounterValues[0].CounterTypeSystemName);
        Assert.Equal(1, shift.CounterValues[0].CounterValue_);
    }

    [Fact]
    public void Execute_ShouldInvokeAction_WhenRuleMatches()
    {
        var context = CreateContext("ActionShift");

        bool actionInvoked = false;

        var rule = Rule<WorkingScheduleEvaluationContext>.Create(
            "ActionRule",
            ctx => ctx.Shifts.Any(),
            action: ctx =>
            {
                actionInvoked = true;
                foreach (var shift in ctx.Shifts)
                {
                    shift.CounterValues.Add(new CounterValue("CustomShift", 42));
                }
            });

        var rules = new List<Rule<WorkingScheduleEvaluationContext>> { rule };
        var evaluator = new ObjectRuleEvaluator<WorkingScheduleEvaluationContext>();
        var counter = new WorkingHoursCounter(rules, evaluator);

        counter.Execute(context);

        Assert.True(actionInvoked);
        var shift = context.Shifts.First();
        Assert.Contains(shift.CounterValues, c => c.CounterTypeSystemName == "CustomShift" && c.CounterValue_ == 42);
    }

    [Fact]
    public void Execute_ShouldApplyCounter_WhenExpressionRuleMatches()
    {
        var context = CreateContext("ExpressionShift");

        var rule = Rule<WorkingScheduleEvaluationContext>.FromExpression(
            "ExpressionShiftRule",
            "Shifts.Any(AssignedTo == \"User123\")"
        );

        var rules = new List<Rule<WorkingScheduleEvaluationContext>> { rule };
        var evaluator = new ExpressionRuleEvaluator<WorkingScheduleEvaluationContext>(ctx => new Dictionary<string, object>
        {
            { "Shifts", ctx.Shifts }
        });

        var counter = new WorkingHoursCounter(rules, evaluator);

        counter.Execute(context);

        var shift = context.Shifts.First();
        Assert.Single(shift.CounterValues);
        Assert.Equal("ExpressionShift", shift.CounterValues[0].CounterTypeSystemName);
        Assert.Equal(1, shift.CounterValues[0].CounterValue_);
    }
}
