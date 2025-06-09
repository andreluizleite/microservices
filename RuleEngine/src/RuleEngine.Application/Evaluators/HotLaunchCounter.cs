using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;

public class HotLaunchCounter
{
    private readonly List<Rule<CrewManagementEvaluationContext>> _rules;
    private readonly IRuleEvaluator<CrewManagementEvaluationContext> _evaluator;

    public HotLaunchCounter(
        IEnumerable<Rule<CrewManagementEvaluationContext>> rules,
        IRuleEvaluator<CrewManagementEvaluationContext> evaluator)
    {
        _rules = rules.ToList();
        _evaluator = evaluator;
    }

    public void Execute(CrewManagementEvaluationContext context)
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
    private void ApplyCounter(CrewManagementEvaluationContext context)
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

