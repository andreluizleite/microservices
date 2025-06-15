using RuleEngine.Application.DTOs;
using RuleEngine.Application.Enums;
using RuleEngine.Application.Services;
using RuleEngine.Domain.Core.Enums;
using Xunit;

namespace RuleEngine.Application.Tests
{
    public class RulePersistenceTests
    {
        [Fact]
        public async Task Should_Save_And_Load_RuleTreeDto_From_File()
        {
            var service = new InMemoryRulePersistenceService();
            var id = "test-rule";

            var dto = new RuleTreeDto
            {
                Name = "Root",
                Type = RuleTreeNodeType.Group,
                Operator = LogicalOperator.And,
                Children = new List<RuleTreeDto>
                {
                    new RuleTreeDto
                    {
                        Name = "AgeCheck",
                        Type = RuleTreeNodeType.Rule,
                        Expression = "Age >= 18"
                    }
                }
            };

            await service.SaveAsync(id, dto);
            var loaded = await service.LoadAsync(id);

            Assert.NotNull(loaded);
            Assert.Equal(dto.Name, loaded!.Name);
            Assert.Single(loaded.Children);
            Assert.Equal("AgeCheck", loaded.Children[0].Name);
        }
    }
}
