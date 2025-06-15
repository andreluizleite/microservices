namespace RuleEngine.Domain.Core.Interfaces
{
    public interface IRuleNode<T>
    {
        bool Evaluate(T input);
        string Name { get; }
    }
}
