namespace RuleEngine.Domain.WorkingSchedules.Entities
{
    public class WorkingSchedule
    {
        public Guid WorkingScheduleId { get; set; }
        public List<WorkShift> Shifts { get; set; } = new();
    }
}
