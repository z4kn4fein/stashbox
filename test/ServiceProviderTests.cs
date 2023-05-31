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
        using var container = new StashboxContainer().Register<IServiceProvider>(c => c.WithFactory(dr => new CustomSp(dr)));

        Assert.IsType<CustomSp>(container.Resolve<IServiceProvider>());
        Assert.Same(container.ContainerContext.RootScope, ((CustomSp)container.Resolve<IServiceProvider>()).DependencyResolver);

        using var scope = container.BeginScope();
        
        Assert.IsType<CustomSp>(scope.Resolve<IServiceProvider>());
        Assert.Same(scope, ((CustomSp)scope.Resolve<IServiceProvider>()).DependencyResolver);
    }

    class CustomSp : IServiceProvider
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
    }
}