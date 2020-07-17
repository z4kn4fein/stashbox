using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#if HAS_ASYNC_DISPOSABLE

namespace Stashbox.Tests
{
    public class AsyncDisposeTests
    {
        [Fact]
        public async Task Async_Dispose_Works()
        {
            AsyncDisposable disposable;
            {
                await using var container = new StashboxContainer(c => c.WithDisposableTransientTracking())
                    .Register<AsyncDisposable>();
                disposable = container.Resolve<AsyncDisposable>();
            }

            Assert.True(disposable.Disposed);
        }

        [Fact]
        public async Task Async_Dispose_Works_On_Scoped()
        {
            AsyncDisposable disposable;

            await using var container = new StashboxContainer()
                .RegisterScoped<AsyncDisposable>();

            {
                await using var scope = container.BeginScope();
                disposable = scope.Resolve<AsyncDisposable>();
            }

            Assert.True(disposable.Disposed);
        }

        [Fact]
        public async Task Async_Dispose_Works_On_Singleton()
        {
            AsyncDisposable disposable;
            {
                await using var container = new StashboxContainer()
                    .RegisterSingleton<AsyncDisposable>();

                {
                    await using var scope = container.BeginScope();
                    disposable = scope.Resolve<AsyncDisposable>();
                }

                Assert.False(disposable.Disposed);
            }

            Assert.True(disposable.Disposed);
        }

        [Fact]
        public async Task Disposes_Either_Normal_And_Async()
        {
            AsyncDisposable asyncDisposable;
            Disposable disposable;
            {
                await using var container = new StashboxContainer(c => c.WithDisposableTransientTracking())
                    .Register<AsyncDisposable>()
                    .Register<Disposable>();
                asyncDisposable = container.Resolve<AsyncDisposable>();
                disposable = container.Resolve<Disposable>();
            }

            Assert.True(asyncDisposable.Disposed);
            Assert.True(disposable.Disposed);
        }

        [Fact]
        public async Task Prefers_Async_Disposable_Over_Normal()
        {
            A disposable;
            {
                await using var container = new StashboxContainer(c => c.WithDisposableTransientTracking())
                    .Register<A>();
                disposable = container.Resolve<A>();
            }

            Assert.True(disposable.Disposed);
        }

        [Fact]
        public async Task Async_Dispose_Works_On_Dependencies()
        {
            B b;
            C c;
            {
                await using var container = new StashboxContainer(c => c
                        .WithDisposableTransientTracking()
                        .WithUnknownTypeResolution())
                    .Register<B>()
                    .Register<C>();
                b = container.Resolve<B>();
                c = container.Resolve<C>();
            }

            Assert.True(b.Disposed);
            Assert.True(b.Disposable.Disposed);
            Assert.True(c.Disposed);
            Assert.True(c.Disposable.Disposed);
        }

        class A : AsyncDisposable, IDisposable
        {
            public void Dispose()
            {
                throw new InvalidOperationException("should not be called.");
            }
        }

        class B : AsyncDisposable
        {
            public Disposable Disposable { get; }

            public B(Disposable disposable)
            {
                Disposable = disposable;
            }
        }

        class C : Disposable
        {
            public AsyncDisposable Disposable { get; }

            public C(AsyncDisposable disposable)
            {
                Disposable = disposable;
            }
        }

        class AsyncDisposable : IAsyncDisposable
        {
            private int disposed;

            public bool Disposed => this.disposed == 1;

            public ValueTask DisposeAsync()
            {
                if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                    throw new ObjectDisposedException("object already disposed");

                return new ValueTask(Task.CompletedTask);
            }
        }

        class Disposable : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (Disposed)
                    throw new ObjectDisposedException("object already disposed");

                Disposed = true;
            }
        }
    }
}
#endif