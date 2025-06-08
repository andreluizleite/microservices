using RuleEngine.Domain.Entities;
using RuleEngine.Domain.Interfaces;

public class HotLaunchCounter
{
    private readonly List<Rule<AssignmentContext>> _rules;
    private readonly IRuleEvaluator<AssignmentContext> _evaluator;

    public HotLaunchCounter(
        IEnumerable<Rule<AssignmentContext>> rules,
        IRuleEvaluator<AssignmentContext> evaluator)
    {
        _rules = rules.ToList();
        _evaluator = evaluator;
    }

    public void Execute(AssignmentContext context)
    {
        foreach (var rule in _rules)
        {
            if (_evaluator.Evaluate(rule, context))
            {
                rule.Action?.Invoke(context);
                ApplyCounter(context);
            }
        }
    }

    private void ApplyCounter(AssignmentContext context)
    {
        foreach (var leg in context.Assignments.OfType<Leg>())
        {
            leg.CounterValues.Add(new CounterValue
            {
                CounterTypeSystemName = context.CounterType,
                CounterValue_ = 1
            });
        }
    }
}

