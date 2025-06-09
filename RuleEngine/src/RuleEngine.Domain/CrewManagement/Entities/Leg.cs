using RuleEngine.Domain.Core.Interfaces;

namespace RuleEngine.Domain.CrewManagement.Entities
{
    public class Leg : Assignment, ICounterTrackable
    {
        public int FlightNumber { get; set; }

        public List<CounterValue> CounterValues { get; set; } = new();
    }
}
