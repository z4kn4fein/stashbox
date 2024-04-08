using Stashbox.Attributes;
using Stashbox.Utils;
using Xunit;

namespace Stashbox.Tests;

public class WireUpTests
{
    [Fact]
    public void WireUp_Multiple()
    {
        using var container = new StashboxContainer();
        var test1 = new Test1();
        container.WireUp<ITest1>(test1);

        var test2 = new Test();
        container.WireUp<ITest>(test2);

        var inst = container.Resolve<ITest1>();
        var inst2 = container.Resolve<ITest>();

        Assert.Same(test1, inst);
        Assert.Same(test2, inst2);
    }

    [Fact]
    public void WireUp_Multiple_Named()
    {
        using var container = new StashboxContainer();
        var test1 = new Test();
        container.WireUp<ITest>(test1, "test1");

        var test2 = new Test();
        container.WireUp<ITest>(test2, "test2");

        var inst = container.Resolve<ITest>("test1");
        var inst2 = container.Resolve<ITest>("test2");

        Assert.Same(test1, inst);
        Assert.Same(test2, inst2);
    }

    [Fact]
    public void WireUpTests_InjectionMember()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton<ITest, Test>();

        var test1 = new Test1();
        container.WireUp<ITest1>(test1);

        container.Register<Test2>();

        var inst = container.Resolve<Test2>();

        Assert.NotNull(inst);
        Assert.NotNull(inst.Test1);
        Assert.IsType<Test2>(inst);
        Assert.IsType<Test1>(inst.Test1);
        Assert.IsType<Test>(inst.Test1.Test);
    }

    [Fact]
    public void WireUpTests_InjectionMember_ServiceUpdated()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton<ITest, Test>();

        var test1 = new Test1();
        container.WireUp<ITest1>(test1);

        container.ReMap<ITest, Test>(c => c.WithSingletonLifetime());

        container.Register<Test2>();

        var inst = container.Resolve<Test2>();

        Assert.NotNull(inst);
        Assert.NotNull(inst.Test1);
        Assert.IsType<Test2>(inst);
        Assert.IsType<Test1>(inst.Test1);
        Assert.IsType<Test>(inst.Test1.Test);
    }

    [Fact]
    public void WireUpTests_InjectionMember_WithoutService()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton<ITest, Test>();
        var test1 = new Test1();
        container.WireUp(test1);
        var inst = container.Resolve<Test1>();

        Assert.NotNull(inst);
        Assert.NotNull(inst.Test);
        Assert.IsType<Test1>(inst);
        Assert.IsType<Test>(inst.Test);
    }

    [Fact]
    public void WireUpTests_WithoutService_NonGeneric()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton<ITest, Test>();
        object test1 = new Test1();
        container.WireUp(test1, typeof(Test1));
        var inst = container.Resolve<Test1>();

        Assert.NotNull(inst);
        Assert.NotNull(inst.Test);
        Assert.NotNull(inst.test);
        Assert.IsType<Test1>(inst);
        Assert.IsType<Test>(inst.Test);
        Assert.IsType<Test>(inst.test);
    }

    interface ITest;

    interface ITest1 { ITest Test { get; } }

    class Test : ITest;

    class Test1 : ITest1
    {
        [Dependency]
#pragma warning disable 649
        public ITest test;
#pragma warning restore 649

        [Dependency]
        public ITest Test { get; set; }

        [InjectionMethod]
        public void Init()
        {
            Shield.EnsureNotNull(Test, nameof(Test));
        }
    }

    class Test2
    {
        public ITest1 Test1 { get; set; }

        public Test2(ITest1 test1)
        {
            this.Test1 = test1;
        }
    }
}