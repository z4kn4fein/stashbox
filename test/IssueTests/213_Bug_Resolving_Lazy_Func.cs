using System;
using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class BugResolvingLazyFunc
{
    [Fact]
    public void ResolvingLazyWrapper_ShouldNotInstantiateService_Singleton()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton<Service>();

        var lazy = container.Resolve<Lazy<Service>>();
        Assert.Throws<InvalidOperationException>(() => lazy.Value);
    }
    
    [Fact]
    public void ResolvingFuncWrapper_ShouldNotInstantiateService_Singleton()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton<Service>();

        var func = container.Resolve<Func<Service>>();
        Assert.Throws<InvalidOperationException>(() => func());
    }
    
    [Fact]
    public void ResolvingLazyWrapper_ShouldNotInstantiateService_Scoped()
    {
        using var container = new StashboxContainer();
        container.RegisterScoped<Service>();

        var lazy = container.Resolve<Lazy<Service>>();
        Assert.Throws<InvalidOperationException>(() => lazy.Value);
        
        var scope = container.BeginScope();
        
        lazy = scope.Resolve<Lazy<Service>>();
        Assert.Throws<InvalidOperationException>(() => lazy.Value);
    }
    
    [Fact]
    public void ResolvingFuncWrapper_ShouldNotInstantiateService_Scoped()
    {
        using var container = new StashboxContainer();
        container.RegisterScoped<Service>();

        var func = container.Resolve<Func<Service>>();
        Assert.Throws<InvalidOperationException>(() => func());
        
        var scope = container.BeginScope();
        
        func = scope.Resolve<Func<Service>>();
        Assert.Throws<InvalidOperationException>(() => func());
    }

    private class Service
    {
        public Service()
        {
            throw new InvalidOperationException("Don't instantiate me yet!");
        }
    }
}