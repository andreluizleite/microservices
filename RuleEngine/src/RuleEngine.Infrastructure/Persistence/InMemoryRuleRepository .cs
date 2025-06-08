using RuleEngine.Domain.Entities;
using RuleEngine.Domain.Interfaces;

namespace RuleEngine.Infrastructure.Persistence;
public class InMemoryRuleRepository<T> : IRuleRepository<T>
{
    private readonly List<Rule<T>> _rules = new();

    public Task AddAsync(Rule<T> rule)
    {
        _rules.Add(rule);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Rule<T>>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Rule<T>>>(_rules);
    }
}
