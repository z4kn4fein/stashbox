using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Stashbox
{
    internal sealed partial class ResolutionScope : IResolutionScope
    {
        private class DelegateCache
        {
            public ImmutableTree<object, Func<IResolutionScope, object>> ServiceDelegates = ImmutableTree<object, Func<IResolutionScope, object>>.Empty;
            public ImmutableTree<object, Func<IResolutionScope, Delegate>> FactoryDelegates = ImmutableTree<object, Func<IResolutionScope, Delegate>>.Empty;
        }

        private class DelegateCacheProvider
        {
            public readonly DelegateCache DefaultCache = new DelegateCache();
            public ImmutableTree<object, DelegateCache> NamedCache = ImmutableTree<object, DelegateCache>.Empty;

            public DelegateCache GetNamedCache(object name)
            {
                var cache = this.NamedCache.GetOrDefault(name, false);
                if (cache != null) return cache;

                cache = new DelegateCache();
                if(Swap.SwapValue(ref this.NamedCache, (t1, t2, t3, t4, items) =>
                    items.AddOrUpdate(t1, t2, false), name, cache, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder))
                return cache;

                return this.NamedCache.GetOrDefault(name, false);
            }
        }


        private readonly ResolutionStrategy resolutionStrategy;
        private readonly ExpressionFactory expressionFactory;
        private readonly IContainerContext containerContext;
        private readonly DelegateCacheProvider delegateCacheProvider;
        private readonly DelegateCache delegateCache;

        private ImmutableTree<object> scopedItems = ImmutableTree<object>.Empty;
        private ImmutableTree<object, object> scopedInstances = ImmutableTree<object, object>.Empty;
        private ImmutableTree<ThreadLocal<bool>> circularDependencyBarrier = ImmutableTree<ThreadLocal<bool>>.Empty;

        public object Name { get; }

        public IResolutionScope ParentScope { get; }

        private ResolutionScope(ResolutionStrategy resolutionStrategy,
            ExpressionFactory expressionBuilder, IContainerContext containerContext,
            DelegateCacheProvider delegateCacheProvider, object name)
        {
            this.resolutionStrategy = resolutionStrategy;
            this.expressionFactory = expressionBuilder;
            this.containerContext = containerContext;
            this.Name = name;
            this.delegateCacheProvider = delegateCacheProvider;

            this.delegateCache = name == null
                ? delegateCacheProvider.DefaultCache
                : delegateCacheProvider.GetNamedCache(name);
        }

        internal ResolutionScope(ResolutionStrategy resolutionStrategy,
            ExpressionFactory expressionBuilder, IContainerContext containerContext)
            : this(resolutionStrategy, expressionBuilder, containerContext,
                  new DelegateCacheProvider(), null)
        { }

        private ResolutionScope(ResolutionStrategy resolutionStrategy, ExpressionFactory expressionBuilder,
            IContainerContext containerContext, IResolutionScope parent, DelegateCacheProvider delegateCacheProvider, object name = null)
            : this(resolutionStrategy, expressionBuilder, containerContext, delegateCacheProvider, name)
        {
            this.ParentScope = parent;
        }

        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
        {
            this.ThrowIfDisposed();

            var scope = new ResolutionScope(this.resolutionStrategy, this.expressionFactory,
                this.containerContext, this, this.delegateCacheProvider, name);

            return attachToParent ? this.AddDisposableTracking(scope) : scope;
        }

        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object name = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            var key = name ?? typeFrom;
            Swap.SwapValue(ref this.scopedInstances, (t1, t2, t3, t4, instances) =>
                instances.AddOrUpdate(t1, t2, false), key, instance, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            if (!withoutDisposalTracking && instance is IDisposable disposable)
                this.AddDisposableTracking(disposable);

            this.InvalidateDelegateCache();

            return this;
        }

        public object GetOrAddScopedObject(int key, object sync, Func<IResolutionScope, object> factory)
        {
            this.ThrowIfDisposed();

            var item = this.scopedItems.GetOrDefault(key);
            if (item != null) return item;

            lock (sync)
            {
                item = this.scopedItems.GetOrDefault(key);
                if (item != null) return item;

                item = factory(this);
                Swap.SwapValue(ref this.scopedItems, (t1, t2, t3, t4, items) =>
                    items.AddOrUpdate(t1, t2), key, item, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

                return item;
            }
        }

        public void InvalidateDelegateCache()
        {
            this.ThrowIfDisposed();

            this.delegateCacheProvider.DefaultCache.ServiceDelegates = ImmutableTree<object, Func<IResolutionScope, object>>.Empty;
            this.delegateCacheProvider.DefaultCache.FactoryDelegates = ImmutableTree<object, Func<IResolutionScope, Delegate>>.Empty;
            this.delegateCacheProvider.NamedCache = ImmutableTree<object, DelegateCache>.Empty;
        }

        public IEnumerable<object> GetActiveScopeNames()
        {
            this.ThrowIfDisposed();

            IResolutionScope current = this;
            while (current != null)
            {
                if (current.Name != null)
                    yield return current.Name;

                current = current.ParentScope;
            }
        }

        public void CheckRuntimeCircularDependencyBarrier(int key, Type type)
        {
            var check = this.circularDependencyBarrier.GetOrDefault(key);
            if (check != null && check.Value)
                throw new CircularDependencyException(type);

            Swap.SwapValue(ref this.circularDependencyBarrier, (t1, t2, t3, t4, barrier) => barrier.AddOrUpdate(t1, new ThreadLocal<bool>(), (old, @new) =>
                { old.Value = true; return old; }), key, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
        }

        public void ResetRuntimeCircularDependencyBarrier(int key)
        {
            var check = this.circularDependencyBarrier.GetOrDefault(key);
            if (check != null)
                check.Value = false;
        }

        private void ThrowIfDisposed(
#if !NET40
            [CallerMemberName] 
#endif
            string caller = "<unknown>")
        {
            if (this.disposed == 1)
                Shield.ThrowDisposedException(this.GetType().FullName, caller);
        }
    }
}
