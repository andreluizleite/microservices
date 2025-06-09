using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.WorkingSchedules.Entities.Evaluation;

namespace RuleEngine.Application.Evaluators
{
    public class WorkingHoursCounter
    {
        private readonly List<Rule<WorkingScheduleEvaluationContext>> _rules;
        private readonly IRuleEvaluator<WorkingScheduleEvaluationContext> _evaluator;

        public WorkingHoursCounter(
            IEnumerable<Rule<WorkingScheduleEvaluationContext>> rules,
            IRuleEvaluator<WorkingScheduleEvaluationContext> evaluator)
        {
            _rules = rules.ToList();
            _evaluator = evaluator;
        }

        public void Execute(WorkingScheduleEvaluationContext context)
        {
            foreach (var rule in _rules)
            {
                if (_evaluator.Evaluate(rule, context))
                {
                    rule.Action?.Invoke(context);
                    context.CounterType = rule.Name;
                    ApplyCounter(context);
                }
            }
        }

        private void ApplyCounter(WorkingScheduleEvaluationContext context)
        {
            foreach (var shift in context.Shifts)
            {
                shift.CounterValues.Add(new CounterValue
                {
                    CounterTypeSystemName = context.CounterType,
                    CounterValue_ = 1
                });
            }
        }
    }
}
