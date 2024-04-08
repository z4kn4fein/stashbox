using Xunit;

namespace Stashbox.Tests.IssueTests;

public class GenerateOneInstanceForMultipleInterfaces
{
    [Fact]
    public void Generate_one_instance_for_multiple_interfaces()
    {
        var container = new StashboxContainer();
        container.Register<Test2>(context => context.AsImplementedTypes());
        var inst1 = container.Resolve<ITest>();
        var inst2 = container.Resolve<ITest1>();
        var inst3 = container.Resolve<ITest2>();
        var inst4 = container.Resolve<Test>();
        var inst5 = container.Resolve<Test2>();

        Assert.NotSame(inst1, inst2);
        Assert.NotSame(inst2, inst3);
        Assert.NotSame(inst3, inst4);
        Assert.NotSame(inst4, inst5);
    }

    [Fact]
    public void Generate_one_instance_for_multiple_interfaces_singleton()
    {
        var container = new StashboxContainer();
        container.Register<Test2>(context => context.AsImplementedTypes().WithSingletonLifetime());
        var inst1 = container.Resolve<ITest>();
        var inst2 = container.Resolve<ITest1>();
        var inst3 = container.Resolve<ITest2>();
        var inst4 = container.Resolve<Test>();
        var inst5 = container.Resolve<Test2>();

        Assert.Same(inst1, inst2);
        Assert.Same(inst2, inst3);
        Assert.Same(inst3, inst4);
        Assert.Same(inst4, inst5);
    }

    [Fact]
    public void Generate_one_instance_for_multiple_interfaces_scoped()
    {
        var container = new StashboxContainer();
        container.Register<Test2>(context => context.AsImplementedTypes().WithScopedLifetime());

        var scope = container.BeginScope();
        var inst1 = scope.Resolve<ITest>();
        var inst2 = scope.Resolve<ITest1>();
        var inst3 = scope.Resolve<ITest2>();
        var inst4 = scope.Resolve<Test>();
        var inst5 = scope.Resolve<Test2>();

        Assert.Same(inst1, inst2);
        Assert.Same(inst2, inst3);
        Assert.Same(inst3, inst4);
        Assert.Same(inst4, inst5);
    }

    [Fact]
    public void Generate_one_instance_for_multiple_interfaces_named_scope()
    {
        var container = new StashboxContainer();
        container.Register<Test2>(context => context.AsImplementedTypes().InNamedScope("A"));

        var scope = container.BeginScope("A");
        var inst1 = scope.Resolve<ITest>();
        var inst2 = scope.Resolve<ITest1>();
        var inst3 = scope.Resolve<ITest2>();
        var inst4 = scope.Resolve<Test>();
        var inst5 = scope.Resolve<Test2>();

        Assert.Same(inst1, inst2);
        Assert.Same(inst2, inst3);
        Assert.Same(inst3, inst4);
        Assert.Same(inst4, inst5);
    }

    interface ITest;

    interface ITest1;

    interface ITest2;

    class Test;

    class Test2 : Test, ITest, ITest1, ITest2;
}