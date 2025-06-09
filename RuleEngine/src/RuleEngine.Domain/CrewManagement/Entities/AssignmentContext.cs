namespace RuleEngine.Domain.CrewManagement.Entities
{
    public class AssignmentContext
    {
        public List<Assignment> Assignments { get; init; } = new();
        public string CounterType { get; init; } = string.Empty;
    }
}