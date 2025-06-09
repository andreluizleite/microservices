namespace RuleEngine.Domain.Core.Rules
{
    public class Rule<T>
    {
        public string Name { get; }
        public string? Expression { get; }
        public Func<T, bool>? CompiledCondition { get; }
        public Action<T>? Action { get; }

        public bool IsExpressionRule => !string.IsNullOrWhiteSpace(Expression);
        public bool IsLambdaRule => CompiledCondition is not null;

        private Rule(string name, Func<T, bool>? condition, string? expression = null, Action<T>? action = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Expression = expression;
            CompiledCondition = condition;
            Action = action;
        }

        public static Rule<T> Create(string name, Func<T, bool> condition, Action<T>? action = null)
            => new(name, condition, null, action);

        public static Rule<T> FromExpression(string name, string expression)
            => new(name, null, expression);
    }
}
