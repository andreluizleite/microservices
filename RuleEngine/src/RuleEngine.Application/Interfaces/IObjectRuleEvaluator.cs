using RuleEngine.Domain.Core.Rules;

public interface IObjectRuleEvaluator<T>
{
    bool Evaluate(Rule<T> rule, T input);
}