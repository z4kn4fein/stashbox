using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
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


        private readonly ResolutionStrategy resolutionStrategy;
        private readonly ExpressionFactory expressionFactory;
        private readonly IContainerContext containerContext;

        private ImmutableTree<object> scopedItems = ImmutableTree<object>.Empty;
        private ImmutableTree<object, object> scopedInstances = ImmutableTree<object, object>.Empty;
        private ImmutableTree<ThreadLocal<bool>> circularDependencyBarrier = ImmutableTree<ThreadLocal<bool>>.Empty;

        private DelegateCache delegateCache;

        public object Name { get; }

        public IResolutionScope ParentScope { get; }

        private ResolutionScope(ResolutionStrategy resolutionStrategy,
            ExpressionFactory expressionBuilder, IContainerContext containerContext,
            DelegateCache delegateCache, object name)
        {
            this.resolutionStrategy = resolutionStrategy;
            this.expressionFactory = expressionBuilder;
            this.containerContext = containerContext;
            this.Name = name;
            this.delegateCache = delegateCache;
        }

        internal ResolutionScope(ResolutionStrategy resolutionStrategy,
            ExpressionFactory expressionBuilder, IContainerContext containerContext)
            : this(resolutionStrategy, expressionBuilder, containerContext,
                  new DelegateCache(), null)
        { }

        private ResolutionScope(ResolutionStrategy resolutionStrategy, ExpressionFactory expressionBuilder,
            IContainerContext containerContext, IResolutionScope parent, DelegateCache delegateCache, object name = null)
            : this(resolutionStrategy, expressionBuilder, containerContext, delegateCache, name)
        {
            this.ParentScope = parent;
        }

        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
        {
            var scope = new ResolutionScope(this.resolutionStrategy, this.expressionFactory,
                this.containerContext, this, this.delegateCache, name);

            return attachToParent ? this.AddDisposableTracking(scope) : scope;
        }

        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object name = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            var key = name ?? typeFrom;
            Swap.SwapValue(ref this.scopedInstances, (t1, t2, t3, t4, instances) =>
                instances.AddOrUpdate(t1, t2), key, instance, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            if (!withoutDisposalTracking && instance is IDisposable disposable)
                this.AddDisposableTracking(disposable);

            this.delegateCache = new DelegateCache();

            return this;
        }

        public object GetOrAddScopedObject(int key, object sync, Func<IResolutionScope, object> factory)
        {
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
            this.delegateCache.ServiceDelegates = ImmutableTree<object, Func<IResolutionScope, object>>.Empty;
            this.delegateCache.FactoryDelegates = ImmutableTree<object, Func<IResolutionScope, Delegate>>.Empty;
        }

        public IEnumerable<object> GetActiveScopeNames()
        {
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
    }
}
