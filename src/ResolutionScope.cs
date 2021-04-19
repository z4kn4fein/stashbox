using Stashbox.Exceptions;
using Stashbox.Expressions;
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
                var cache = this.NamedCache.GetOrDefaultByValue(name);
                if (cache != null) return cache;

                var newCache = new DelegateCache();
                if (Swap.SwapValue(ref this.NamedCache, (t1, t2, t3, t4, items) =>
                     items.AddOrUpdate(t1, t2, false), name, newCache, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder))
                    return newCache;

                return this.NamedCache.GetOrDefaultByValue(name) ?? newCache;
            }
        }

        private class ScopedEvaluator
        {
            private static int MaxWaitTimeInMs = 3000;
            private static readonly object Default = new object();
            private readonly Type serviceType;
            private int evaluated;
            private object evaluatedObject = Default;

            public ScopedEvaluator(Type serviceType)
            {
                this.serviceType = serviceType;
            }

            public object Evaluate(IResolutionScope scope, Func<IResolutionScope, object> factory)
            {
                if (!ReferenceEquals(this.evaluatedObject, Default))
                    return this.evaluatedObject;

                if (Interlocked.CompareExchange(ref this.evaluated, 1, 0) != 0)
                    return this.WaitForEvaluation();

                this.evaluatedObject = factory(scope);
                return this.evaluatedObject;
            }

            private object WaitForEvaluation()
            {
                SpinWait spin = default;
                var startTime = (uint)Environment.TickCount;
                while (ReferenceEquals(this.evaluatedObject, Default))
                {
                    if(spin.NextSpinWillYield)
                    {
                        var currentTime = (uint)Environment.TickCount;
                        if(MaxWaitTimeInMs <= currentTime - startTime)
                            throw new ResolutionFailedException(this.serviceType,
                                $"The scoped service {this.serviceType} was unavailable after {MaxWaitTimeInMs} ms. " +
                                "It's possible that the thread used to construct it crashed by a handled exception." +
                                "This exception is supposed to prevent other caller threads from infinite waiting for service construction.");
                    }

                    spin.SpinOnce();
                }

                return this.evaluatedObject;
            }
        }

        private readonly ExpressionFactory expressionFactory;
        private readonly IContainerContext containerContext;
        private readonly DelegateCacheProvider delegateCacheProvider;
        private readonly DelegateCache delegateCache;

        private ImmutableTree<ScopedEvaluator> scopedItems = ImmutableTree<ScopedEvaluator>.Empty;
        private ImmutableTree<object, object> scopedInstances = ImmutableTree<object, object>.Empty;
        private ImmutableTree<ThreadLocal<bool>> circularDependencyBarrier = ImmutableTree<ThreadLocal<bool>>.Empty;

        public object Name { get; }

        public IResolutionScope ParentScope { get; }

        private ResolutionScope(ExpressionFactory expressionBuilder, IContainerContext containerContext,
            DelegateCacheProvider delegateCacheProvider, object name)
        {
            this.expressionFactory = expressionBuilder;
            this.containerContext = containerContext;
            this.Name = name;
            this.delegateCacheProvider = delegateCacheProvider;

            this.delegateCache = name == null
                ? delegateCacheProvider.DefaultCache
                : delegateCacheProvider.GetNamedCache(name);
        }

        internal ResolutionScope(ExpressionFactory expressionBuilder, IContainerContext containerContext)
            : this(expressionBuilder, containerContext, new DelegateCacheProvider(), null)
        { }

        private ResolutionScope(ExpressionFactory expressionBuilder, IContainerContext containerContext,
            IResolutionScope parent, DelegateCacheProvider delegateCacheProvider, object name = null)
            : this(expressionBuilder, containerContext, delegateCacheProvider, name)
        {
            this.ParentScope = parent;
        }

        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
        {
            this.ThrowIfDisposed();

            var scope = new ResolutionScope(this.expressionFactory,
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

        public object GetOrAddScopedObject(int key, Func<IResolutionScope, object> factory, Type requestedType, bool validateLifetimeFromRootScope = false)
        {
            this.ThrowIfDisposed();

            var item = this.scopedItems.GetOrDefault(key);
            if (item != null) return item.Evaluate(this, factory);

            if (validateLifetimeFromRootScope &&
                this.containerContext.ContainerConfiguration.LifetimeValidationEnabled &&
                ReferenceEquals(this, this.containerContext.RootScope))
                throw new LifetimeValidationFailedException(requestedType,
                    $"Resolution of scoped {requestedType} from the root scope is not allowed, " +
                    $"that would promote the service's lifetime to singleton.");

            var evaluator = new ScopedEvaluator(requestedType);
            if (Swap.SwapValue(ref this.scopedItems, (t1, t2, t3, t4, items) =>
                 items.AddOrUpdate(t1, t2), key, evaluator, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder))
                return evaluator.Evaluate(this, factory);

            return this.scopedItems.GetOrDefault(key).Evaluate(this, factory);
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
