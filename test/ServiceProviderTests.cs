using System;
using Xunit;

namespace Stashbox.Tests;

public class ServiceProviderTests
{
    [Fact]
    public void ServiceProviderTests_Resolve_Self()
    {
        using IStashboxContainer container = new StashboxContainer();
        
        Assert.Same(container.ContainerContext.RootScope, container.Resolve<IServiceProvider>());

        using var scope = container.BeginScope();
        
        Assert.Same(scope, scope.Resolve<IServiceProvider>());
    }
    
    [Fact]
    public void ServiceProviderTests_Resolve_Override()
    {
        using var container = new StashboxContainer()
            .Register<IServiceProvider, CustomSp>(c => c.WithFactory(dr => new CustomSp(dr)).AsServiceAlso<ITest>().AsServiceAlso<ITest2>())
            .Register<SpAware>();

        Assert.IsType<CustomSp>(container.Resolve<IServiceProvider>());
        Assert.Same(container.ContainerContext.RootScope, ((CustomSp)container.Resolve<IServiceProvider>()).DependencyResolver);

        using var scope = container.BeginScope();
        
        Assert.IsType<CustomSp>(scope.Resolve<IServiceProvider>());
        Assert.Same(scope, ((CustomSp)scope.Resolve<IServiceProvider>()).DependencyResolver);

        Assert.IsType<CustomSp>(container.Resolve<SpAware>().Test);
    }
    
    [Fact]
    public void ServiceProviderTests_Resolve_MultiReg()
    {
        using var container = new StashboxContainer(c => c.WithDisposableTransientTracking())
            .Register<IServiceProvider, CustomSp>(c => c.WithFactory(dr => new CustomSp(dr)).AsServiceAlso<ITest>().AsServiceAlso<ITest2>())
            .Register<SpAware>();

        Assert.IsType<CustomSp>(container.Resolve<SpAware>().Test);
        Assert.IsType<CustomSp>(container.Resolve<SpAware>().Test2);
    }
    
    [Fact]
    public void ServiceProviderTests_Resolve_MultiReg_AllImplemented()
    {
        using var container = new StashboxContainer(c => c.WithDisposableTransientTracking())
            .Register<IServiceProvider, CustomSp>(c => c.WithFactory(dr => new CustomSp(dr)).AsImplementedTypes())
            .Register<SpAware>();

        Assert.IsType<CustomSp>(container.Resolve<SpAware>().Test);
        Assert.IsType<CustomSp>(container.Resolve<SpAware>().Test2);
    }

    interface ITest;
    interface ITest2;
    
    class CustomSp : IServiceProvider, ITest, ITest2, IDisposable
    {
        public IDependencyResolver DependencyResolver { get; }

        public CustomSp(IDependencyResolver dependencyResolver)
        {
            DependencyResolver = dependencyResolver;
        }
        
        public object GetService(Type serviceType)
        {
            return null;
        }
        
        public void Dispose() { }
    }

    class SpAware
    {
        public IServiceProvider ServiceProvider { get; }
        public ITest Test { get; }
        public ITest2 Test2 { get; }

        public SpAware(IServiceProvider serviceProvider, ITest test, ITest2 test2)
        {
            ServiceProvider = serviceProvider;
            Test = test;
            Test2 = test2;
        }
    }
}