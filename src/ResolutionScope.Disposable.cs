using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    internal partial class ResolutionScope
    {
        private readonly struct Finalizable
        {
            public readonly object Item;
            public readonly Action<object> Finalizer;

            public Finalizable(object item, Action<object> finalizer)
            {
                this.Item = item;
                this.Finalizer = finalizer;
            }
        }

        private int disposed;
        private ImmutableLinkedList<object> disposables = ImmutableLinkedList<object>.Empty;
        private ImmutableLinkedList<Finalizable> finalizables = ImmutableLinkedList<Finalizable>.Empty;

        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
        {
            this.ThrowIfDisposed();

            Swap.SwapValue(ref this.disposables, (t1, t2, t3, t4, root) =>
                    root.Add(t1), disposable, Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return disposable;
        }

        public TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer)
        {
            this.ThrowIfDisposed();

            Swap.SwapValue(ref this.finalizables, (t1, t2, t3, t4, root) =>
                    root.Add(new Finalizable(t1, f => t2((TService)f))), finalizable, finalizer,
                Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return finalizable;
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.CallFinalizers();
            this.CallDisposes();
            this.ReleaseRuntimeCircularDependencyBarriers();
        }

#if HAS_ASYNC_DISPOSABLE
        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.CallFinalizers();
            await CallAsyncDisposes().ConfigureAwait(false);
            this.ReleaseRuntimeCircularDependencyBarriers();

            async ValueTask CallAsyncDisposes()
            {
                var root = this.disposables;
                while (!ReferenceEquals(root, ImmutableLinkedList<object>.Empty))
                {
                    if (root.Value is IAsyncDisposable asyncDisposable)
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    else
                        ((IDisposable)root.Value).Dispose();

                    root = root.Next;
                }
            }
        }
#endif

        private void CallFinalizers()
        {
            var rootFinalizable = this.finalizables;
            while (!ReferenceEquals(rootFinalizable, ImmutableLinkedList<Finalizable>.Empty))
            {
                rootFinalizable.Value.Finalizer(rootFinalizable.Value.Item);
                rootFinalizable = rootFinalizable.Next;
            }
        }

        private void CallDisposes()
        {
            var root = this.disposables;
            while (!ReferenceEquals(root, ImmutableLinkedList<object>.Empty) && root.Value is IDisposable disposable)
            {
                disposable.Dispose();
                root = root.Next;
            }
        }

        private void ReleaseRuntimeCircularDependencyBarriers()
        {
            if (this.circularDependencyBarrier.IsEmpty)
                return;

            foreach (var threadLocal in this.circularDependencyBarrier.Walk())
                threadLocal.Value.Dispose();
        }
    }
}
