using Stashbox.Resolution;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class QuestionHowTWorkWithDependencyOverridesFromFactoryMethod
    {
        [Fact]
        public void Ensure_Context_Available_In_Factory()
        {
            using var container = new StashboxContainer()
                .Register<Test>(c => c.WithFactory<IResolutionContext>(ctx =>
                {
                    var d = ctx.GetDependencyOverrideOrDefault<Dep2>();
                    Assert.NotNull(d);
                    return new Test(d);
                }))
                .Register<Dep1>();

            var t = container.Resolve<Test>(dependencyOverrides: new object[] { new Dep2() });
            Assert.IsType<Dep2>(t.Dep);
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
