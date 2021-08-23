using Stashbox.Exceptions;
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

        [Fact]
        public void Ensure_Override_Doesnt_Trigger_Unknown()
        {
            var depOverride = new Dep2();
            using var container = new StashboxContainer(c => c.WithUnknownTypeResolution(ctx =>
                {
                    ctx.Skip();
                }))
                .Register<Test>();

            var t = container.Resolve<Test>(dependencyOverrides: new object[] { depOverride });
            Assert.Same(depOverride, t.Dep);
        }

        [Fact]
        public void Ensure_Skipping_Doesnt_Let_Uknown_Type_Be_Registered()
        {
            var depOverride = new Dep2();
            using var container = new StashboxContainer(c => c.WithUnknownTypeResolution(ctx =>
            {
                if (ctx.ServiceType == typeof(IDep))
                    ctx.Skip();
            }))
            .Register<Test>()
            .Register<Test1>();

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<Test>());
            Assert.NotNull(container.Resolve<Test1>());
        }

        class Test
        {
            public IDep Dep { get; set; }

            public Test(IDep dep)
            {
                this.Dep = dep;
            }
        }

        class Test1
        {
            public Dep1 Dep { get; set; }

            public Test1(Dep1 dep)
            {
                this.Dep = dep;
            }
        }

        interface IDep { }

        class Dep1 : IDep { }

        class Dep2 : IDep { }
    }
}
