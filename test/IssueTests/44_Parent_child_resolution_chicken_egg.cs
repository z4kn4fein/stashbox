using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class ParentChildResolutionChickenEgg
    {
        [TestMethod]
        public void Parent_child_resolution_chicken_egg()
        {
            var container = new StashboxContainer(options => options.WithCircularDependencyWithLazy());
            var instance = container
                .Register<IPsnServer, PsnServer>(options => options.InjectMember(server => server.LatencyCheck).WithSingletonLifetime())
                .Register<ILatencyCheck, LatencyCheck>(options => options.InjectMember(check => check.Server).WithSingletonLifetime())
                .Resolve<IPsnServer>();

            Assert.AreSame(instance, instance.LatencyCheck.Value.Server);
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
