namespace RuleEngine.Domain.Entities;

public class Activity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Location { get; set; }
    public string Type { get; set; }
    public bool IsDeadhead { get; set; }
}
