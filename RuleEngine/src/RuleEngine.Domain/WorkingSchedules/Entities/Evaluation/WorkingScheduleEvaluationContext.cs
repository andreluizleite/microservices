using RuleEngine.Domain.WorkingSchedules.Entities;

namespace RuleEngine.Domain.WorkingSchedules.Entities.Evaluation
{
    public class WorkingScheduleEvaluationContext
    {
        // The main schedule object being evaluated
        public WorkingSchedule? Schedule { get; set; }

        // All shifts part of the evaluation
        public IEnumerable<WorkShift> Shifts { get; set; } = new List<WorkShift>();

        // Tasks performed during the schedule
        public IEnumerable<WorkTask> Tasks { get; set; } = new List<WorkTask>();

        // Breaks taken in the schedule
        public IEnumerable<BreakPeriod> Breaks { get; set; } = new List<BreakPeriod>();

        // The type of counter this context applies to (e.g., TotalHours, UnpaidBreaks)
        public string CounterType { get; set; } = string.Empty;
    }
}
