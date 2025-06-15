using RuleEngine.Domain.Core.Interfaces;

namespace RuleEngine.Domain.Core.Rules
{
    public class RuleGroup<T> : IRuleNode<T>
    {
        public string Name { get; }
        public LogicalOperator Operator { get; }
        public List<IRuleNode<T>> Rules { get; }

        public RuleGroup(string name, LogicalOperator op, params IRuleNode<T>[] rules)
        {
            Name = name;
            Operator = op;
            Rules = rules.ToList();
        }

        public bool Evaluate(T context)
        {
            return Operator switch
            {
                LogicalOperator.And => Rules.All(r => r.Evaluate(context)),
                LogicalOperator.Or => Rules.Any(r => r.Evaluate(context)),
                LogicalOperator.Not => Rules.Count == 1 && !Rules[0].Evaluate(context),
                _ => throw new InvalidOperationException("Unsupported logical operator")
            };
        }

        public static RuleGroup<T> And(string name, params IRuleNode<T>[] rules) =>
            new(name, LogicalOperator.And, rules);

        public static RuleGroup<T> Or(string name, params IRuleNode<T>[] rules) =>
            new(name, LogicalOperator.Or, rules);

        public static RuleGroup<T> Not(string name, IRuleNode<T> rule) =>
            new(name, LogicalOperator.Not, rule);
    }

}
