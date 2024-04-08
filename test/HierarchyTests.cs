using Xunit;

namespace Stashbox.Tests;

public class HierarchyTests
{
    [Fact]
    public void NamedScope_Hierarchy_Respected()
    {
        var container = new StashboxContainer(c => c
                .WithAutoMemberInjection())
            .Register<ITest, Test>(c => c.DefinesScope("root").WithName("root"))
            .Register<ITest, Test1>(c => c.InNamedScope("root").DefinesScope("A"))
            .Register<ITest, Test2>(c => c.InNamedScope("root").DefinesScope("B"))
            .Register<ITest, Test3>(c => c.InNamedScope("A"))
            .Register<ITest, Test4>(c => c.InNamedScope("A"))
            .Register<ITest, Test5>(c => c.InNamedScope("B"))
            .Register<ITest, Test6>(c => c.InNamedScope("B"));

        var r = container.Resolve<ITest>("root");

        Assert.IsType<Test1>(((Test)r).Subs[0]);
        Assert.IsType<Test2>(((Test)r).Subs[1]);
        Assert.IsType<Test3>(((Test1)((Test)r).Subs[0]).Subs[0]);
        Assert.IsType<Test4>(((Test1)((Test)r).Subs[0]).Subs[1]);
        Assert.IsType<Test5>(((Test2)((Test)r).Subs[1]).Subs[0]);
        Assert.IsType<Test6>(((Test2)((Test)r).Subs[1]).Subs[1]);
    }

    [Fact]
    public void NamedScope_Hierarchy_Respected_WithoutNames()
    {
        var container = new StashboxContainer(c => c
                .WithAutoMemberInjection())
            .Register<ITest, Test>(c => c.DefinesScope().WithName("root"))
            .Register<ITest, Test1>(c => c.InScopeDefinedBy<Test>().DefinesScope())
            .Register<ITest, Test2>(c => c.InScopeDefinedBy<Test>().DefinesScope())
            .Register<ITest, Test3>(c => c.InScopeDefinedBy(typeof(Test1)))
            .Register<ITest, Test4>(c => c.InScopeDefinedBy(typeof(Test1)))
            .Register<ITest, Test5>(c => c.InScopeDefinedBy<Test2>())
            .Register<ITest, Test6>(c => c.InScopeDefinedBy<Test2>());

        var r = container.Resolve<ITest>("root");

        Assert.IsType<Test1>(((Test)r).Subs[0]);
        Assert.IsType<Test2>(((Test)r).Subs[1]);
        Assert.IsType<Test3>(((Test1)((Test)r).Subs[0]).Subs[0]);
        Assert.IsType<Test4>(((Test1)((Test)r).Subs[0]).Subs[1]);
        Assert.IsType<Test5>(((Test2)((Test)r).Subs[1]).Subs[0]);
        Assert.IsType<Test6>(((Test2)((Test)r).Subs[1]).Subs[1]);
    }

    [Fact]
    public void Conditional_Hierarchy_Respected()
    {
        var container = new StashboxContainer(c => c
                .WithAutoMemberInjection())
            .Register<R>(c => c.WithName("root"))
            .Register<ITest, Test1>(c => c.WhenDependantIs<R>())
            .Register<ITest, Test2>(c => c.WhenDependantIs<R>())
            .Register<ITest, Test3>(c => c.WhenDependantIs(typeof(Test1)))
            .Register<ITest, Test4>(c => c.WhenDependantIs(typeof(Test1)))
            .Register<ITest, Test5>(c => c.WhenDependantIs<Test2>())
            .Register<ITest, Test6>(c => c.WhenDependantIs<Test2>());

        var r = container.Resolve<R>("root");

        Assert.IsType<Test1>(r.Subs[0]);
        Assert.IsType<Test2>(r.Subs[1]);
        Assert.IsType<Test3>(((Test1)r.Subs[0]).Subs[0]);
        Assert.IsType<Test4>(((Test1)r.Subs[0]).Subs[1]);
        Assert.IsType<Test5>(((Test2)r.Subs[1]).Subs[0]);
        Assert.IsType<Test6>(((Test2)r.Subs[1]).Subs[1]);
    }

    interface ITest;

    class R
    {
        public ITest[] Subs { get; set; }
    }

    class Test : ITest
    {
        public ITest[] Subs { get; set; }
    }

    class Test1 : ITest
    {
        public ITest[] Subs { get; set; }
    }

    class Test2 : ITest
    {
        public ITest[] Subs { get; set; }
    }

    class Test3 : ITest;

    class Test4 : ITest;

    class Test5 : ITest;

    class Test6 : ITest;
}