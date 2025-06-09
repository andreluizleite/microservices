using RuleEngine.Domain.CrewManagement.Enums;

namespace RuleEngine.Domain.CrewManagement.Entities.Evaluation
{
    public class CrewManagementEvaluationContext
    {
        /// <summary>
        /// Creditable entities under evaluation (e.g., DutyPeriod, Leg, GroundActivity).
        /// </summary>
        public List<Creditable> Creditables { get; set; } = new();

        /// <summary>
        /// Daily duty periods within the evaluation scope.
        /// </summary>
        public List<DutyPeriod> DutyPeriods { get; set; } = new();

        /// <summary>
        /// All assignments (e.g., Leg, GroundActivity).
        /// </summary>
        public List<Assignment> Assignments { get; set; } = new();

        /// <summary>
        /// Filtered list of Legs.
        /// </summary>
        public List<Leg> Legs => Assignments.OfType<Leg>().ToList();


        /// <summary>
        /// Counter type being calculated.
        /// </summary>
        public string CounterType { get; set; } = string.Empty;

        /// <summary>
        /// Optional parameters for dynamic rule evaluation.
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new();

        /// <summary>
        /// Type of the current evaluation context.
        /// </summary>
        public CreditableType ContextType { get; set; }

        /// <summary>
        /// Optional day-level calendar context, such as RosterDay.
        /// </summary>
        public object? CalendarDay { get; set; }

        /// <summary>
        /// Optional reference ID (e.g., pairing ID, crew ID).
        /// </summary>
        public string? ReferenceId { get; set; }

    }
}
