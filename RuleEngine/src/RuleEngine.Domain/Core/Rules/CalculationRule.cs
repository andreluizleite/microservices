namespace RuleEngine.Domain.Core.Rules
{
    public class CalculationRule<T, TResult>
    {
        public string Name { get; }
        public Func<T, TResult> Expression { get; }
        public Action<T, TResult>? Action { get; }

        public CalculationRule(string name, Func<T, TResult> expression, Action<T, TResult>? action = null)
        {
            Name = name;
            Expression = expression;
            Action = action;
        }

        public TResult Evaluate(T context)
        {
            var result = Expression(context);
            Action?.Invoke(context, result);
            return result;
        }
    }
}
