using Stashbox.Exceptions;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Stashbox
{
    internal sealed partial class ResolutionScope : IResolutionScope
    {
        private class ScopedEvaluator
        {
            private const int MaxWaitTimeInMs = 3000;
            private static readonly object Default = new();
            private int evaluated;
            private object evaluatedObject = Default;

            public object Evaluate(IResolutionScope scope, IRequestContext requestContext, Func<IResolutionScope, IRequestContext, object> factory, Type serviceType)
            {
                if (!ReferenceEquals(this.evaluatedObject, Default))
                    return this.evaluatedObject;

                if (Interlocked.CompareExchange(ref this.evaluated, 1, 0) != 0)
                    return this.WaitForEvaluation(serviceType);

                this.evaluatedObject = factory(scope, requestContext);
                return this.evaluatedObject;
            }

            private object WaitForEvaluation(Type serviceType)
            {
                SpinWait spin = default;
                var startTime = (uint)Environment.TickCount;
                while (ReferenceEquals(this.evaluatedObject, Default))
                {
                    if (spin.NextSpinWillYield)
                    {
                        var currentTime = (uint)Environment.TickCount;
                        if (MaxWaitTimeInMs <= currentTime - startTime)
                            throw new ResolutionFailedException(serviceType,
                                $"The service {serviceType} was unavailable after {MaxWaitTimeInMs} ms. " +
                                "It's possible that the thread used to construct it crashed by a handled exception." +
                                "This exception is supposed to prevent other caller threads from infinite waiting for service construction.");
                    }

                    spin.SpinOnce();
                }

                return this.evaluatedObject;
            }
        }

        private int disposed;
        private readonly IContainerContext containerContext;
        private readonly DelegateCacheProvider delegateCacheProvider;
        private ImmutableTree<ScopedEvaluator> scopedInstances = ImmutableTree<ScopedEvaluator>.Empty;
        private ImmutableTree<object, object> lateKnownInstances = ImmutableTree<object, object>.Empty;
        private ImmutableTree<ThreadLocal<bool>> circularDependencyBarrier = ImmutableTree<ThreadLocal<bool>>.Empty;


        internal readonly DelegateCache DelegateCache;

        public object Name { get; }

        public IResolutionScope ParentScope { get; }

        public ResolutionScope(IContainerContext containerContext)
        {
            this.containerContext = containerContext;
            this.delegateCacheProvider = new DelegateCacheProvider();
            this.DelegateCache = new DelegateCache();
        }

        private ResolutionScope(IResolutionScope parent, IContainerContext containerContext, DelegateCacheProvider delegateCacheProvider, object name)
        {
            this.containerContext = containerContext;
            this.delegateCacheProvider = delegateCacheProvider;
            this.ParentScope = parent;
            this.Name = name;
            this.DelegateCache = name == null
                ? delegateCacheProvider.DefaultCache
                : delegateCacheProvider.GetNamedCache(name);
        }

        public object GetOrAddScopedObject(int key, Func<IResolutionScope, IRequestContext, object> factory,
            IRequestContext requestContext, Type requestedType)
        {
            this.ThrowIfDisposed();

            var item = this.scopedInstances.GetOrDefault(key);
            if (item != null) return item.Evaluate(this, requestContext, factory, requestedType);

            var evaluator = new ScopedEvaluator();
            return Swap.SwapValue(ref this.scopedInstances, (t1, t2, _, _, items) =>
                items.AddOrUpdate(t1, t2), key, evaluator, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder)
                ? evaluator.Evaluate(this, requestContext, factory, requestedType)
                : this.scopedInstances.GetOrDefault(key).Evaluate(this, requestContext, factory, requestedType);
        }

        public void InvalidateDelegateCache()
        {
            this.ThrowIfDisposed();

            this.delegateCacheProvider.DefaultCache.ServiceDelegates = this.DelegateCache.ServiceDelegates = 
                ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty;
            this.delegateCacheProvider.DefaultCache.RequestContextAwareDelegates = this.DelegateCache.RequestContextAwareDelegates = 
                ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty;
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
            if (check is { Value: true })
                throw new CircularDependencyException(type);

            Swap.SwapValue(ref this.circularDependencyBarrier, (t1, _, _, _, barrier) => barrier.AddOrUpdate(t1, new ThreadLocal<bool>(), (old, @new) =>
                { old.Value = true; return old; }), key, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
        }

        public void ResetRuntimeCircularDependencyBarrier(int key)
        {
            var check = this.circularDependencyBarrier.GetOrDefault(key);
            if (check != null)
                check.Value = false;
        }

        internal HashTree<object, ConstantExpression> ProcessDependencyOverrides(object[] dependencyOverrides)
        {
            if (dependencyOverrides == null && this.lateKnownInstances.IsEmpty)
                return null;

            var result = new HashTree<object, ConstantExpression>();

            if (!this.lateKnownInstances.IsEmpty)
                foreach (var lateKnownInstance in this.lateKnownInstances.Walk())
                    result.Add(lateKnownInstance.Key, lateKnownInstance.Value.AsConstant(), false);

            if (dependencyOverrides == null) return result;

            foreach (var dependencyOverride in dependencyOverrides)
            {
                var type = dependencyOverride.GetType();
                var expression = dependencyOverride.AsConstant();

                result.Add(type, expression, false);

                foreach (var baseType in type.GetRegisterableInterfaceTypes().Concat(type.GetRegisterableBaseTypes()))
                    result.Add(baseType, expression, false);
            }

            return result;
        }
    }
}
