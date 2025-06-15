namespace RuleEngine.Application.DTOs
{
    public class RuleEvaluationResponse
    {
        public bool Success { get; set; }

        public string? Error { get; set; }

        public decimal Result { get; set; } = 0m; 
        public List<string> FailedRules { get; set; } = new();
    }
}
