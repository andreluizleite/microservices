using RuleEngine.Domain.CrewManagement.Entities;
using System.Collections.Generic;

namespace RuleEngine.Domain.Core.Interfaces
{
    public interface ICounterTrackable
    {
        List<CounterValue> CounterValues { get; set; }
    }
}
