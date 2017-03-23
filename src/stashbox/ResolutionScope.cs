using System;
using System.Collections.Generic;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Utils;

namespace Stashbox
{
    /// <summary>
    /// Represents a resolution scope.
    /// </summary>
    public class ResolutionScope : IResolutionScope
    {
        private readonly IActivationContext activationContext;
        private readonly AtomicBool disposed;
        private readonly ConcurrentStore<IDisposable> disposableObjects;
        private readonly ConcurrentTree<object, object> scopedItems;

        /// <summary>
        /// Constructs a <see cref="ResolutionScope"/>.
        /// </summary>
        public ResolutionScope(IActivationContext activationContext)
        {
            this.activationContext = activationContext;
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
        public TKey Resolve<TKey>(string name = null, bool nullResultAllowed = false) where TKey : class =>
            this.activationContext.Activate(typeof(TKey), this, name, nullResultAllowed) as TKey;

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null, bool nullResultAllowed = false) =>
            this.activationContext.Activate(typeFrom, this, name, nullResultAllowed);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() where TKey : class =>
            this.activationContext.Activate(typeof(IEnumerable<TKey>), this) as IEnumerable<TKey>;

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.activationContext.Activate(type, this);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, string name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.activationContext.ActivateFactory(typeFrom, parameterTypes, this, name, nullResultAllowed);

        /// <inheritdoc />
        public IResolutionScope BeginScope() => new ResolutionScope(this.activationContext);
        
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
