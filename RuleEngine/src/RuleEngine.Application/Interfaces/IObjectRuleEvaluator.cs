using RuleEngine.Domain.CrewManagement.Entities;

public interface IObjectRuleEvaluator<T>
{
    bool Evaluate(Rule<T> rule, T input);
}