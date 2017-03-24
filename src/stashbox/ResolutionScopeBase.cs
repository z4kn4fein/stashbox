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
        private readonly AtomicBool disposed;

        private readonly ConcurrentStore<IDisposable> disposableObjects;

        private readonly ConcurrentTree<object, object> scopedItems;

        /// <summary>
        /// Constructs a <see cref="ResolutionScopeBase"/>.
        /// </summary>
        public ResolutionScopeBase()
        {
            this.disposed = new AtomicBool();
            this.disposableObjects = new ConcurrentStore<IDisposable>();
            this.scopedItems = new ConcurrentTree<object, object>();
        }

        /// <inheritdoc />
        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable
        {
            this.disposableObjects.Add(disposable);
            return disposable;
        }

        /// <inheritdoc />
        public void AddOrUpdateScopedItem(object key, object value) =>
            this.scopedItems.AddOrUpdate(key, value, (oldValue, newValue) => newValue);

        /// <inheritdoc />
        public object GetScopedItemOrDefault(object key) =>
            this.scopedItems.GetOrDefault(key);

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
            foreach (var disposableObject in disposableObjects)
                disposableObject?.Dispose();
        }
    }
}
