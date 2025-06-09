using Microsoft.Extensions.DependencyInjection;
using RuleEngine.Application.Interfaces;
using RuleEngine.Application.Services;

namespace RuleEngine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRuleEngine(this IServiceCollection services)
    {
        // Expression evaluator (e.g., NCalc)
        services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();

        // Rule evaluators
      //  services.AddSingleton<IRuleEvaluator, RuleEvaluator>();
        services.AddScoped(typeof(IObjectRuleEvaluator<>), typeof(ObjectRuleEvaluator<>));

        // Rule repositories
       // services.AddSingleton(typeof(IRuleRepository<>), typeof(InMemoryRuleRepository<>));

        return services;
    }
}
