namespace RuleEngine.Domain.CrewManagement.Entities;
public abstract class Creditable
{
    public List<CounterValue> CounterValues { get; set; } = new();

    public void AddCounter(string type, int value)
    {
        CounterValues.Add(new CounterValue(type, value));
    }

    public void RemoveCounter(string type)
    {
        CounterValues = CounterValues.Where(c => c.CounterTypeSystemName != type).ToList();
    }
}
