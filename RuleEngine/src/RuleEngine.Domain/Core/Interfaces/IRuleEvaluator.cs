namespace RuleEngine.Domain.Core.Interfaces
{
    using RuleEngine.Domain.Core.Rules;

    public interface IRuleEvaluator<T>
    {
        bool Evaluate(Rule<T> rule, T input);
    }
}
