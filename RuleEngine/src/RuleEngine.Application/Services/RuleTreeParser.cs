using RuleEngine.Application.DTOs;
using RuleEngine.Application.Enums;
using RuleEngine.Domain.Core.Enums;
using RuleEngine.Domain.Core.Interfaces;
using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Application.Services
{
    public static class RuleTreeParser<T>
    {
        public static IRuleNode<T> Parse(RuleTreeDto dto)
        {
            switch (dto.Type)
            {
                case RuleTreeNodeType.Rule:
                    if (string.IsNullOrWhiteSpace(dto.Expression))
                        throw new ArgumentException("Expression is required for rule nodes");
                    return Rule<T>.FromExpression(dto.Name, dto.Expression);

                case RuleTreeNodeType.Group:
                    if (dto.Children == null || dto.Children.Count == 0)
                        throw new ArgumentException("Group node must contain children");

                    var nodes = dto.Children.Select(Parse).ToArray();
                    return dto.Operator switch
                    {
                        LogicalOperator.And => RuleGroup<T>.And(dto.Name, nodes),
                        LogicalOperator.Or => RuleGroup<T>.Or(dto.Name, nodes),
                        LogicalOperator.Not when nodes.Length == 1 => RuleGroup<T>.Not(dto.Name, nodes[0]),
                        _ => throw new ArgumentException($"Invalid or unsupported operator: {dto.Operator}")
                    };

                default:
                    throw new ArgumentException($"Unknown rule type: {dto.Type}");
            }
        }
    }
}
