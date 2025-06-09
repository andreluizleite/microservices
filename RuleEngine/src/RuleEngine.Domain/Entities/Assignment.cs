
namespace RuleEngine.Domain.Entities
{
    public abstract class Assignment : Creditable
    {
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public string? StartAirport { get; set; }
        public string? EndAirport { get; set; }
        public string? ServiceTypeCode { get; set; }
    }
}
