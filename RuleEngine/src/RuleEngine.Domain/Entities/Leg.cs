
namespace RuleEngine.Domain.Entities
{
    public class Leg : Assignment
    {
        public int FlightNumber { get; set; }
        public List<CounterValue> CounterValues { get; set; } = new();
    }
}
