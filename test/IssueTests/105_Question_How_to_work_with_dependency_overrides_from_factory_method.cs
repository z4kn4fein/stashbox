using Stashbox.Resolution;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class QuestionHowTWorkWithDependencyOverridesFromFactoryMethod
    {
        [Fact]
        public void Ensure_Context_Available_In_Factory()
        {
            var depOverride = new Dep2();
            using var container = new StashboxContainer()
                .Register<Test>(c => c.WithFactory<IResolutionContext>(ctx =>
                {
                    var d = ctx.GetDependencyOverrideOrDefault<Dep2>();
                    Assert.Same(depOverride, d);
                    return new Test(d);
                }))
                .Register<Dep1>();

            var t = container.Resolve<Test>(dependencyOverrides: new object[] { depOverride });
            Assert.Same(depOverride, t.Dep);
        }

        class Test
        {
            public IDep Dep { get; set; }

            public Test(IDep dep)
            {
                this.Dep = dep;
            }
        }

        interface IDep { }

        class Dep1 : IDep { }

        class Dep2 : IDep { }
    }
}
