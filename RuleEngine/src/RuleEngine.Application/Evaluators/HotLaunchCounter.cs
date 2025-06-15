using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Interfaces.RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Entities.Evaluation;

public class HotLaunchCounter : ICounter<CrewManagementEvaluationContext>
{
    private readonly IRuleNode<CrewManagementEvaluationContext> _ruleTree;
    public HotLaunchCounter(IRuleNode<CrewManagementEvaluationContext> ruleTree)
    {
        _ruleTree = ruleTree;
    }

    public void Execute(CrewManagementEvaluationContext context)
    {
        if (_ruleTree.Evaluate(context))
        {
            foreach (var leg in context.Assignments.OfType<Leg>())
            {
                leg.CounterValues.Add(new CounterValue(context.CounterType, 1));
            }
        }
    }

}

