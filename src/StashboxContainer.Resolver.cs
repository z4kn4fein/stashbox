using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox;

public partial class StashboxContainer
{
    /// <inheritdoc />
    public object Resolve(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this.rootScope, RequestContext.Empty);

        return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory?.Invoke(this.rootScope, RequestContext.Begin()) ??
               this.rootScope.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var resultFromCachedFactory = this.rootScope.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name);
        return resultFromCachedFactory ?? this.rootScope.BuildAndResolveService(typeFrom, name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.BuildAndResolveService(typeFrom, name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this.rootScope, RequestContext.Empty);

        return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.ServiceFactory?.Invoke(this.rootScope, RequestContext.Begin()) ??
               this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var resultFromCachedFactory = this.rootScope.GetObjectFromCachedFactoryOrDefault<object>(typeFrom, name);
        return resultFromCachedFactory ?? this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType)
    {
        this.ThrowIfDisposed();

        var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(serviceType)?.ServiceFactory;
        if (cachedFactory != null)
            return cachedFactory(this.rootScope, RequestContext.Empty);

        return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(serviceType)?.ServiceFactory?.Invoke(this.rootScope, RequestContext.Begin()) ??
               this.rootScope.BuildAndResolveServiceOrDefault(serviceType, name: null, dependencyOverrides: null, resolutionBehavior: ResolutionBehavior.Default);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.ServiceFactory;
        if (cachedFactory != null)
            return (IEnumerable<TKey>)cachedFactory(this.rootScope, RequestContext.Empty);

        return (IEnumerable<TKey>)(this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.ServiceFactory?.Invoke(this.rootScope, RequestContext.Begin()) ??
                                   this.rootScope.BuildAndResolveService(type, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior));
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache<IEnumerable<TKey>>.Type;
        var resultFromCachedFactory = this.rootScope.GetObjectFromCachedFactoryOrDefault<IEnumerable<TKey>>(type, name);
        return resultFromCachedFactory ??
               (IEnumerable<TKey>)this.rootScope.BuildAndResolveService(type, name: name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<TKey>)this.rootScope.BuildAndResolveService(TypeCache<IEnumerable<TKey>>.Type, name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<TKey>)this.rootScope.BuildAndResolveService(TypeCache<IEnumerable<TKey>>.Type, name: name, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type)?.ServiceFactory;
        if (cachedFactory != null)
            return (IEnumerable<object>)cachedFactory(this.rootScope, RequestContext.Empty);

        return (IEnumerable<object>)(this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.ServiceFactory?.Invoke(this.rootScope, RequestContext.Begin()) ??
                                     this.rootScope.BuildAndResolveService(type, name: null, dependencyOverrides: null, resolutionBehavior: resolutionBehavior));
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        var type = TypeCache.EnumerableType.MakeGenericType(typeFrom);
        var resultFromCachedFactory = this.rootScope.GetObjectFromCachedFactoryOrDefault<IEnumerable<object>>(type, name);
        return resultFromCachedFactory ??
               (IEnumerable<object>)this.rootScope.BuildAndResolveService(type, name: name, dependencyOverrides: null, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<object>)this.rootScope.BuildAndResolveService(TypeCache.EnumerableType.MakeGenericType(typeFrom),
            name: null, dependencyOverrides: dependencyOverrides, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return (IEnumerable<object>)this.rootScope.BuildAndResolveService(TypeCache.EnumerableType.MakeGenericType(typeFrom),
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
        var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this.rootScope, RequestContext.Empty);

        return (Delegate?)this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this.rootScope, RequestContext.Begin()) ??
               this.rootScope.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key, resolutionBehavior: resolutionBehavior);
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
        var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key);
        if (cachedFactory != null)
            return (Delegate)cachedFactory(this.rootScope, RequestContext.Empty);

        return (Delegate?)this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.NamedFactories?.GetOrDefaultByValue(key)?.Invoke(this.rootScope, RequestContext.Begin()) ??
               this.rootScope.BuildAndResolveFactoryDelegateOrDefault(typeFrom, parameterTypes, name, key, resolutionBehavior: resolutionBehavior);
    }

    /// <inheritdoc />
    public TTo BuildUp<TTo>(TTo instance)
        where TTo : class
    {
        this.ThrowIfDisposed();

        var resolutionContext = ResolutionContext.BeginTopLevelContext(this.rootScope.GetActiveScopeNames(),
            this.ContainerContext, true);
        var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), new TypeInformation(TypeCache<TTo>.Type, null));
        return (TTo)expression.CompileDelegate(resolutionContext, this.ContainerContext.ContainerConfiguration)(this.rootScope,
            resolutionContext.RequestConfiguration.RequiresRequestContext ? RequestContext.Begin() : RequestContext.Empty);
    }

    /// <inheritdoc />
    public object Activate(Type type, params object[] arguments) =>
        Activate(type, ResolutionBehavior.Default, arguments);

    /// <inheritdoc />
    public object Activate(Type type, ResolutionBehavior resolutionBehavior, params object[] arguments) =>
        this.rootScope.Activate(type, resolutionBehavior, arguments);

    /// <inheritdoc />
    public bool CanResolve<TFrom>(object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        this.CanResolve(TypeCache<TFrom>.Type, name, resolutionBehavior);

    /// <inheritdoc />
    public bool CanResolve(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

        return this.ContainerContext.ResolutionStrategy
            .IsTypeResolvable(ResolutionContext.BeginTopLevelContext(this.rootScope.GetActiveScopeNames(), this.ContainerContext, false),
                new TypeInformation(typeFrom, name));
    }

    /// <inheritdoc />
    public ValueTask InvokeAsyncInitializers(CancellationToken token = default) =>
        this.rootScope.InvokeAsyncInitializers(token);

    /// <inheritdoc />
    public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false)
        => this.rootScope.BeginScope(name, attachToParent);

    /// <inheritdoc />
    public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null) =>
        this.rootScope.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking, name);

    /// <inheritdoc />
    public IEnumerable<DelegateCacheEntry> GetDelegateCacheEntries() =>
        this.rootScope.GetDelegateCacheEntries();
}