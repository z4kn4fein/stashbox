using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class VerifyChildContainerWorking
    {
        [TestMethod]
        public void Verify_child_container_working()
        {
            var container = new StashboxContainer()
                .RegisterSingleton<Singleton>()
                .RegisterScoped<Scoped>();

            Assert.AreEqual(1, container.Resolve<Singleton>().Id);
            Assert.AreEqual(1, container.BeginScope().Resolve<Scoped>().Id);

            var child = container.CreateChildContainer();
            Assert.AreEqual(1, child.Resolve<Singleton>().Id);
            Assert.AreEqual(2, child.BeginScope().Resolve<Scoped>().Id);

            var child2 = container.CreateChildContainer().RegisterSingleton<Singleton>();
            Assert.AreEqual(2, child2.Resolve<Singleton>().Id);
            Assert.AreEqual(3, child2.BeginScope().Resolve<Scoped>().Id);
        }

        private class Singleton
        {
            private static int seed;

            public int Id = Interlocked.Increment(ref seed);
        }

        private class Scoped
        {
            private static int seed;

            public int Id = Interlocked.Increment(ref seed);
        }
    }
}
