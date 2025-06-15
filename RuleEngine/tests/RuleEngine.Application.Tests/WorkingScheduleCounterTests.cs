using RuleEngine.Application.Evaluators;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.WorkingSchedules.Entities;
using RuleEngine.Domain.WorkingSchedules.Entities.Evaluation;
using Xunit;

namespace RuleEngine.Application.Tests
{
    public class WorkingScheduleCounterTests
    {
        // Helper method to initialize test context with one shift and task
        private WorkingScheduleEvaluationContext CreateContext(string counterType)
        {
            return new WorkingScheduleEvaluationContext
            {
                Shifts = new List<WorkShift>
                {
                    new WorkShift
                    {
                        ShiftId = "SHIFT1",
                        AssignedTo = "User123",
                        Tasks = new List<WorkTask>
                        {
                            new WorkTask
                            {
                                TaskId = "TASK1",
                                Start = DateTime.UtcNow,
                                End = DateTime.UtcNow.AddDays(4),
                            }
                        }
                    }
                },
                CounterType = counterType
            };
        }

        [Fact]
        public void Should_Apply_Multiple_Counters_When_All_Rules_Match()
        {
            var context = CreateContext("BasicShift");

            // Rule 1: There is at least one shift
            var rule1 = Rule<WorkingScheduleEvaluationContext>.Create(
                "ValidShiftsExist",
                ctx => ctx.Shifts.Any()
            );

            // Rule 2: At least one task is longer than 6 days
            var rule2 = Rule<WorkingScheduleEvaluationContext>.Create(
                "MoreThan6Days",
                ctx => ctx.Shifts
                    .SelectMany(s => s.Tasks)
                    .Any(t => (t.End - t.Start) > TimeSpan.FromDays(6))
            );

            // Rule 3: Task ID ends with an odd digit
            var rule3 = Rule<WorkingScheduleEvaluationContext>.Create(
                "OddTaskIdEnding",
                ctx => ctx.Shifts
                    .SelectMany(s => s.Tasks)
                    .Any(t =>
                    {
                        var lastChar = t.TaskId?.LastOrDefault();
                        return char.IsDigit((char)lastChar) && int.Parse(lastChar.ToString()) % 2 != 0;
                    })
            );

            var rules = new List<Rule<WorkingScheduleEvaluationContext>> { rule1, rule2, rule3 };
            var evaluator = new ObjectRuleEvaluator<WorkingScheduleEvaluationContext>();
            var counter = new WorkingHoursCounter(rules, evaluator);

            // Act
            counter.Execute(context);

            // Assert
            var shift = context.Shifts.First();
            Assert.Equal(2, shift.CounterValues.Count); // Only rule1 and rule3 apply
            Assert.Contains(shift.CounterValues, c => c.CounterTypeSystemName == "ValidShiftsExist");
            Assert.Contains(shift.CounterValues, c => c.CounterTypeSystemName == "OddTaskIdEnding");
        }

        [Fact]
        public void Should_Invoke_Custom_Action_When_Rule_Matches()
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

            // Act
            counter.Execute(context);

            // Assert
            Assert.True(actionInvoked);
            var shift = context.Shifts.First();
            Assert.Contains(shift.CounterValues, c => c.CounterTypeSystemName == "CustomShift" && c.CounterValue_ == 42);
        }

        [Fact]
        public void Should_Apply_Counter_When_Expression_Rule_Matches()
        {
            var context = CreateContext("ExpressionShift");

            var rule = Rule<WorkingScheduleEvaluationContext>.FromExpression(
                "ExpressionShiftRule",
                "Shifts.Any(AssignedTo == \"User123\")"
            );

            var rules = new List<Rule<WorkingScheduleEvaluationContext>> { rule };

            var evaluator = new ObjectRuleEvaluator<WorkingScheduleEvaluationContext>();
            var counter = new WorkingHoursCounter(rules, evaluator);

            // Act
            counter.Execute(context);

            // Assert
            var shift = context.Shifts.First();
            Assert.Single(shift.CounterValues);
            Assert.Equal("ExpressionShiftRule", shift.CounterValues[0].CounterTypeSystemName);
            Assert.Equal(1, shift.CounterValues[0].CounterValue_);
        }
    }
}
