using RuleEngine.Application.Enums;
using RuleEngine.Domain.Core.Enums;

namespace RuleEngine.Application.DTOs
{
    public class RuleTreeDto
    {
        public RuleTreeNodeType Type { get; set; } = RuleTreeNodeType.Rule;
        public string Name { get; set; } = string.Empty;

        // For Rule
        public string? Expression { get; set; }

        // For Group
        public LogicalOperator? Operator { get; set; }
        public List<RuleTreeDto>? Children { get; set; }
    }
}
