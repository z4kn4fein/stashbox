using System;
using System.Threading;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class VerifyChildContainerWorking
    {
        [Fact]
        public void Verify_child_container_working()
        {
            var container = new StashboxContainer()
                .RegisterSingleton<Singleton>()
                .RegisterScoped<Scoped>();

            Assert.Equal(1, container.Resolve<Singleton>().Id);
            Assert.Equal(1, container.BeginScope().Resolve<Scoped>().Id);

            var child = container.CreateChildContainer();
            Assert.Equal(1, child.Resolve<Singleton>().Id);
            Assert.Equal(2, child.BeginScope().Resolve<Scoped>().Id);

            var child2 = container.CreateChildContainer().RegisterSingleton<Singleton>();
            Assert.Equal(2, child2.Resolve<Singleton>().Id);
            Assert.Equal(3, child2.BeginScope().Resolve<Scoped>().Id);
        }

        [Fact]
        public void Verify_child_container_working_dispose()
        {
            Singleton s = null;
            {
                using var container = new StashboxContainer()
                    .RegisterSingleton<Singleton>();
                {
                    {
                        using var child = container.CreateChildContainer();
                        s = child.Resolve<Singleton>();
                    }

                    Assert.False(s.Disposed);
                }
            }

            Assert.True(s.Disposed);
        }

        private class Singleton : IDisposable
        {
            private static int seed;

            public int Id = Interlocked.Increment(ref seed);

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(Singleton));

                this.Disposed = true;
            }
        }

        private class Scoped
        {
            private static int seed;

            public int Id = Interlocked.Increment(ref seed);
        }
    }
}
