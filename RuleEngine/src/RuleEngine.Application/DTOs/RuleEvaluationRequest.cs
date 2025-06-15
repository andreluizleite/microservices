namespace RuleEngine.Application.DTOs
{
    public class RuleEvaluationRequest
    {
        public string RuleId { get; set; } = string.Empty;
        public Dictionary<string, object> Input { get; set; } = new();
    }
}
