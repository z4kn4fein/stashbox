using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests;

public class CanResolveTests
{
    [Fact]
    public void CanResolveTests_ExplicitService()
    {
        using var container = new StashboxContainer()
            .Register<IA, A>();

        Assert.True(container.CanResolve<IA>());
        Assert.False(container.CanResolve<A>());
    }

    [Fact]
    public void CanResolveTests_OpenGeneric()
    {
        using var container = new StashboxContainer()
            .Register(typeof(IB<>), typeof(B<>));

        Assert.False(container.CanResolve(typeof(IB<>)));
        Assert.True(container.CanResolve<IB<IA>>());
    }

    [Fact]
    public void CanResolveTests_ClosedGeneric()
    {
        using var container = new StashboxContainer()
            .Register(typeof(IB<IA>), typeof(B<IA>));

        Assert.True(container.CanResolve<IB<IA>>());
    }

    [Fact]
    public void CanResolveTests_Wrapper()
    {
        using var container = new StashboxContainer()
            .Register<IA, A>();

        Assert.True(container.CanResolve<Func<IA>>());
        Assert.True(container.CanResolve<Lazy<IA>>());
        Assert.True(container.CanResolve<Tuple<IA, object>>());
    }

    [Fact]
    public void CanResolveTests_Enumerable()
    {
        using var container = new StashboxContainer()
            .Register<IA, A>();

        Assert.True(container.CanResolve<IEnumerable<IA>>());
        Assert.True(container.CanResolve<IEnumerable<IB<IA>>>());
    }

    [Fact]
    public void CanResolveTests_ExplicitService_Scope()
    {
        using var scope = new StashboxContainer()
            .Register<IA, A>().BeginScope();

        Assert.True(scope.CanResolve<IA>());
        Assert.False(scope.CanResolve<A>());
    }

    [Fact]
    public void CanResolveTests_OpenGeneric_Scope()
    {
        using var scope = new StashboxContainer()
            .Register(typeof(IB<>), typeof(B<>)).BeginScope();

        Assert.False(scope.CanResolve(typeof(IB<>)));
        Assert.True(scope.CanResolve<IB<IA>>());
    }

    [Fact]
    public void CanResolveTests_ClosedGeneric_Scope()
    {
        using var scope = new StashboxContainer()
            .Register(typeof(IB<IA>), typeof(B<IA>)).BeginScope();

        Assert.True(scope.CanResolve<IB<IA>>());
    }

    [Fact]
    public void CanResolveTests_Wrapper_Scope()
    {
        using var scope = new StashboxContainer()
            .Register<IA, A>().BeginScope();

        Assert.True(scope.CanResolve<Func<IA>>());
        Assert.True(scope.CanResolve<Lazy<IA>>());
        Assert.True(scope.CanResolve<Tuple<IA, object>>());
    }

    [Fact]
    public void CanResolveTests_Enumerable_Scope()
    {
        using var scope = new StashboxContainer()
            .Register<IA, A>().BeginScope();

        Assert.True(scope.CanResolve<IEnumerable<IA>>());
        Assert.True(scope.CanResolve<IEnumerable<IB<IA>>>());
    }

    [Fact]
    public void CanResolveTests_Special_Types()
    {
        using var scope = new StashboxContainer();

        Assert.True(scope.CanResolve<IServiceProvider>());
        Assert.True(scope.CanResolve<IDependencyResolver>());
        Assert.True(scope.CanResolve<IRequestContext>());
    }
    
    [Fact]
    public void CanResolveTests_WithExceptionOverEmptyCollection()
    {
        var container = new StashboxContainer(c => c.WithExceptionOverEmptyCollection()).Register<C>();

        Assert.False(container.CanResolve<byte[]>());
        Assert.Null(container.ResolveOrDefault<C>());
        var ex = Assert.Throws<ResolutionFailedException>(() => container.Resolve<C>());
        Assert.Contains("Constructor Void .ctor(Byte[]) found with unresolvable parameter: (System.Byte[])payload.", ex.Message);
    }

    private interface IA;

    private class A : IA;

    private interface IB<T>;

    private class B<T> : IB<T>;

#pragma warning disable CS9113 // Parameter is unread.
    private class C(byte[] payload);
#pragma warning restore CS9113 // Parameter is unread.
}