using Stashbox.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    internal partial class ResolutionScope
    {
        private class DisposableItem
        {
            public object Item;
            public DisposableItem Next;

            public static readonly DisposableItem Empty = new DisposableItem();
        }

        private class FinalizableItem
        {
            public object Item;
            public Action<object> Finalizer;
            public FinalizableItem Next;

            public static readonly FinalizableItem Empty = new FinalizableItem();
        }

        private int disposed;
        private DisposableItem rootItem = DisposableItem.Empty;
        private FinalizableItem rootFinalizableItem = FinalizableItem.Empty;

        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
        {
            Swap.SwapValue(ref this.rootItem, (t1, t2, t3, t4, root) =>
                    new DisposableItem { Item = t1, Next = root }, disposable, Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return disposable;
        }

        public TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer)
        {
            Swap.SwapValue(ref this.rootFinalizableItem, (t1, t2, t3, t4, root) =>
                    new FinalizableItem { Item = t1, Finalizer = f => t2((TService)f), Next = root }, finalizable, finalizer,
                Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return finalizable;
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.CallFinalizers();
            this.CallDisposes();

            if (this.circularDependencyBarrier.IsEmpty)
                return;

            foreach (var threadLocal in this.circularDependencyBarrier.Walk())
                threadLocal.Value.Dispose();
        }

#if HAS_ASYNC_DISPOSABLE
        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.CallFinalizers();
            await CallAsyncDisposes().ConfigureAwait(false);

            if (this.circularDependencyBarrier.IsEmpty)
                return;

            foreach (var threadLocal in this.circularDependencyBarrier.Walk())
                threadLocal.Value.Dispose();

            async ValueTask CallAsyncDisposes()
            {
                var root = this.rootItem;
                while (!ReferenceEquals(root, DisposableItem.Empty))
                {
                    if (root.Item is IAsyncDisposable asyncDisposable)
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    else
                        ((IDisposable)root.Item).Dispose();

                    root = root.Next;
                }
            }
        }
#endif

        private void CallFinalizers()
        {
            var rootFinalizable = this.rootFinalizableItem;
            while (!ReferenceEquals(rootFinalizable, FinalizableItem.Empty))
            {
                rootFinalizable.Finalizer(rootFinalizable.Item);
                rootFinalizable = rootFinalizable.Next;
            }
        }

        private void CallDisposes()
        {
            var root = this.rootItem;
            while (!ReferenceEquals(root, DisposableItem.Empty) && root.Item is IDisposable disposable)
            {
                disposable.Dispose();
                root = root.Next;
            }
        }
    }
}
