namespace RuleEngine.Domain.CrewManagement.Interfaces;

public interface IRule
{
    string Name { get; }
    string Expression { get; }
}
