using System;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    
    public class ParentChildResolutionChickenEgg
    {
        [Fact]
        public void Parent_child_resolution_chicken_egg()
        {
            var container = new StashboxContainer(options => options.WithCircularDependencyWithLazy());
            var instance = container
                .Register<IPsnServer, PsnServer>(options => options.WithDependencyBinding(server => server.LatencyCheck).WithSingletonLifetime())
                .Register<ILatencyCheck, LatencyCheck>(options => options.WithDependencyBinding(check => check.Server).WithSingletonLifetime())
                .Resolve<IPsnServer>();

            Assert.Same(instance, instance.LatencyCheck.Value.Server);
        }
    }

    interface IPsnServer
    {
        Lazy<ILatencyCheck> LatencyCheck { get; }
    }

    interface ILatencyCheck
    {
        IPsnServer Server { get; }
    }

    class PsnServer : IPsnServer
    {
        public Lazy<ILatencyCheck> LatencyCheck { get; set; }
    }

    class LatencyCheck : ILatencyCheck
    {
        public IPsnServer Server { get; set; }
    }
}
