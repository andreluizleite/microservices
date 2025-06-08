//using System.Collections.Immutable;

//namespace RuleEngine.Application.Services;

//public class RuleService
//{
//    private readonly IRuleEvaluator _evaluator;

//    public RuleService(IRuleEvaluator evaluator)
//    {
//        _evaluator = evaluator;
//    }

//    public bool Execute(Rule rule, IDictionary<string, object> context)
//    {
//        var immutableContext = context.ToImmutableDictionary();
//        return _evaluator.Evaluate(rule, immutableContext);
//    }
//}
