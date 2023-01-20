using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class CircularDependencyTrackingDoesntWorkWithFactoryResolution
{
    [Fact]
    public void Circular_dependency_tracking_doesnt_work_with_factory_resolution()
    {
        var container = new StashboxContainer();
        container.Register<IFoo, Foo>(registrator => registrator.WithFactory<IFoo>(f => new Foo(f)));
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