using System;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

namespace RuleEngine.Domain.Core.Rules
{
    public class CalculationRule<T, TResult>
    {
        public string Name { get; }
        public string? ExpressionString { get; }
        public Func<T, TResult>? CompiledExpression { get; private set; }
        public Action<T, TResult>? Action { get; }

        public CalculationRule(string name, string expressionString, Action<T, TResult>? action = null)
        {
            Name = name;
            ExpressionString = expressionString;
            Action = action;
        }

        public void Compile()
        {
            if (string.IsNullOrWhiteSpace(ExpressionString))
                throw new InvalidOperationException("Expression string is required for compilation.");

            var parameter = Expression.Parameter(typeof(T), "x");

            var lambda = DynamicExpressionParser.ParseLambda(
                new[] { parameter },
                typeof(TResult),
                ExpressionString
            );

            CompiledExpression = (Func<T, TResult>)lambda.Compile();
        }

        public TResult Evaluate(T context)
        {
            if (CompiledExpression == null)
                throw new InvalidOperationException("Expression must be compiled before evaluation.");

            var result = CompiledExpression(context);
            Action?.Invoke(context, result);
            return result;
        }
    }
}
