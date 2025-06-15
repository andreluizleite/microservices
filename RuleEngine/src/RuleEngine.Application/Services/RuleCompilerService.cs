using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using RuleEngine.Application.Interfaces;
using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Application.Services
{
    public class RuleCompilerService<T> : IRuleCompilerService<T>
    {
        public void Compile(IEnumerable<Rule<T>> rules)
        {
            foreach (var rule in rules)
            {
                if (rule.IsExpressionRule && rule.CompiledCondition == null)
                {
                    try
                    {
                        var parameter = Expression.Parameter(typeof(T), "x");
                        var lambda = DynamicExpressionParser.ParseLambda(
                            new[] { parameter },
                            typeof(bool),
                            rule.Expression!
                        );

                        var compiled = (Func<T, bool>)lambda.Compile();

                        typeof(Rule<T>)
                            .GetProperty(nameof(Rule<T>.CompiledCondition))!
                            .SetValue(rule, compiled);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"Error compiling rule '{rule.Name}': {ex.Message}", ex);
                    }
                }
            }
        }
    }
}
