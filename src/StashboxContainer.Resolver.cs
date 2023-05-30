using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        return this.rootScope.Resolve(typeFrom, resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.Resolve(typeFrom, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.Resolve(typeFrom, name, resolutionBehavior);
    }

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.Resolve(typeFrom, name, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveOrDefault(typeFrom, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveOrDefault(typeFrom, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveOrDefault(typeFrom, name, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveOrDefault(typeFrom, name, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType)
    {
        this.ThrowIfDisposed();

        return this.rootScope.GetService(serviceType);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll<TKey>(resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll<TKey>(name, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll<TKey>(dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<TKey> ResolveAll<TKey>(object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll<TKey>(name, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll(typeFrom, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll(typeFrom, name, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll(typeFrom, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public IEnumerable<object> ResolveAll(Type typeFrom, object? name, object[] dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveAll(typeFrom, name, dependencyOverrides, resolutionBehavior);
    }

    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, object? name = null, params Type[] parameterTypes)
    {
        return this.rootScope.ResolveFactory(typeFrom, name, parameterTypes);
    }

    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, ResolutionBehavior resolutionBehavior, object? name = null, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveFactory(typeFrom, resolutionBehavior, name, parameterTypes);
    }

    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, params Type[] parameterTypes)
    {
        return this.rootScope.ResolveFactoryOrDefault(typeFrom, name, parameterTypes);
    }

    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, ResolutionBehavior resolutionBehavior, object? name = null, params Type[] parameterTypes)
    {
        this.ThrowIfDisposed();

        return this.rootScope.ResolveFactoryOrDefault(typeFrom, resolutionBehavior, name, parameterTypes);
    }

    /// <inheritdoc />
    public TTo BuildUp<TTo>(TTo instance)
        where TTo : class
    {
        this.ThrowIfDisposed();

        return this.rootScope.BuildUp<TTo>(instance);
    }

    /// <inheritdoc />
    public object Activate(Type type, params object[] arguments)
    {
        this.ThrowIfDisposed();

        return this.rootScope.Activate(type, arguments);
    }

    /// <inheritdoc />
    public object Activate(Type type, ResolutionBehavior resolutionBehavior, params object[] arguments)
    {
        this.ThrowIfDisposed();

        return this.rootScope.Activate(type, resolutionBehavior, arguments);
    }

    /// <inheritdoc />
    public bool CanResolve<TFrom>(object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.CanResolve<TFrom>(name, resolutionBehavior);
    }

    /// <inheritdoc />
    public bool CanResolve(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.CanResolve(typeFrom, name, resolutionBehavior);
    }

    /// <inheritdoc />
    public ValueTask InvokeAsyncInitializers(CancellationToken token = default)
    {
        this.ThrowIfDisposed();

        return this.rootScope.InvokeAsyncInitializers(token);
    }

    /// <inheritdoc />
    public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false)
    {
        this.ThrowIfDisposed();

        return this.rootScope.BeginScope(name, attachToParent);
    }

    /// <inheritdoc />
    public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null)
    {
        this.ThrowIfDisposed();

        this.rootScope.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking, name);
    }

    /// <inheritdoc />
    public IEnumerable<DelegateCacheEntry> GetDelegateCacheEntries()
    {
        this.ThrowIfDisposed();

        return this.rootScope.GetDelegateCacheEntries();
    }
}