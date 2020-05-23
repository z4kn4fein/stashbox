using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class ExpectedOverrideBehaviourNotWorkingWithScopes
    {
        [Fact]
        public void RegisteredInstancesCanBeOverridenViaAFactory()
        {
            var container = new StashboxContainer();

            var toInclude = new A { Id = 20 };
            container.RegisterInstance(toInclude);

            var outer = container.Resolve<A>();
            A inner1 = null;
            A inner2 = null;
            using (var scope = container.BeginScope())
            {
                inner1 = scope.Resolve<A>();
                var toOverride = new A { Id = 30 };
                scope.PutInstanceInScope(toOverride);
                inner2 = scope.Resolve<A>();
            }

            Assert.Equal(toInclude.Id, outer.Id);
            Assert.Equal(toInclude.Id, inner1.Id);
            Assert.Equal(30, inner2.Id);
        }
    }

    class A
    {
        public int Id { get; set; }
    }
}
