using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class InjectingContainerItself
    {
        [TestMethod]
        public void Injecting_container_itself()
        {
            var container = new StashboxContainer();
            var factory = container.RegisterInstance(container)
                .RegisterSingleton<ICoreFactory, CoreFactory>()
                .Resolve<ICoreFactory>();

            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void Injecting_container_itself2()
        {
            var factory = new StashboxContainer()
                .RegisterSingleton<ICoreFactory, CoreFactory2>()
                .Resolve<ICoreFactory>();

            Assert.IsNotNull(factory);
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
