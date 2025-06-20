using RuleEngine.Application.Interfaces;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.CrewManagement.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IRulePersistenceService, InMemoryRulePersistenceService>();
builder.Services.AddSingleton<IRuleCompilerService<Dictionary<string, object>>, RuleCompilerService<Dictionary<string, object>>>();
builder.Services.AddScoped<IObjectRuleEvaluator<Activity>, ActivityRuleEvaluator>();
builder.Services.AddScoped<HotLaunchCounter>();
builder.Services.AddScoped<IObjectRuleEvaluator<AssignmentContext>, ObjectRuleEvaluator<AssignmentContext>>();
builder.Services.AddSingleton(typeof(IRuleEvaluator<>), typeof(ExpressionRuleEvaluator<>));
builder.Services.AddSingleton(typeof(IObjectRuleEvaluator<>), typeof(ObjectRuleEvaluator<>));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
