namespace RuleEngine.Application.Interfaces;

public interface IExpressionEvaluator
{
    bool Evaluate(string expression, IReadOnlyDictionary<string, object> context);
}
