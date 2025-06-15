using RuleEngine.Domain.Core.Interfaces;

namespace RuleEngine.Domain.Core.Rules
{
    public class RuleGroup<T> : IRuleComponent<T>
    {
        public string Name { get; }
        public LogicalOperator Operator { get; }
        public List<IRuleComponent<T>> Children { get; }

        public RuleGroup(string name, LogicalOperator op, List<IRuleComponent<T>> children)
        {
            Name = name;
            Operator = op;
            Children = children ?? throw new ArgumentNullException(nameof(children));
        }

        public bool Evaluate(T input)
        {
            return Operator switch
            {
                LogicalOperator.And => Children.TrueForAll(c => c.Evaluate(input)),
                LogicalOperator.Or => Children.Exists(c => c.Evaluate(input)),
                LogicalOperator.Not => !Children[0].Evaluate(input),
                _ => throw new NotSupportedException("Unknown logical operator")
            };
        }
    }
}
