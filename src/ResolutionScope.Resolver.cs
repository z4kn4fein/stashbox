using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox;

internal partial class ResolutionScope
{
    /// <inheritdoc />
    public object Resolve(Type typeFrom)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null, Constants.DefaultResolutionBehavior);
    }
    
    /// <inheritdoc />
    public object Resolve(Type typeFrom, ResolutionBehavior resolutionBehavior)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name, resolutionBehavior);
        return resultFromCachedFactory ?? this.BuildAndResolveService(typeFrom, name, dependencyOverrides: null, resolutionBehavior);
    }


    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveService(typeFrom, name, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null, Constants.DefaultResolutionBehavior);
    }
    
    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, ResolutionBehavior resolutionBehavior)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name, resolutionBehavior);
        return resultFromCachedFactory ?? this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: null, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(serviceType)?
            .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(serviceType)?
                   .GetOrDefault(Constants.DefaultResolutionBehaviorInt)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveServiceOrDefault(serviceType, name: null, dependencyOverrides: null, Constants.DefaultResolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?
            .GetOrDefault((int)resolutionBehavior)?.ServiceFactory;
        if (cachedFactory != null)
            return (IEnumerable<TKey>)cachedFactory(this, RequestContext.Empty);

        return (IEnumerable<TKey>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?
                                       .GetOrDefault((int)resolutionBehavior)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
                                   this.BuildAndResolveService(type, name: null, dependencyOverrides: null, resolutionBehavior));
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<IEnumerable<TKey>>(type, name, resolutionBehavior);

        return resultFromCachedFactory ??
               (IEnumerable<TKey>)this.BuildAndResolveService(type, name: name, dependencyOverrides: null, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        return (IEnumerable<TKey>)this.BuildAndResolveService(type, name: null, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<TKey>)this.BuildAndResolveService(TypeCache<IEnumerable<TKey>>.Type, name: name, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?
            .GetOrDefault((int)resolutionBehavior)?.ServiceFactory;
        if (cachedFactory != null)
            return (IEnumerable<object>)cachedFactory(this, RequestContext.Empty);

        return (IEnumerable<object>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?
                                         .GetOrDefault((int)resolutionBehavior)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
                                     this.BuildAndResolveService(type, name: null, dependencyOverrides: null, resolutionBehavior));
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<IEnumerable<object>>(type, name, resolutionBehavior);

        return resultFromCachedFactory ?? (IEnumerable<object>)this.BuildAndResolveService(type, name: name, dependencyOverrides: null, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        return (IEnumerable<object>)this.BuildAndResolveService(type, name: null, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<object>)this.BuildAndResolveService(TypeCache.EnumerableType.MakeGenericType(typeFrom),
            name: name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this, RequestContext.Empty);

        return (Delegate?)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key, resolutionBehavior);
    }

    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?
            .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this, RequestContext.Empty);

        return (Delegate?)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?
                   .GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveFactoryDelegateOrDefault(typeFrom, parameterTypes, name, key, resolutionBehavior);
    }

    /// <inheritdoc />
    public TTo BuildUp<TTo>(TTo instance, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
        where TTo : class
    {
        this.ThrowIfDisposed();

        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(),
            this.containerContext, resolutionBehavior, this.ParentScope == null);
        var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), new TypeInformation(TypeCache<TTo>.Type, null));
        return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
            resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty);
    }

    /// <inheritdoc />
    public object Activate(Type type, params object[] arguments) =>
        this.Activate(type, Constants.DefaultResolutionBehavior, arguments);
    
    /// <inheritdoc />
    public object Activate(Type type, ResolutionBehavior resolutionBehavior, params object[] arguments)
    {
        this.ThrowIfDisposed();

        if (!type.IsResolvableType())
            throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, resolutionBehavior, this.ParentScope == null,
            arguments, this.lateKnownInstances);
        var expression = ExpressionFactory.ConstructExpression(resolutionContext, new TypeInformation(type, null));
        return expression?.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
            resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty) ?? throw new ResolutionFailedException(type);
    }

    /// <inheritdoc />
    public bool CanResolve<TFrom>(object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        this.CanResolve(TypeCache<TFrom>.Type, name);

    /// <inheritdoc />
    public bool CanResolve(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

        return this.containerContext.ResolutionStrategy
            .IsTypeResolvable(ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, resolutionBehavior, false),
                new TypeInformation(typeFrom, name));
    }

    /// <inheritdoc />
    public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false)
    {
        this.ThrowIfDisposed();

        var scope = new ResolutionScope(this, this.containerContext, this.delegateCacheProvider, name);

        if (attachToParent)
            this.AddDisposableTracking(scope);

        return scope;
    }

    /// <inheritdoc />
    public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null)
    {
        this.ThrowIfDisposed();

        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
        Shield.EnsureNotNull(instance, nameof(instance));

        var key = name ?? typeFrom;
        Swap.SwapValue(ref this.lateKnownInstances, (t1, t2, _, _, instances) =>
            instances.AddOrUpdate(t1, t2, false), key, instance, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        if (!withoutDisposalTracking && instance is IDisposable disposable)
            this.AddDisposableTracking(disposable);

        this.DelegateCache = new DelegateCache();
    }

    /// <inheritdoc />
    public IEnumerable<DelegateCacheEntry> GetDelegateCacheEntries()
    {
        this.ThrowIfDisposed();

        return this.DelegateCache.ServiceDelegates?.Walk()?.SelectMany(d => d.Value.Walk().Select(e =>
            new DelegateCacheEntry(d.Key, e.Value?.ServiceFactory, e.Value?.NamedFactories?.Walk()?.Select(n =>
                new NamedCacheEntry(n.Key, n.Value)), (ResolutionBehavior)e.Key)).OrderBy(c => c.ServiceType.FullName)) ?? Enumerable.Empty<DelegateCacheEntry>();
    }

    internal object BuildAndResolveService(Type type, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, dependencyOverrides, this.lateKnownInstances);

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            throw new ResolutionFailedException(type, name);

        var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return name != null
            ? StoreAndInvokeNamedServiceDelegate(type, name, factory, resolutionContext, resolutionBehavior)
            : StoreAndInvokeServiceDelegate(type, factory, resolutionContext, resolutionBehavior);
    }

    internal object? BuildAndResolveServiceOrDefault(Type type, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, dependencyOverrides, this.lateKnownInstances);

        var serviceContext = this.containerContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            return null;

        var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return name != null 
            ? StoreAndInvokeNamedServiceDelegate(type, name, factory, resolutionContext, resolutionBehavior)
            : StoreAndInvokeServiceDelegate(type, factory, resolutionContext, resolutionBehavior);
    }

    internal Delegate BuildAndResolveFactoryDelegate(Type type, Type[] parameterTypes, object? name, string key, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, initialParameters: parameterTypes.AsParameters());

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            throw new ResolutionFailedException(type, name);

        var expression = serviceContext.ServiceExpression.AsLambda(resolutionContext.ParameterExpressions
            .SelectMany(x => x.Select(i => i.I2)));

        var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return (Delegate)StoreAndInvokeNamedServiceDelegate(type, key, factory, resolutionContext, resolutionBehavior);
    }

    internal Delegate? BuildAndResolveFactoryDelegateOrDefault(Type type, Type[] parameterTypes, object? name, string key, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            resolutionBehavior, this.ParentScope == null, initialParameters: parameterTypes.AsParameters());

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
                    ? ref this.DelegateCache.RequestContextAwareDelegates
                    : ref this.DelegateCache.ServiceDelegates, (t1, t2, t3, _, c) =>
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
                ? ref this.DelegateCache.RequestContextAwareDelegates
                : ref this.DelegateCache.ServiceDelegates, (t1, t2, t3, t4, c) =>
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

    internal T? GetObjectFromCachedFactoryOrDefault<T>(Type type, object? name, ResolutionBehavior resolutionBehavior)
    {
        var cachedFactory = name == null
            ? this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.ServiceFactory
            : this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(name);

        if (cachedFactory != null)
            return (T?)cachedFactory(this, RequestContext.Empty);

        var requestContextAwareFactory = name == null
            ? this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.ServiceFactory
            : this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.GetOrDefault((int)resolutionBehavior)?.NamedFactories?.GetOrDefaultByValue(name);

        return (T?)requestContextAwareFactory?.Invoke(this, RequestContext.Begin());
    }
}