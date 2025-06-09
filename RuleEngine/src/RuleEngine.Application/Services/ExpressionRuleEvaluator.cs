using System.Linq.Dynamic.Core;
using RuleEngine.Domain.CrewManagement.Entities;
using RuleEngine.Domain.CrewManagement.Interfaces;

namespace RuleEngine.Application.Services
{
    public class ExpressionRuleEvaluator<T> : IRuleEvaluator<T>
    {
        public bool Evaluate(Rule<T> rule, T input)
        {
            if (string.IsNullOrWhiteSpace(rule.Expression))
                throw new InvalidOperationException("Rule must have a valid expression.");

            var inputList = new List<T> { input };
            var result = inputList.AsQueryable().Where(rule.Expression).Any();

            return result;
        }
    }
}
