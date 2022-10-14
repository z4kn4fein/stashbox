using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class GenericResolutionIssue
    {
        [Fact]
        public void Ensure_generic_resolution_works()
        {
            using var container = new StashboxContainer();
            container.Register<IA<C>, AT>();
            container.Register<IA<C, long>, AT>();
            container.Register<B>();

            var inst = container.Resolve<B>();

            Assert.NotNull(inst);
        }

        class C { }

        class AT : IA<C>, IA<C, long> { }

        interface IA<T> { }

        interface IA<T, R> { }

        class B
        {
            public B(IA<C> c)
            {

            }
        }
    }
}
