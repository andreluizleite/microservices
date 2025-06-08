namespace RuleEngine.Domain.Interfaces
{
    using RuleEngine.Domain.Entities;

    public interface IRuleEvaluator<T>
    {
        bool Evaluate(Rule<T> rule, T input);
    }
}
