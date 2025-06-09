using RuleEngine.Application.Interfaces;
using RuleEngine.Domain.CrewManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser;

namespace RuleEngine.Application.Services
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        public bool Evaluate(string expression, IReadOnlyDictionary<string, object> context)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression must not be null or empty.", nameof(expression));

            var parameters = context.Select(kvp => new DynamicProperty(kvp.Key, kvp.Value?.GetType() ?? typeof(object))).ToList();
            var inputType = DynamicClassFactory.CreateType(parameters);

            var inputInstance = Activator.CreateInstance(inputType);
            foreach (var prop in inputType.GetProperties())
            {
                if (context.TryGetValue(prop.Name, out var value))
                {
                    prop.SetValue(inputInstance, value);
                }
            }

            // Registrar tipos customizados aqui
            var parsingConfig = new ParsingConfig
            {
                CustomTypeProvider = new CustomTypeProvider()
            };

            // Avaliar expressão
            var lambda = DynamicExpressionParser
                .ParseLambda(parsingConfig, inputType, typeof(bool), expression);

            var result = lambda.Compile().DynamicInvoke(inputInstance);
            return result is bool b && b;
        }
    }

    // Adiciona os tipos reconhecíveis por nome simples (sem namespace)
    public class CustomTypeProvider : DefaultDynamicLinqCustomTypeProvider
    {
        public override HashSet<Type> GetCustomTypes()
        {
            var types = base.GetCustomTypes();
            types.Add(typeof(Leg));
            types.Add(typeof(Assignment));
            return types;
        }
    }
}
