using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Infrastructure;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class ScopedLifetimeThreadSafeTests
    {
        [TestMethod]
        public void ScopedLifetime_thread_safety()
        {
            for (var i = 0; i < 1000; i++)
            {
                using (IStashboxContainer container = new StashboxContainer())
                {
                    container.RegisterScoped<Test4>();

                    using (var scope = container.BeginScope())
                    {
                        Parallel.For(0, 50, _ => scope.Resolve<Test4>());
                    }
                }
            }
        }
    }
}
