using NCalc;
using RuleEngine.Application.Interfaces;

namespace RuleEngine.Application.Services;

public class ExpressionEvaluator : IExpressionEvaluator
{
    public bool Evaluate(string expression, IReadOnlyDictionary<string, object> context)
    {
        var expr = new Expression(expression);

        foreach (var kvp in context)
        {
            expr.Parameters[kvp.Key] = kvp.Value;
        }

        var result = expr.Evaluate();
        return result is bool b && b;
    }
}
