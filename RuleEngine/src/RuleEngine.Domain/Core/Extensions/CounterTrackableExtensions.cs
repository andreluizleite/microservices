using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.CrewManagement.Entities;

namespace RuleEngine.Domain.Core.Extensions
{
    public static class CounterTrackableExtensions
    {
        public static void AddCounter(this ICounterTrackable target, string counterType, int value)
        {
            if (target == null || string.IsNullOrWhiteSpace(counterType))
                return;

            target.CounterValues.Add(new CounterValue
            {
                CounterTypeSystemName = counterType,
                CounterValue_ = value
            });
        }
    }
}
