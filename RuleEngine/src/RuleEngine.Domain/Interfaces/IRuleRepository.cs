namespace RuleEngine.Domain.Interfaces;
using RuleEngine.Domain.Entities;

public interface IRuleRepository<T>
{
    Task<IEnumerable<Rule<T>>> GetAllAsync();
    Task AddAsync(Rule<T> rule);
}
