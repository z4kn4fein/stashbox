using Stashbox.Exceptions;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Stashbox;

internal sealed partial class ResolutionScope : IResolutionScope
{
    private sealed class ScopedEvaluator
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


    internal DelegateCache DelegateCache;

    public object? Name { get; }

    public IResolutionScope? ParentScope { get; }

    public ResolutionScope(IContainerContext containerContext)
    {
        this.containerContext = containerContext;
        this.delegateCacheProvider = new DelegateCacheProvider();
        this.DelegateCache = new DelegateCache();
    }

    private ResolutionScope(IResolutionScope parent, IContainerContext containerContext, DelegateCacheProvider delegateCacheProvider, object? name)
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
        IRequestContext requestContext, Type serviceType)
    {
        var item = this.scopedInstances.GetOrDefault(key);
        if (item != null) return item.Evaluate(this, requestContext, factory, serviceType);

        var evaluator = new ScopedEvaluator();
        return Swap.SwapValue(ref this.scopedInstances, (t1, t2, _, _, items) =>
            items.AddOrUpdate(t1, t2), key, evaluator, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder)
            ? evaluator.Evaluate(this, requestContext, factory, serviceType)
            : this.scopedInstances.GetOrDefault(key)!.Evaluate(this, requestContext, factory, serviceType);
    }

    public void InvalidateDelegateCache()
    {
        this.ThrowIfDisposed();

        this.delegateCacheProvider.DefaultCache.ServiceDelegates = this.DelegateCache.ServiceDelegates =
            ImmutableTree<Type, ImmutableTree<CacheEntry>>.Empty;
        this.delegateCacheProvider.DefaultCache.RequestContextAwareDelegates = this.DelegateCache.RequestContextAwareDelegates =
            ImmutableTree<Type, ImmutableTree<CacheEntry>>.Empty;
        this.delegateCacheProvider.NamedCache = ImmutableTree<object, DelegateCache>.Empty;
    }

    public IEnumerable<object> GetActiveScopeNames()
    {
        this.ThrowIfDisposed();

        IResolutionScope? current = this;
        while (current != null)
        {
            if (current.Name != null)
                yield return current.Name;

            current = current.ParentScope;
        }
    }
}