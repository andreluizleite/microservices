namespace RuleEngine.Application.DTOs
{
    public class RuleEvaluationResponse
    {
        public bool Success { get; set; }
        public List<string> FailedRules { get; set; } = new();
    }
}
