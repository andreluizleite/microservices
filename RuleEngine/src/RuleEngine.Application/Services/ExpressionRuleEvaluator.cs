using System.Linq.Dynamic.Core;
using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Application.Services
{
    public class ExpressionRuleEvaluator<T> : IRuleEvaluator<T>
    {
        private readonly Func<T, Dictionary<string, object>> _contextMapper;

        public ExpressionRuleEvaluator(Func<T, Dictionary<string, object>> contextMapper)
        {
            _contextMapper = contextMapper;
        }

        public bool Evaluate(Rule<T> rule, T context)
        {
            if (string.IsNullOrWhiteSpace(rule.Expression))
                throw new InvalidOperationException("Rule must have a valid expression.");

            var expression = rule.Expression;
            var contextVariables = _contextMapper(context);

            var evaluator = new ExpressionEvaluator();
            return evaluator.Evaluate(expression, contextVariables);
        }
    }

}
