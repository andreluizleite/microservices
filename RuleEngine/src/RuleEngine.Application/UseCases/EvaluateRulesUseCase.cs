using RuleEngine.Domain.CrewManagement.Entities;

namespace RuleEngine.Application.Services;

public class ActivityRuleEvaluator : IObjectRuleEvaluator<Activity>
{
    public bool Evaluate(Rule<Activity> rule, Activity activity)
    {
        if (rule.CompiledCondition is null)
            throw new InvalidOperationException("Rule must have a compiled condition.");

        return rule.CompiledCondition(activity);
    }
}
