using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Domain.Core.Interfaces;

public interface IRuleRepository<T>
{
    Task<IEnumerable<Rule<T>>> GetAllAsync();
    Task AddAsync(Rule<T> rule);
}
