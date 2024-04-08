using Xunit;

namespace Stashbox.Tests.IssueTests;

public class ChainedNamedScopesAreNotWorkingProperly
{
    [Fact]
    public void Chained_named_scopes_are_not_working_properly()
    {
        var container = new StashboxContainer()
            .Register<NamedScopeTest2>(config => config.DefinesScope("B").InNamedScope("A"))
            .Register<NamedScopeTest1>(config => config.InNamedScope("B"))
            .Register<NamedScopeTest3>(config => config.DefinesScope("A"));

        Assert.NotNull(container.Resolve<NamedScopeTest3>());
    }

    class NamedScopeTest1;

    class NamedScopeTest2
    {
        public NamedScopeTest2(NamedScopeTest1 t)
        { }
    }

    class NamedScopeTest3
    {
        public NamedScopeTest3(NamedScopeTest2 t)
        { }
    }
}