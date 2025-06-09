using System;

namespace RuleEngine.Domain.WorkingSchedules.Entities
{
    public class BreakPeriod
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Description { get; set; } = string.Empty;

        public TimeSpan Duration => End - Start;

        public bool IsPaid { get; set; } = false;
    }
}
