using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Domain.Core.Interfaces
{
    public interface ICalculationEvaluator<T, TResult>
    {
        TResult Evaluate(CalculationRule<T, TResult> rule, T context);
    }
}
