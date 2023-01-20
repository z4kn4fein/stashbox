using Castle.DynamicProxy;
using System;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class CallInterception
{
    [Fact]
    public void Ensure_Expression_Override_Does_Not_Mess_Up_Cache()
    {
        var proxyBuilder = new DefaultProxyBuilder();

        using var container = new StashboxContainer().Register<IInterceptor, NoInterceptor>()
            .RegisterDecorator<ILevel2Service>(proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(typeof(ILevel2Service), new Type[0], ProxyGenerationOptions.Default))
            .RegisterDecorator<ILevel2bService>(proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(typeof(ILevel2bService), new Type[0], ProxyGenerationOptions.Default))
            .RegisterDecorator<ILevel3Service>(proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(typeof(ILevel3Service), new Type[0], ProxyGenerationOptions.Default))
            .RegisterScoped<ILevel1Service, Level1Service>()
            .RegisterScoped<ILevel2Service, Level2Service>()
            .RegisterScoped<ILevel2bService, Level2bService>()
            .RegisterScoped<ILevel3Service, Level3Service>()
            .RegisterScoped<ILevel4Service, Level4Service>();

        Assert.NotNull(container.Resolve<ILevel2Service>());
    }

    public interface ILevel1Service
    {
    }

    public interface ILevel2bService
    {
    }

    public interface ILevel2Service
    {
    }

    public interface ILevel3Service
    {
    }

    public interface ILevel4Service
    {
    }

    class Level1Service : ILevel1Service
    {
        private readonly ILevel2Service level2Service;

        public Level1Service(ILevel2Service level2Service)
        {
            this.level2Service = level2Service;
        }
    }

    class Level2bService : ILevel2bService
    {
        public Level2bService(ILevel3Service level3Service)
        {

        }
    }

    class Level2Service : ILevel2Service
    {
        public Level2Service(ILevel2bService level2BService, ILevel3Service level3Service)
        {

        }
    }

    class Level3Service : ILevel3Service
    {
        public Level3Service(ILevel4Service level4Service)
        {

        }
    }

    class Level4Service : ILevel4Service
    {
        public Level4Service()
        {
        }
    }

    class NoInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {

        }
    }
}