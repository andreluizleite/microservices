namespace RuleEngine.Domain.CrewManagement.Entities
{
    public class DutyPeriod : Creditable
    {
        public List<Assignment> Assignments { get; set; } = new();
    }
}
