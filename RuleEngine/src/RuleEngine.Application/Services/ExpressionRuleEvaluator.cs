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

            var contextVariables = _contextMapper(context);

            // Build dynamic type and instance for evaluation
            var parameters = contextVariables
                .Select(kvp => new DynamicProperty(kvp.Key, kvp.Value?.GetType() ?? typeof(object)))
                .ToList();

            var inputType = DynamicClassFactory.CreateType(parameters);
            var inputInstance = Activator.CreateInstance(inputType);

            foreach (var prop in inputType.GetProperties())
            {
                if (contextVariables.TryGetValue(prop.Name, out var value))
                {
                    prop.SetValue(inputInstance, value);
                }
            }

            // Parse and evaluate the expression
            var parsedLambda = DynamicExpressionParser.ParseLambda(
                inputType, typeof(bool), rule.Expression);

            return (bool)parsedLambda.Compile().DynamicInvoke(inputInstance);
        }
    }
}
