using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class NamedPutInstanceInScope
    {
        [Fact]
        public void Ensure_Named_Scoped_Instance_Working()
        {
            using var container = new StashboxContainer()
                .Register<A>();

            var a1 = new A();
            var a2 = new A();
            var a3 = new A();

            {
                using var scope = container.BeginScope();



                scope.PutInstanceInScope(a1);
                scope.PutInstanceInScope(a2, name: "a");
                scope.PutInstanceInScope(a2);

                Assert.Same(a2, scope.Resolve<A>("a"));
            }

            {
                using var scope = container.BeginScope();

                scope.PutInstanceInScope(a1, name: "a1");
                scope.PutInstanceInScope(a2, name: "a2");
                scope.PutInstanceInScope(a2, name: "a3");

                Assert.Same(a2, scope.Resolve<A>("a2"));
            }
        }

        class A
        { }
    }
}
