using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Application.Interfaces
{
    public interface IRuleCompilerService<T>
    {
        void Compile(IEnumerable<Rule<T>> rules);
    }
}
