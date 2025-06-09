using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Application.Evaluators
{
    public class CalculationEvaluator<T, TResult> : ICalculationEvaluator<T, TResult>
    {
        public TResult Evaluate(CalculationRule<T, TResult> rule, T context)
        {
            return rule.Evaluate(context);
        }
    }
}
