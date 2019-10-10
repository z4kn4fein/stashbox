using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class CircularDependencyTrackingDoesntWorkWithFactoryResolution
    {
        [Fact]
        public void Circular_dependency_tracking_doesnt_work_with_factory_resolution()
        {
            var container = new StashboxContainer(configurator => configurator.WithCircularDependencyTracking(true));
            container.Register<IFoo, Foo>(registrator => registrator.WithFactory(() => new Foo(container.Resolve<IFoo>())));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<IFoo>());
        }

        [Fact]
        public void Circular_dependency_tracking_doesnt_work_with_factory_resolution_resolver()
        {
            var container = new StashboxContainer(configurator => configurator.WithCircularDependencyTracking(true));
            container.Register<IFoo, Foo>(registrator => registrator.WithFactory(resolver => new Foo(resolver.Resolve<IFoo>())));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<IFoo>());
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
