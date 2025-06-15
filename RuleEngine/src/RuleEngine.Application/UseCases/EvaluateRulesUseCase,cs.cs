using RuleEngine.Application.Interfaces;
using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Application.UseCases
{
    public class EvaluateRulesUseCase<T>
    {
        private readonly IRuleCompilerService<T> _compiler;
        private readonly IRuleEvaluator<T> _evaluator;

        public EvaluateRulesUseCase(IRuleCompilerService<T> compiler, IRuleEvaluator<T> evaluator)
        {
            _compiler = compiler;
            _evaluator = evaluator;
        }

        public RuleEvaluationResult Execute(IEnumerable<Rule<T>> rules, T input)
        {
            _compiler.Compile(rules);

            var failed = rules
                .Where(r => !_evaluator.Evaluate(r, input))
                .Select(r => r.Name)
                .ToList();

            return new RuleEvaluationResult
            {
                Success = failed.Count == 0,
                FailedRules = failed
            };
        }
    }
}
