using RuleEngine.Application.Interfaces;
using RuleEngine.Domain.Entities;

namespace RuleEngine.Application.Services
{
    public class ObjectRuleEvaluator<T> : IObjectRuleEvaluator<T>
    {
        public bool Evaluate(Rule<T> rule, T input)
        {
            if (rule.CompiledCondition is null)
                throw new InvalidOperationException("Missing compiled condition.");

            return rule.CompiledCondition(input);
        }
    }

}
