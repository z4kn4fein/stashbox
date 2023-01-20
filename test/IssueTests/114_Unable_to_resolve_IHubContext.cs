using Xunit;

namespace Stashbox.Tests.IssueTests;

public class UnableToResolveIHubContext
{
    [Fact]
    public void Ensure_Resolving_HubContext_Works()
    {
        using var container = new StashboxContainer()
            .Register(typeof(IHubContext<,>), typeof(HubContext<,>))
            .Register(typeof(HubLifetimeManager<>));

        Assert.NotNull(container.Resolve<IHubContext<TestHub, ITest>>());
    }

    class Hub { }

    class Hub<T> : Hub
        where T : class
    { }

    interface IHubContext<THub, T>
        where THub : Hub<T>
        where T : class
    { }

    class HubLifetimeManager<THub> where THub : Hub { }

    class HubContext<THub, T> : IHubContext<THub, T>
        where THub : Hub<T>
        where T : class
    {
        public HubContext(HubLifetimeManager<THub> lifetimeManager)
        { }
    }

    interface ITest { }

    class TestHub : Hub<ITest>
    { }
}