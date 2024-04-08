using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class NamedResolutioUsingResolveAllReturnsAllNamedAndUnnameInstanсes
{
    [Fact]
    public void Ensure_Named_ResolveAll_Works()
    {
        using var container = new StashboxContainer()
            .Register<ITest, Test1>("test1")
            .Register<ITest, Test1>("test2")
            .Register<ITest, Test1>();

        var inst = container.ResolveAll<ITest>("test1").ToArray();
        var nameLess = container.ResolveAll<ITest>().ToArray();

        Assert.Single(inst);
        Assert.Equal(3, nameLess.Length);
    }

    [Fact]
    public void Ensure_Named_ResolveAll_Works_Injection()
    {
        using var container = new StashboxContainer()
            .Register<ITest, Test1>("test1")
            .Register<ITest, Test1>("test2")
            .Register<ITest, Test1>()
            .Register<Test2>(c => c.WithDependencyBinding<IEnumerable<ITest>>("test1"))
            .Register<Test2>("t2");

        var inst = container.Resolve<Test2>();
        var nameLess = container.Resolve<Test2>("t2");

        Assert.Single(inst.Tests);
        Assert.Equal(3, nameLess.Tests.Count());
    }

    [Fact]
    public void Ensure_Named_ResolveAll_Works_Injection_Convention()
    {
        using var container = new StashboxContainer(c => c.TreatParameterAndMemberNameAsDependencyName())
            .Register<ITest, Test1>("test1")
            .Register<ITest, Test1>("test2")
            .Register<ITest, Test1>()
            .Register<Test2>();

        var inst = container.Resolve<Test2>();

        Assert.Single(inst.Tests);
    }

    interface ITest;

    class Test1 : ITest;

    class Test2
    {
        public Test2(IEnumerable<ITest> test1)
        {
            this.Tests = test1;
        }

        public IEnumerable<ITest> Tests { get; }
    }
}