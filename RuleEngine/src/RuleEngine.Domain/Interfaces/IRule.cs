namespace RuleEngine.Domain.Interfaces;

public interface IRule
{
    string Name { get; }
    string Expression { get; }
}
