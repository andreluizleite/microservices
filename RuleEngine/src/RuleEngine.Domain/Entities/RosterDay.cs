namespace RuleEngine.Domain.Entities;

public class RosterDay
{
    public DateTime Date { get; set; }
    public List<Activity> Activities { get; set; } = new();
    public Footprint ActualFootprint { get; set; } = new();
    public List<CounterValue> CounterValues { get; set; } = new();
}
