namespace RuleEngine.Domain.Core.Contexts
{
    public class DynamicEvaluationContext
    {
        public Dictionary<string, object> Data { get; }

        public DynamicEvaluationContext(Dictionary<string, object> data)
        {
            Data = data;
        }

        public object? this[string key] => Data.ContainsKey(key) ? Data[key] : null;
    }
}
