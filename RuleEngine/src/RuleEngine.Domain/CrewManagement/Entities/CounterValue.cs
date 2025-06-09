namespace RuleEngine.Domain.CrewManagement.Entities
{
    public class CounterValue
    {
        public string CounterTypeSystemName { get; set; }
        public decimal CounterValue_ { get; set; }
        public CounterValue(string counterTypeSystemName, decimal counterValue)
        {
            CounterTypeSystemName = counterTypeSystemName;
            CounterValue_ = counterValue;
        }

        public CounterValue() { }
    }

}
