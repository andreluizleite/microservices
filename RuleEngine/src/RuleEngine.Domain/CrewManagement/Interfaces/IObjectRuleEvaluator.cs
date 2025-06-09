using RuleEngine.Domain.CrewManagement.Entities;

namespace RuleEngine.Domain.CrewManagement.Interfaces
{
    public interface IObjectRuleEvaluator<T>
    {
        bool Evaluate(Rule<T> rule, T input);
    }
}
