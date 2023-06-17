using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox;

public partial class StashboxContainer
{
    /// <inheritdoc />
    public object Resolve(Type typeFrom) => this.rootScope.Resolve(typeFrom);

    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, object[]? dependencyOverrides,
        ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        this.rootScope.Resolve(typeFrom, name, dependencyOverrides, resolutionBehavior);
    
    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom) =>
        this.rootScope.ResolveOrDefault(typeFrom);

    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, object[]? dependencyOverrides,
        ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        this.rootScope.ResolveOrDefault(typeFrom, name, dependencyOverrides, resolutionBehavior);
    
    /// <inheritdoc />
    public object? GetService(Type serviceType) => this.rootScope.ResolveOrDefault(serviceType);

    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, object? name = null,
        ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes) =>
        this.rootScope.ResolveFactory(typeFrom, name, resolutionBehavior, parameterTypes);
    
    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null,
        ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes) =>
        this.rootScope.ResolveFactoryOrDefault(typeFrom, name, resolutionBehavior, parameterTypes);

    /// <inheritdoc />
    public TTo BuildUp<TTo>(TTo instance, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
        where TTo : class => this.rootScope.BuildUp(instance, resolutionBehavior);

    /// <inheritdoc />
    public object Activate(Type type, ResolutionBehavior resolutionBehavior, params object[] arguments) =>
        this.rootScope.Activate(type, resolutionBehavior, arguments);

    /// <inheritdoc />
    public bool CanResolve(Type typeFrom, object? name = null,
        ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        this.rootScope.CanResolve(typeFrom, name, resolutionBehavior);

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