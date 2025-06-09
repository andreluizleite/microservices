using RuleEngine.Domain.CrewManagement.Entities;
using System.Collections.Generic;

namespace RuleEngine.Domain.WorkingSchedules.Entities
{
    public class WorkShift
    {
        public string ShiftId { get; set; } = string.Empty;
        public List<WorkTask> Tasks { get; set; } = new();
        public string AssignedTo { get; set; } = string.Empty;

        public List<CounterValue> CounterValues { get; set; } = new();
    }
}
