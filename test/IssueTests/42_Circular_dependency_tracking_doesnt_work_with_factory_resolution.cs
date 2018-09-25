using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Exceptions;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class CircularDependencyTrackingDoesntWorkWithFactoryResolution
    {
        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void Circular_dependency_tracking_doesnt_work_with_factory_resolution()
        {
            var container = new StashboxContainer(configurator => configurator.WithCircularDependencyTracking(true));
            container.Register<IFoo, Foo>(registrator => registrator.WithFactory(() => new Foo(container.Resolve<IFoo>())));
            container.Resolve<IFoo>();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void Circular_dependency_tracking_doesnt_work_with_factory_resolution_resolver()
        {
            var container = new StashboxContainer(configurator => configurator.WithCircularDependencyTracking(true));
            container.Register<IFoo, Foo>(registrator => registrator.WithFactory(resolver => new Foo(resolver.Resolve<IFoo>())));
            container.Resolve<IFoo>();
        }

        interface IFoo
        { }

        class Foo : IFoo
        {
            public Foo(IFoo foo)
            { }
        }
    }
}
