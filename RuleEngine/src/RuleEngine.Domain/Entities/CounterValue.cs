
namespace RuleEngine.Domain.Entities
{
    public class CounterValue
    {
        public string CounterTypeSystemName { get; set; }
        public int CounterValue_ { get; set; }
        public CounterValue(string counterTypeSystemName, int counterValue)
        {
            CounterTypeSystemName = counterTypeSystemName;
            CounterValue_ = counterValue;
        }

        public CounterValue() { }
    }

}
