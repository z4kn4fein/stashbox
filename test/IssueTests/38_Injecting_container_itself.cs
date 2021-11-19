using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class InjectingContainerItself
    {
        [Fact]
        public void Injecting_container_itself()
        {
            var container = new StashboxContainer();
            var factory = container.RegisterInstance(container)
                .RegisterSingleton<ICoreFactory, CoreFactory>()
                .Resolve<ICoreFactory>();

            Assert.NotNull(factory);
        }

        [Fact]
        public void Injecting_container_itself2()
        {
            var factory = new StashboxContainer()
                .RegisterSingleton<ICoreFactory, CoreFactory2>()
                .Resolve<ICoreFactory>();

            Assert.NotNull(factory);
        }
    }

    interface ICoreFactory
    {
        IEngine GetEngine();
        ILogManager GetLogManager();
    }

    interface IEngine
    { }

    interface ILogManager
    { }

    class CoreFactory : ICoreFactory
    {
        private readonly StashboxContainer _container;

        public CoreFactory(StashboxContainer container)
        {
            _container = container;
        }

        public IEngine GetEngine()
        {
            return _container.Resolve<IEngine>();
        }

        public ILogManager GetLogManager()
        {
            return _container.Resolve<ILogManager>();
        }
    }

    class CoreFactory2 : ICoreFactory
    {
        private readonly IDependencyResolver _container;

        public CoreFactory2(IDependencyResolver container)
        {
            _container = container;
        }

        public IEngine GetEngine()
        {
            return _container.Resolve<IEngine>();
        }

        public ILogManager GetLogManager()
        {
            return _container.Resolve<ILogManager>();
        }
    }
}
