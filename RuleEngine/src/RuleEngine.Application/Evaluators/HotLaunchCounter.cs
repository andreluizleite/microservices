using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;
using RuleEngine.Domain.CrewManagement.Interfaces;

public class HotLaunchCounter
{
    private readonly List<Rule<RuleEvaluationContext>> _rules;
    private readonly IRuleEvaluator<RuleEvaluationContext> _evaluator;

    public HotLaunchCounter(
        IEnumerable<Rule<RuleEvaluationContext>> rules,
        IRuleEvaluator<RuleEvaluationContext> evaluator)
    {
        _rules = rules.ToList();
        _evaluator = evaluator;
    }

    public void Execute(RuleEvaluationContext context)
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
    private void ApplyCounter(RuleEvaluationContext context)
    {
        foreach (var assignment in context.Assignments)
        {
            if (assignment is Leg leg)
            {
                leg.CounterValues.Add(new CounterValue
                {
                    CounterTypeSystemName = context.CounterType,
                    CounterValue_ = 1
                });
            }
        }
    }

}

