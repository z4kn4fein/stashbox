using System;
using Xunit;

namespace Stashbox.Tests;

public class WithDynamicResolutionTests
{
    [Fact]
    public void WithDynamicResolutionTests_TopRequest()
    {
        using var container = new StashboxContainer()
            .Register<A>(c => c.WithDynamicResolution());

        Assert.NotNull(container.Resolve<A>());
    }

    [Fact]
    public void WithDynamicResolutionTests_Dependency()
    {
        using var container = new StashboxContainer()
            .Register<A>(c => c.WithDynamicResolution())
            .Register<B>(c => c.WithDependencyBinding("A"));

        Assert.NotNull(container.Resolve<B>().A);
    }

    [Fact]
    public void WithDynamicResolutionTests_Dependency_Override()
    {
        using var container = new StashboxContainer()
            .Register<B>(c => c.WithDependencyBinding("A").WithDynamicResolution())
            .Register<C>(c => c.WithDependencyBinding("B"));

        var @override = new A();
        var inst = container.Resolve<C>([@override]);

        Assert.Same(@override, inst.B.A);
    }

    [Fact]
    public void WithDynamicResolutionTests_Circle()
    {
        using var container = new StashboxContainer()
            .Register<Circle1>(c => c.WithDependencyBinding("Circle2").WithDynamicResolution())
            .Register<Circle2>(c => c.WithDependencyBinding("Circle1").WithDynamicResolution());

        var circle1 = container.Resolve<Circle1>();
        var circle2 = container.Resolve<Circle2>();

        Assert.NotNull(circle1.Circle2.Value);
        Assert.NotNull(circle1.Circle2.Value.Circle1);
        Assert.NotNull(circle2.Circle1.Value);
        Assert.NotNull(circle2.Circle1.Value.Circle2);
    }

    class A;

    class B
    {
        public A A { get; set; }
    }

    class C
    {
        public B B { get; set; }
    }

    class Circle1
    {
        public Lazy<Circle2> Circle2 { get; set; }
    }

    class Circle2
    {
        public Lazy<Circle1> Circle1 { get; set; }
    }
}