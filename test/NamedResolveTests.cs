using Stashbox.Exceptions;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Stashbox.Tests;

public class NamedResolveTests
{
    [Fact]
    public void NamedResolveTests_Resolve()
    {
        using var container = new StashboxContainer()
            .Register<IA, A>("A")
            .Register<IA, B>("B");

        Assert.IsType<A>(container.Resolve<IA>("A"));
        Assert.IsType<B>(container.Resolve<IA>("B"));
    }

    [Fact]
    public void NamedResolveTests_Resolve_ImplType()
    {
        using var container = new StashboxContainer()
            .Register<A>("A")
            .Register<B>("B");

        Assert.IsType<A>(container.Resolve<A>("A"));
        Assert.IsType<B>(container.Resolve<B>("B"));
    }

    [Fact]
    public void NamedResolveTests_Resolve_Throw_On_Same_Name()
    {
        Assert.Throws<ServiceAlreadyRegisteredException>(
            () => new StashboxContainer(c => c.WithRegistrationBehavior(Configuration.Rules.RegistrationBehavior.ThrowException))
                .Register<IA, A>("A")
                .Register<IA, A>("A"));
    }

    [Fact]
    public void NamedResolveTests_Resolve_Throw_On_Same_Name_Equality_By_Value()
    {
        var key = new StringBuilder("A");
        Assert.Throws<ServiceAlreadyRegisteredException>(
            () => new StashboxContainer(c => c.WithRegistrationBehavior(Configuration.Rules.RegistrationBehavior.ThrowException))
                .Register<IA, A>("A")
                .Register<IA, A>(key.ToString()));
    }

    [Fact]
    public void NamedResolveTests_Ensure_Delegate_Cache_Works_OnRoot()
    {
        using var container = new StashboxContainer()
            .Register<IA, A>("A");

        container.Resolve<IA>("A");
        container.Resolve<IA>("A");

        var cache = container.ContainerContext.RootScope.GetDelegateCacheEntries();
        Assert.Single(cache);
    }

    [Fact]
    public void NamedResolveTests_Ensure_Delegate_Cache_Works_By_Value_OnRoot()
    {
        var key = new StringBuilder("A");
        using var container = new StashboxContainer()
            .Register<IA, A>("A");

        container.Resolve<IA>("A");
        container.Resolve<IA>(key.ToString());

        var cache = container.ContainerContext.RootScope.GetDelegateCacheEntries();
        Assert.Single(cache);
    }

    [Fact]
    public void NamedResolveTests_Ensure_Delegate_Cache_Shared_Between_UnNamed_Scopes()
    {
        var key = new StringBuilder("A");
        using var container = new StashboxContainer();

        var scope1 = container.BeginScope();
        var scope2 = container.BeginScope();
        var scope3 = scope2.BeginScope();

        var scopeType = scope1.GetType();
        var delegateCache1 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope1);
        var delegateCache2 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope2);
        var delegateCache3 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope3);

        Assert.Same(delegateCache1, delegateCache2);
        Assert.Same(delegateCache2, delegateCache3);
    }

    [Fact]
    public void NamedResolveTests_Ensure_Delegate_Cache_Not_Shared_Between_Root_And_UnNamed_Scopes()
    {
        var key = new StringBuilder("A");
        using var container = new StashboxContainer();

        var scope = container.BeginScope();

        var scopeType = scope.GetType();
        var delegateCache1 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope);
        var delegateCache2 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(container.ContainerContext.RootScope);

        Assert.NotSame(delegateCache1, delegateCache2);
    }

    [Fact]
    public void NamedResolveTests_Named_Scope_Cache_Works()
    {
        using var container = new StashboxContainer()
            .Register<A>()
            .Register<B>();

        var scope1 = container.BeginScope("A");
        var scope2 = container.BeginScope("A");

        scope1.Resolve<A>();
        scope2.Resolve<B>();

        var rootCache = container.ContainerContext.RootScope.GetDelegateCacheEntries();
        Assert.Empty(rootCache);

        var scope1Cache = scope1.GetDelegateCacheEntries();
        Assert.Equal(2, scope1Cache.Count());

        var scope2Cache = scope2.GetDelegateCacheEntries();
        Assert.Equal(2, scope2Cache.Count());
    }

    [Fact]
    public void NamedResolveTests_Named_Scope_Cache_Works_Equality_By_Value()
    {
        var key = new StringBuilder("A");
        using var container = new StashboxContainer()
            .Register<A>()
            .Register<B>();

        var scope1 = container.BeginScope("A");
        var scope2 = container.BeginScope("A");

        scope1.Resolve<A>();
        scope2.Resolve<B>();

        var rootCache = container.ContainerContext.RootScope.GetDelegateCacheEntries();
        Assert.Empty(rootCache);

        var scope1Cache = scope1.GetDelegateCacheEntries();
        Assert.Equal(2, scope1Cache.Count());

        var scope2Cache = scope2.GetDelegateCacheEntries();
        Assert.Equal(2, scope2Cache.Count());
    }

    interface IA { }

    class A : IA { }

    class B : IA { }

    class C : IA { }

    class D : IA { }
}