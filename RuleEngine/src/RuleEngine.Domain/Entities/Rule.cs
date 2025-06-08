namespace RuleEngine.Domain.Entities
{
    public class Rule<T>
    {
        public string Name { get; }
        public Func<T, bool>? CompiledCondition { get; }

        private Rule(string name, Func<T, bool> compiledCondition)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CompiledCondition = compiledCondition ?? throw new ArgumentNullException(nameof(compiledCondition));
        }

        public static Rule<T> Create(string name, Func<T, bool> condition) =>
            new(name, condition);
    }
}
