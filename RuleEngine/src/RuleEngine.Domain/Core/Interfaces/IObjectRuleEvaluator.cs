using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Domain.Core.Interfaces
{
    public interface IObjectRuleEvaluator<T>
    {
        bool Evaluate(Rule<T> rule, T input);
    }
}
