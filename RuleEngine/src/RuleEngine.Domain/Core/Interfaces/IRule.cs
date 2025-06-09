namespace RuleEngine.Domain.Core.Interfaces;

public interface IRule
{
    string Name { get; }
    string Expression { get; }
}
