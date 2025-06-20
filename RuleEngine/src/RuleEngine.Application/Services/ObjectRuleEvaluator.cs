﻿using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Application.Services
{
    public class ObjectRuleEvaluator<T> : IObjectRuleEvaluator<T>, IRuleEvaluator<T>
    {
        public bool Evaluate(Rule<T> rule, T input)
        {
            if (rule.CompiledCondition == null)
                throw new InvalidOperationException("CompiledCondition is required.");

            return rule.CompiledCondition(input);
        }
    }

}
