using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.Utils.Data;

namespace Stashbox;

internal partial class ResolutionScope
{
    /// <inheritdoc />
    public object Resolve(Type typeFrom)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null, Constants.DefaultResolutionBehavior);
    }
    
    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        if (dependencyOverrides != null) 
            return this.BuildAndResolveService(typeFrom, name, dependencyOverrides, resolutionBehavior);
        
        if (name != null)
        {
            var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name, resolutionBehavior);
            return resultFromCachedFactory ?? this.BuildAndResolveService(typeFrom, name, dependencyOverrides: null, resolutionBehavior);
        }

        var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null, Constants.DefaultResolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        if (dependencyOverrides != null)
            return this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides, resolutionBehavior);
        
        if (name != null)
        {
            var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name, resolutionBehavior);
            return resultFromCachedFactory ?? this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: null, resolutionBehavior);
        }
        
        var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType) => this.ResolveOrDefault(serviceType);

    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
        var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this, RequestContext.Empty);

        return (Delegate?)this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key, resolutionBehavior);
    }

    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
        var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this, RequestContext.Empty);

        return (Delegate?)this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveFactoryDelegateOrDefault(typeFrom, parameterTypes, name, key, resolutionBehavior);
    }

    /// <inheritdoc />
    public TTo BuildUp<TTo>(TTo instance, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
        where TTo : class
    {
        this.ThrowIfDisposed();

        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(),
            this.containerContext, resolutionBehavior, this.ParentScope == null, false);
        var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), new TypeInformation(TypeCache<TTo>.Type, null));
        return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
            resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty);
    }

    /// <inheritdoc />
    public object Activate(Type type, ResolutionBehavior resolutionBehavior, params object[] arguments)
    {
        this.ThrowIfDisposed();

        if (!type.IsResolvableType())
            throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, resolutionBehavior, this.ParentScope == null,
            false, arguments, this.lateKnownInstances);
        var expression = ExpressionFactory.ConstructExpression(resolutionContext, new TypeInformation(type, null));
        return expression?.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
            resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty) ?? throw new ResolutionFailedException(type);
    }

    /// <inheritdoc />
    public bool CanResolve(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

        return this.containerContext.ResolutionStrategy
            .IsTypeResolvable(ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, resolutionBehavior, false, false),
                new TypeInformation(typeFrom, name));
    }

    /// <inheritdoc />
    public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false)
    {
        this.ThrowIfDisposed();

        var scope = new ResolutionScope(this, this.containerContext, this.delegateCacheProvider, name, attachToParent);

        if (attachToParent)
            Swap.SwapValue(ref this.childScopes, (t1, _, _, _, childStore) =>
                    childStore.AddOrSkip(t1),
                scope, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        return scope;
    }

    /// <inheritdoc />
    public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null)
    {
        this.ThrowIfDisposed();

        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
        Shield.EnsureNotNull(instance, nameof(instance));
        
        var @override = Override.Of(typeFrom, instance, name);

        Swap.SwapValue(ref this.lateKnownInstances, (t1, t2, t3, _, instances) =>
            instances.AddOrUpdate(t1, t2, false, (o, _) => 
                o.Add(t3)), typeFrom, new ImmutableBucket<Override>([@override]), 
            @override, Constants.DelegatePlaceholder);

        if (!withoutDisposalTracking && instance is IDisposable disposable)
            this.AddDisposableTracking(disposable);

        this.delegateCache = new DelegateCache();
    }

    /// <inheritdoc />
    public IEnumerable<DelegateCacheEntry> GetDelegateCacheEntries()
    {
        this.ThrowIfDisposed();

        return this.delegateCache.ServiceDelegates.Walk().SelectMany(d => d.Value.Walk().Select(e =>
            new DelegateCacheEntry(d.Key, e.Value.ServiceFactory, e.Value.NamedFactories?.Walk().Select(n =>
                new NamedCacheEntry(n.Key, n.Value)), (ResolutionBehavior)e.Key)).OrderBy(c => c.ServiceType.FullName));
    }

    private object BuildAndResolveService(Type type, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, nullResultAllowed: false, dependencyOverrides, this.lateKnownInstances);

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            throw new ResolutionFailedException(type, name);

        var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return name != null
            ? StoreAndInvokeNamedServiceDelegate(type, name, factory, resolutionContext, resolutionBehavior)
            : StoreAndInvokeServiceDelegate(type, factory, resolutionContext, resolutionBehavior);
    }

    private object? BuildAndResolveServiceOrDefault(Type type, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, nullResultAllowed: true, dependencyOverrides, this.lateKnownInstances);

        var serviceContext = this.containerContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            return null;

        var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return name != null 
            ? StoreAndInvokeNamedServiceDelegate(type, name, factory, resolutionContext, resolutionBehavior)
            : StoreAndInvokeServiceDelegate(type, factory, resolutionContext, resolutionBehavior);
    }

    private Delegate BuildAndResolveFactoryDelegate(Type type, Type[] parameterTypes, object? name, string key, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, nullResultAllowed: false, initialParameters: parameterTypes.AsParameters());

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            throw new ResolutionFailedException(type, name);

        var expression = serviceContext.ServiceExpression.AsLambda(resolutionContext.ParameterExpressions
            .SelectMany(x => x.Select(i => i.I2)));

        var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return (Delegate)StoreAndInvokeNamedServiceDelegate(type, key, factory, resolutionContext, resolutionBehavior);
    }

    private Delegate? BuildAndResolveFactoryDelegateOrDefault(Type type, Type[] parameterTypes, object? name, string key, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, nullResultAllowed: true, initialParameters: parameterTypes.AsParameters());

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            return null;

        var expression = serviceContext.ServiceExpression.AsLambda(resolutionContext.ParameterExpressions
            .SelectMany(x => x.Select(i => i.I2)));

        var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return (Delegate)StoreAndInvokeNamedServiceDelegate(type, key, factory, resolutionContext, resolutionBehavior);
    }

    private object StoreAndInvokeServiceDelegate(Type serviceType,
        Func<IResolutionScope, IRequestContext, object> factory,
        ResolutionContext resolutionContext, ResolutionBehavior resolutionBehavior)
    {
        if (resolutionContext.RequestConfiguration.FactoryDelegateCacheEnabled)
        {
            
            Swap.SwapValue(ref resolutionContext.RequestConfiguration.RequiresRequestContext
                    ? ref this.delegateCache.RequestContextAwareDelegates
                    : ref this.delegateCache.ServiceDelegates, (t1, t2, t3, _, c) =>
                {
                    var newEntry = new CacheEntry(t2, null);
                    var tree = ImmutableTree<CacheEntry>.Empty.AddOrUpdate((int)t3, newEntry);
                    return c.AddOrUpdate(t1, tree, true,
                        (old, _) => old.AddOrUpdate((int)t3, newEntry, (oldEntry, _) => new CacheEntry(t2, oldEntry.NamedFactories)));
                },
            serviceType, factory, resolutionBehavior, Constants.DelegatePlaceholder);
        }

        return factory(this, resolutionContext.RequestConfiguration.RequiresRequestContext
            ? resolutionContext.RequestContext
            : RequestContext.Empty);
    }

    private object StoreAndInvokeNamedServiceDelegate(Type serviceType, object key, Func<IResolutionScope, IRequestContext, object> factory,
        ResolutionContext resolutionContext, ResolutionBehavior resolutionBehavior)
    {
        if (resolutionContext.RequestConfiguration.FactoryDelegateCacheEnabled)
            Swap.SwapValue(ref resolutionContext.RequestConfiguration.RequiresRequestContext
                ? ref this.delegateCache.RequestContextAwareDelegates
                : ref this.delegateCache.ServiceDelegates, (t1, t2, t3, t4, c) =>
            {
                var newEntry = new CacheEntry(null, ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty.AddOrUpdate(t2, t3, false));
                var tree = ImmutableTree<CacheEntry>.Empty.AddOrUpdate((int)t4, newEntry);
                return c.AddOrUpdate(t1, tree, true, (old, _) => old.AddOrUpdate((int)t4, newEntry, (oldEntry, _) => oldEntry.NamedFactories != null
                    ? new CacheEntry(oldEntry.ServiceFactory, oldEntry.NamedFactories.AddOrUpdate(t2, t3, false))
                    : new CacheEntry(oldEntry.ServiceFactory,
                        ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty.AddOrUpdate(t2,
                            t3, false))));
            }, serviceType, key, factory, resolutionBehavior);

        return factory(this, resolutionContext.RequestConfiguration.RequiresRequestContext
            ? resolutionContext.RequestContext
            : RequestContext.Empty);
    }

    private T? GetObjectFromCachedFactoryOrDefault<T>(Type type, object? name, ResolutionBehavior resolutionBehavior)
    {
        var cachedFactory = name == null
            ? this.delegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.ServiceFactory
            : this.delegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(name);

        if (cachedFactory != null)
            return (T?)cachedFactory(this, RequestContext.Empty);

        var requestContextAwareFactory = name == null
            ? this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.ServiceFactory
            : this.delegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(name);

        return (T?)requestContextAwareFactory?.Invoke(this, RequestContext.Begin());
    }
}