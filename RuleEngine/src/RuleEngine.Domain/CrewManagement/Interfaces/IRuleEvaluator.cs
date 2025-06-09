namespace RuleEngine.Domain.CrewManagement.Interfaces
{
    using RuleEngine.Domain.CrewManagement.Entities;

    public interface IRuleEvaluator<T>
    {
        bool Evaluate(Rule<T> rule, T input);
    }
}
