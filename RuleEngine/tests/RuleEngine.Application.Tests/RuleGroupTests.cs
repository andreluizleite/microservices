using RuleEngine.Domain.Core.Rules;

namespace RuleEngine.Tests
{
    public class RuleGroupTests
    {
        public class DummyContext
        {
            public int Value { get; set; }
        }

        [Fact]
        public void Should_Evaluate_AND_Group_Correctly()
        {
            var rule1 = Rule<DummyContext>.Create("GreaterThan5", x => x.Value > 5);
            var rule2 = Rule<DummyContext>.Create("LessThan10", x => x.Value < 10);
            var group = RuleGroup<DummyContext>.And("Group1", rule1, rule2);

            var context = new DummyContext { Value = 7 };
            Assert.True(group.Evaluate(context));
        }

        [Fact]
        public void Should_Evaluate_OR_Group_Correctly()
        {
            var rule1 = Rule<DummyContext>.Create("LessThan3", x => x.Value < 3);
            var rule2 = Rule<DummyContext>.Create("GreaterThan10", x => x.Value > 10);
            var group = RuleGroup<DummyContext>.Or("Group2", rule1, rule2);

            var context = new DummyContext { Value = 12 };
            Assert.True(group.Evaluate(context));
        }

        [Fact]
        public void Should_Evaluate_NOT_Group_Correctly()
        {
            var rule = Rule<DummyContext>.Create("Equals5", x => x.Value == 5);
            var notGroup = RuleGroup<DummyContext>.Not("Not5", rule);

            var context = new DummyContext { Value = 6 };
            Assert.True(notGroup.Evaluate(context));
        }
    }
}