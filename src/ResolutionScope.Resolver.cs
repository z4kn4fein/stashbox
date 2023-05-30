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
    public object Resolve(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name);
        return resultFromCachedFactory ?? this.BuildAndResolveService(typeFrom, name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveService(typeFrom, name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name);
        return resultFromCachedFactory ?? this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(serviceType)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this, RequestContext.Empty);

        return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(serviceType)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveServiceOrDefault(serviceType, name: null, dependencyOverrides: null, resolutionBehavior: ResolutionBehavior.Default);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.ServiceFactory;
        if (cachedFactory != null)
            return (IEnumerable<TKey>)cachedFactory(this, RequestContext.Empty);

        return (IEnumerable<TKey>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
                                   this.BuildAndResolveService(type, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior));
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<IEnumerable<TKey>>(type, name);

        return resultFromCachedFactory ??
               (IEnumerable<TKey>)this.BuildAndResolveService(type, name: name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        return (IEnumerable<TKey>)this.BuildAndResolveService(type, name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<TKey>)this.BuildAndResolveService(TypeCache<IEnumerable<TKey>>.Type, name: name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.ServiceFactory;
        if (cachedFactory != null)
            return (IEnumerable<object>)cachedFactory(this, RequestContext.Empty);

        return (IEnumerable<object>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.ServiceFactory?.Invoke(this, RequestContext.Begin()) ??
                                     this.BuildAndResolveService(type, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior));
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        var resultFromCachedFactory = this.GetObjectFromCachedFactoryOrDefault<IEnumerable<object>>(type, name);

        return resultFromCachedFactory ?? (IEnumerable<object>)this.BuildAndResolveService(type, name: name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        return (IEnumerable<object>)this.BuildAndResolveService(type, name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<object>)this.BuildAndResolveService(TypeCache.EnumerableType.MakeGenericType(typeFrom),
            name: name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, object? name = null, params Type[] parameterTypes)
    {
        return ResolveFactory(typeFrom, ResolutionBehavior.Default, name, parameterTypes);
    }

    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, ResolutionBehavior resolutionBehavior, object? name = null, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this, RequestContext.Empty);

        return (Delegate?)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, params Type[] parameterTypes)
    {
        return ResolveFactoryOrDefault(typeFrom, ResolutionBehavior.Default, name, parameterTypes);
    }

    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, ResolutionBehavior resolutionBehavior, object? name = null, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
        var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this, RequestContext.Empty);

        return (Delegate?)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this, RequestContext.Begin()) ??
               this.BuildAndResolveFactoryDelegateOrDefault(typeFrom, parameterTypes, name, key, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public TTo BuildUp<TTo>(TTo instance)
        where TTo : class
    {
        this.ThrowIfDisposed();

        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(),
            this.containerContext, this.ParentScope == null);
        var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), new TypeInformation(TypeCache<TTo>.Type, null));
        return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
            resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty);
    }

    /// <inheritdoc />
    public object Activate(Type type, params object[] arguments)
    {
        return Activate(type, ResolutionBehavior.Default, arguments);
    }

    /// <inheritdoc />
    public object Activate(Type type, ResolutionBehavior resolutionBehavior, params object[] arguments)
    {
        this.ThrowIfDisposed();

        if (!type.IsResolvableType())
            throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, this.ParentScope == null,
            arguments, resolutionBehavior, this.lateKnownInstances);
        var expression = ExpressionFactory.ConstructExpression(resolutionContext, new TypeInformation(type, null));
        return expression?.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
            resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty) ?? throw new ResolutionFailedException(type);
    }

    /// <inheritdoc />
    public bool CanResolve<TFrom>(object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        return this.CanResolve(TypeCache<TFrom>.Type, name, resolutionBehavior);
    }

    /// <inheritdoc />
    public bool CanResolve(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

        return this.containerContext.ResolutionStrategy
            .IsTypeResolvable(ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, false),
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

        return this.DelegateCache.ServiceDelegates?.Walk()?.Select(d =>
            new DelegateCacheEntry(d.Key, d.Value?.ServiceFactory, d.Value?.NamedFactories?.Walk()?.Select(n =>
                new NamedCacheEntry(n.Key, n.Value)))).OrderBy(c => c.ServiceType.FullName) ?? Enumerable.Empty<DelegateCacheEntry>();
    }

    internal object BuildAndResolveService(Type type, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            this.ParentScope == null, dependencyOverrides, resolutionBehavior, this.lateKnownInstances);

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            throw new ResolutionFailedException(type, name);

        var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return name != null
            ? StoreAndInvokeNamedServiceDelegate(type, name, factory, resolutionContext)
            : StoreAndInvokeServiceDelegate(type, factory, resolutionContext);
    }

    internal object? BuildAndResolveServiceOrDefault(Type type, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            this.ParentScope == null, dependencyOverrides, resolutionBehavior, this.lateKnownInstances);

        var serviceContext = this.containerContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            return null;

        var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return name != null
            ? StoreAndInvokeNamedServiceDelegate(type, name, factory, resolutionContext)
            : StoreAndInvokeServiceDelegate(type, factory, resolutionContext);
    }

    internal Delegate BuildAndResolveFactoryDelegate(Type type, Type[] parameterTypes, object? name, string key, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            this.ParentScope == null, initialParameters: parameterTypes.AsParameters(), resolutionBehavior: resolutionBehavior);

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            throw new ResolutionFailedException(type, name);

        var expression = serviceContext.ServiceExpression.AsLambda(resolutionContext.ParameterExpressions
            .SelectMany(x => x.Select(i => i.I2)));

        var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return (Delegate)StoreAndInvokeNamedServiceDelegate(type, key, factory, resolutionContext);
    }

    internal Delegate? BuildAndResolveFactoryDelegateOrDefault(Type type, Type[] parameterTypes, object? name, string key, ResolutionBehavior resolutionBehavior)
    {
        var resolutionContext = ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
            this.ParentScope == null, initialParameters: parameterTypes.AsParameters(), resolutionBehavior: resolutionBehavior);

        var serviceContext = this.containerContext.ResolutionStrategy
            .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
        if (serviceContext.IsEmpty())
            return null;

        var expression = serviceContext.ServiceExpression.AsLambda(resolutionContext.ParameterExpressions
            .SelectMany(x => x.Select(i => i.I2)));

        var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
        return (Delegate)StoreAndInvokeNamedServiceDelegate(type, key, factory, resolutionContext);
    }

    private object StoreAndInvokeServiceDelegate(Type serivceType, Func<IResolutionScope, IRequestContext, object> factory,
        ResolutionContext resolutionContext)
    {
        if (resolutionContext.RequestConfiguration.FactoryDelegateCacheEnabled)
            Swap.SwapValue(ref resolutionContext.RequestConfiguration.RequiresRequestContext
                    ? ref this.DelegateCache.RequestContextAwareDelegates
                    : ref this.DelegateCache.ServiceDelegates, (t1, t2, _, _, c) =>
                    c.AddOrUpdate(t1, new CacheEntry(factory, null), true, (old, _) => new CacheEntry(t2, old.NamedFactories)),
                serivceType, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        return factory(this, resolutionContext.RequestConfiguration.RequiresRequestContext
            ? resolutionContext.RequestContext
            : RequestContext.Empty);
    }

    private object StoreAndInvokeNamedServiceDelegate(Type serivceType, object key, Func<IResolutionScope, IRequestContext, object> factory,
        ResolutionContext resolutionContext)
    {
        if (resolutionContext.RequestConfiguration.FactoryDelegateCacheEnabled)
            Swap.SwapValue(ref resolutionContext.RequestConfiguration.RequiresRequestContext
                ? ref this.DelegateCache.RequestContextAwareDelegates
                : ref this.DelegateCache.ServiceDelegates, (t1, t2, t3, _, c) =>
            {
                var newEntry = new CacheEntry(null, ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty.AddOrUpdate(t2, t3, false));
                return c.AddOrUpdate(t1, newEntry, true, (old, _) =>
                {
                    if (old.NamedFactories != null)
                        return new CacheEntry(old.ServiceFactory, old.NamedFactories.AddOrUpdate(t2, t3, false));

                    return new CacheEntry(old.ServiceFactory, ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty.AddOrUpdate(t2, t3, false));
                });
            }, serivceType, key, factory, Constants.DelegatePlaceholder);

        return factory(this, resolutionContext.RequestConfiguration.RequiresRequestContext
            ? resolutionContext.RequestContext
            : RequestContext.Empty);
    }

    internal T? GetObjectFromCachedFactoryOrDefault<T>(Type type, object? name)
    {
        var cachedFactory = name == null
            ? this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.ServiceFactory
            : this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.NamedFactories?.GetOrDefaultByValue(name);

        if (cachedFactory != null)
            return (T?)cachedFactory(this, RequestContext.Empty);

        var requestContextAwareFactory = name == null
            ? this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.ServiceFactory
            : this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.NamedFactories?.GetOrDefaultByValue(name);

        return (T?)requestContextAwareFactory?.Invoke(this, RequestContext.Begin());
    }
}