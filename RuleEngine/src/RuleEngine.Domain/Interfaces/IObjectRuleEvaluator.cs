using RuleEngine.Domain.Entities;

namespace RuleEngine.Domain.Interfaces
{
    public interface IObjectRuleEvaluator<T>
    {
        bool Evaluate(Rule<T> rule, T input);
    }
}
