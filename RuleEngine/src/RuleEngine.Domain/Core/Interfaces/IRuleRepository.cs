namespace RuleEngine.Domain.Core.Interfaces;

using RuleEngine.Domain.Core.Rules;

public interface IRuleRepository<T>
{
    Task<IEnumerable<Rule<T>>> GetAllAsync();
    Task AddAsync(Rule<T> rule);
}
