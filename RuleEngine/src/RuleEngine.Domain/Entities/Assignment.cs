
namespace RuleEngine.Domain.Entities
{
    public abstract class Assignment
    {
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public string ServiceTypeCode { get; set; } = string.Empty;
    }
}
