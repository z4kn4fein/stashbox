using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class WithUnknownTypeResolutionBreaksConstructorSelectionRules
    {
        [Fact]
        public void WithUnknownTypeResolution_breaks_constructor_selection_rules()
        {
            var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            var inst = container
                .Register<Test>()
                .Register<Dep1>()
                .Resolve<Test>();

            Assert.NotNull(inst.Dep1);
            Assert.Null(inst.Dep);
        }

        [Fact]
        public void WithUnknownTypeResolution_breaks_constructor_selection_rules_ensure_ok_without_unknown_type_resolver()
        {
            var container = new StashboxContainer();
            var inst = container
                .Register<Test>()
                .Register<Dep1>()
                .Resolve<Test>();

            Assert.NotNull(inst.Dep1);
            Assert.Null(inst.Dep);
        }

        class Dep { }

        class Dep1 { }

        class Test
        {
            public Dep Dep { get; }
            public Dep1 Dep1 { get; }
            public Test(Dep dep) { Dep = dep; }
            public Test(Dep1 dep) { Dep1 = dep; }
        }
    }
}
