namespace RuleEngine.Domain.Entities;

public class Footprint
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateTime Middle => Start.AddMinutes((End - Start).TotalMinutes / 2);
    public double Duration => (End - Start).TotalMinutes;
}