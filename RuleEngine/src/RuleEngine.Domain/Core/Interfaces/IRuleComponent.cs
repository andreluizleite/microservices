namespace RuleEngine.Domain.Core.Interfaces
{
    public interface IRuleComponent<T>
    {
        bool Evaluate(T input);
        string Name { get; }
    }
}
