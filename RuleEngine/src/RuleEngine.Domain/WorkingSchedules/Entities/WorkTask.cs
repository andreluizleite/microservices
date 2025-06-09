namespace RuleEngine.Domain.WorkingSchedules.Entities
{
    public class WorkTask
    {
        public string TaskId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
