using System;
using Stashbox.Infrastructure;
using Stashbox.Utils;

namespace Stashbox
{
    /// <summary>
    /// Represents a base resolution scope.
    /// </summary>
    public class ResolutionScopeBase : IResolutionScope
    {
        private class DisposableItem
        {
            public IDisposable Item;
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

        private readonly AtomicBool disposed;
        private DisposableItem rootItem;
        private FinalizableItem rootFinalizableItem;
        private readonly ConcurrentTree<object, object> scopedItems;
        private readonly ConcurrentTree<Type, object> scopedInstances;

        /// <inheritdoc />
        public bool HasScopedInstances => !this.scopedInstances.IsEmpty;

        /// <summary>
        /// Constructs a <see cref="ResolutionScopeBase"/>.
        /// </summary>
        public ResolutionScopeBase()
        {
            this.disposed = new AtomicBool();
            this.rootItem = DisposableItem.Empty;
            this.rootFinalizableItem = FinalizableItem.Empty;
            this.scopedItems = new ConcurrentTree<object, object>();
            this.scopedInstances = new ConcurrentTree<Type, object>();
        }

        /// <inheritdoc />
        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable
        {

            var item = new DisposableItem { Item = disposable, Next = this.rootItem };
            var current = this.rootItem;
            Swap.SwapValue(ref this.rootItem, current, item, root =>
                new DisposableItem { Item = disposable, Next = root });

            return disposable;
        }

        /// <inheritdoc />
        public void AddScopedItem(object key, object value) =>
            this.scopedItems.AddOrUpdate(key, value);

        /// <inheritdoc />
        public object GetScopedItemOrDefault(object key) =>
            this.scopedItems.GetOrDefault(key);

        /// <inheritdoc />
        public void AddScopedInstance(Type key, object value) =>
            this.scopedInstances.AddOrUpdate(key, value);

        /// <inheritdoc />
        public object GetScopedInstanceOrDefault(Type key) =>
            this.scopedInstances.GetOrDefault(key);

        /// <inheritdoc />
        public TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer)
        {
            var item = new FinalizableItem { Item = finalizable, Finalizer = f => finalizer((TService)f), Next = this.rootFinalizableItem };
            var current = this.rootFinalizableItem;
            Swap.SwapValue(ref this.rootFinalizableItem, current, item, root =>
                new FinalizableItem { Item = finalizable, Finalizer = f => finalizer((TService)f), Next = root });

            return finalizable;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the scope.
        /// </summary>
        /// <param name="disposing">Indicates the scope is disposing or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed.CompareExchange(false, true) || !disposing) return;

            var rootFinalizable = this.rootFinalizableItem;
            while (!ReferenceEquals(rootFinalizable, FinalizableItem.Empty))
            {
                rootFinalizable.Finalizer(rootFinalizable.Item);
                rootFinalizable = rootFinalizable.Next;
            }

            var root = this.rootItem;
            while (!ReferenceEquals(root, DisposableItem.Empty))
            {
                root.Item?.Dispose();
                root = root.Next;
            }
        }
    }
}
