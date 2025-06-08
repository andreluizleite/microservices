namespace RuleEngine.Domain.Entities
{
    public class Rule<T>
    {
        public string Name { get; }
        public Func<T, bool>? CompiledCondition { get; }
        public Action<T>? Action { get; }

        private Rule(string name, Func<T, bool> compiledCondition, Action<T>? action = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CompiledCondition = compiledCondition ?? throw new ArgumentNullException(nameof(compiledCondition));
            Action = action;
        }

        public static Rule<T> Create(string name, Func<T, bool> condition, Action<T>? action = null) =>
            new(name, condition, action);
    }
}
