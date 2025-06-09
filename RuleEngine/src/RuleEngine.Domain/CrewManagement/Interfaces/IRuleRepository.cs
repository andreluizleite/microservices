namespace RuleEngine.Domain.CrewManagement.Interfaces;

using RuleEngine.Domain.CrewManagement.Entities;

public interface IRuleRepository<T>
{
    Task<IEnumerable<Rule<T>>> GetAllAsync();
    Task AddAsync(Rule<T> rule);
}
