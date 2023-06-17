using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Registration.Fluent;
using Stashbox.Resolution;

namespace Stashbox.Multitenant;

/// <summary>
/// Represents a tenant distributor that manages tenants in a multi-tenant environment.
/// </summary>
[Obsolete("The functionality of this class was moved to StashboxContainer. Please use StashboxContainer.CreateChildContainer() and StashboxContainer.GetChildContainer() instead.")]
public sealed class TenantDistributor : ITenantDistributor
{
    private int disposed;
    private readonly IStashboxContainer rootContainer;
    /// <summary>
    /// Constructs a <see cref="TenantDistributor"/>.
    /// </summary>
    /// <param name="rootContainer">A pre-configured root container, used to create child tenant containers. If not set, a new will be created.</param>
    public TenantDistributor(IStashboxContainer? rootContainer = null)
    {
        this.rootContainer = rootContainer ?? new StashboxContainer();
    }

    /// <inheritdoc />
    public void ConfigureTenant(object tenantId, Action<IStashboxContainer> tenantConfig, bool attachTenantToRoot = true) =>
        rootContainer.CreateChildContainer(tenantId, tenantConfig, attachTenantToRoot);

    /// <inheritdoc />
    public IDependencyResolver? GetTenant(object tenantId) => rootContainer.GetChildContainer(tenantId);
    /// <inheritdoc />
    public IEnumerable<ReadOnlyKeyValue<object, IStashboxContainer>> ChildContainers => rootContainer.ChildContainers;
    /// <inheritdoc />
    public void RegisterResolver(IResolver resolver) => rootContainer.RegisterResolver(resolver);
    /// <inheritdoc />
    public IStashboxContainer CreateChildContainer(Action<ContainerConfigurator>? config = null, bool attachToParent = true) => rootContainer.CreateChildContainer(config, attachToParent);
    /// <inheritdoc />
    public IStashboxContainer CreateChildContainer(object identifier, Action<IStashboxContainer>? config = null,
        bool attachToParent = true) => rootContainer.CreateChildContainer(identifier, config, attachToParent);

    /// <inheritdoc />
    public IStashboxContainer? GetChildContainer(object identifier) => rootContainer.GetChildContainer(identifier);
    /// <inheritdoc />
    public IContainerContext ContainerContext => rootContainer.ContainerContext;
    /// <inheritdoc />
    public bool IsRegistered<TFrom>(object? name = null) => rootContainer.IsRegistered<TFrom>(name);
    /// <inheritdoc />
    public bool IsRegistered(Type typeFrom, object? name = null) => rootContainer.IsRegistered(typeFrom, name);
    /// <inheritdoc />
    public void Configure(Action<ContainerConfigurator> config) => rootContainer.Configure(config);
    /// <inheritdoc />
    public void Validate() => rootContainer.Validate();
    /// <inheritdoc />
    public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() => rootContainer.GetRegistrationMappings();
    /// <inheritdoc />
    public IEnumerable<RegistrationDiagnosticsInfo> GetRegistrationDiagnostics() => rootContainer.GetRegistrationDiagnostics();
    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
            return;

        this.rootContainer.Dispose();
    }
#if HAS_ASYNC_DISPOSABLE
        /// <inheritdoc />
        public ValueTask DisposeAsync() => Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0 ? new ValueTask(Task.CompletedTask) : this.rootContainer.DisposeAsync();
#endif
    /// <inheritdoc />
    public IStashboxContainer Register<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator) where TFrom : class where TTo : class, TFrom => rootContainer.Register(configurator);
    /// <inheritdoc />
    public IStashboxContainer Register<TFrom, TTo>(object? name = null) where TFrom : class where TTo : class, TFrom => rootContainer.Register<TFrom, TTo>(name);
    /// <inheritdoc />
    public IStashboxContainer Register<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => rootContainer.Register(typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer Register(Type typeFrom, Type typeTo, Action<RegistrationConfigurator>? configurator = null) => rootContainer.Register(typeFrom, typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer Register<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator) where TTo : class => rootContainer.Register<TTo>(configurator);
    /// <inheritdoc />
    public IStashboxContainer Register<TTo>(object? name = null) where TTo : class => rootContainer.Register<TTo>(name);
    /// <inheritdoc />
    public IStashboxContainer Register(Type typeTo, Action<RegistrationConfigurator>? configurator = null) => rootContainer.Register(typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterSingleton<TFrom, TTo>(object? name = null) where TFrom : class where TTo : class, TFrom => rootContainer.RegisterSingleton<TFrom, TTo>(name);
    /// <inheritdoc />
    public IStashboxContainer RegisterSingleton<TTo>(object? name = null) where TTo : class => rootContainer.RegisterSingleton<TTo>(name);
    /// <inheritdoc />
    public IStashboxContainer RegisterSingleton(Type typeFrom, Type typeTo, object? name = null) => rootContainer.RegisterSingleton(typeFrom, typeTo, name);
    /// <inheritdoc />
    public IStashboxContainer RegisterScoped<TFrom, TTo>(object? name = null) where TFrom : class where TTo : class, TFrom => rootContainer.RegisterScoped<TFrom, TTo>(name);
    /// <inheritdoc />
    public IStashboxContainer RegisterScoped(Type typeFrom, Type typeTo, object? name = null) => rootContainer.RegisterScoped(typeFrom, typeTo, name);
    /// <inheritdoc />
    public IStashboxContainer RegisterScoped<TTo>(object? name = null) where TTo : class => rootContainer.RegisterScoped<TTo>(name);
    /// <inheritdoc />
    public IStashboxContainer RegisterInstance<TInstance>(TInstance instance, object? name = null,
        bool withoutDisposalTracking = false, Action<TInstance>? finalizerDelegate = null) where TInstance : class =>
        rootContainer.RegisterInstance(instance, name, withoutDisposalTracking, finalizerDelegate);
    /// <inheritdoc />
    public IStashboxContainer RegisterInstance(object instance, Type serviceType, object? name = null,
        bool withoutDisposalTracking = false) =>
        rootContainer.RegisterInstance(instance, serviceType, name, withoutDisposalTracking);
    /// <inheritdoc />
    public IStashboxContainer WireUp<TInstance>(TInstance instance, object? name = null, bool withoutDisposalTracking = false,
        Action<TInstance>? finalizerDelegate = null) where TInstance : class =>
        rootContainer.WireUp(instance, name, withoutDisposalTracking, finalizerDelegate);
    /// <inheritdoc />
    public IStashboxContainer WireUp(object instance, Type serviceType, object? name = null, bool withoutDisposalTracking = false) => rootContainer.WireUp(instance, serviceType, name, withoutDisposalTracking);
    /// <inheritdoc />
    public object? GetService(Type serviceType) => rootContainer.GetService(serviceType);
    /// <inheritdoc />
    public object Resolve(Type typeFrom) => rootContainer.Resolve(typeFrom);
    /// <inheritdoc />
    public object Resolve(Type typeFrom, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) => rootContainer.Resolve(typeFrom, name, dependencyOverrides, resolutionBehavior);
    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom) => rootContainer.ResolveOrDefault(typeFrom);
    /// <inheritdoc />
    public object? ResolveOrDefault(Type typeFrom, object? name, object[]? dependencyOverrides, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) => rootContainer.ResolveOrDefault(typeFrom, name, dependencyOverrides, resolutionBehavior); 
    /// <inheritdoc />
    public Delegate ResolveFactory(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes) => rootContainer.ResolveFactory(typeFrom, name, resolutionBehavior, parameterTypes);
    /// <inheritdoc />
    public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default, params Type[] parameterTypes) => rootContainer.ResolveFactoryOrDefault(typeFrom, name, resolutionBehavior, parameterTypes);
    /// <inheritdoc />
    public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false) => rootContainer.BeginScope(name, attachToParent);
    /// <inheritdoc />
    public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null) => rootContainer.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking, name);
    /// <inheritdoc />
    public TTo BuildUp<TTo>(TTo instance, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) where TTo : class => rootContainer.BuildUp(instance, resolutionBehavior);
    /// <inheritdoc />
    public object Activate(Type type, ResolutionBehavior resolutionBehavior, params object[] arguments) => rootContainer.Activate(type, resolutionBehavior, arguments);
    /// <inheritdoc />
    public ValueTask InvokeAsyncInitializers(CancellationToken token = default) => rootContainer.InvokeAsyncInitializers(token);
    /// <inheritdoc />
    public bool CanResolve(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) => rootContainer.CanResolve(typeFrom, name);
    /// <inheritdoc />
    public IEnumerable<DelegateCacheEntry> GetDelegateCacheEntries() => rootContainer.GetDelegateCacheEntries();
    /// <inheritdoc />
    public IStashboxContainer ReMap<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>>? configurator = null) where TFrom : class where TTo : class, TFrom => rootContainer.ReMap(configurator);
    /// <inheritdoc />
    public IStashboxContainer ReMap<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => rootContainer.ReMap(typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<RegistrationConfigurator>? configurator = null) => rootContainer.ReMap(typeFrom, typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer ReMap<TTo>(Action<RegistrationConfigurator<TTo, TTo>>? configurator = null) where TTo : class => rootContainer.ReMap<TTo>(configurator);
    /// <inheritdoc />
    public IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator>? configurator = null) => rootContainer.ReMapDecorator(typeFrom, typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer ReMapDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>>? configurator = null) where TFrom : class where TTo : class, TFrom => rootContainer.ReMapDecorator(configurator);
    /// <inheritdoc />
    public IStashboxContainer ReMapDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => rootContainer.ReMapDecorator(typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterTypesAs(Type typeFrom, IEnumerable<Type> types, Func<Type, bool>? selector = null,
        Action<RegistrationConfigurator>? configurator = null) =>
        rootContainer.RegisterTypesAs(typeFrom, types, selector, configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterTypes(IEnumerable<Type> types, Func<Type, bool>? selector = null, Func<Type, Type, bool>? serviceTypeSelector = null,
        bool registerSelf = true, Action<RegistrationConfigurator>? configurator = null) =>
        rootContainer.RegisterTypes(types, selector, serviceTypeSelector, registerSelf, configurator);
    /// <inheritdoc />
    public IStashboxContainer ComposeBy(Type compositionRootType, params object[] compositionRootArguments) => rootContainer.ComposeBy(compositionRootType, compositionRootArguments);
    /// <inheritdoc />
    public IStashboxContainer ComposeBy(ICompositionRoot compositionRoot) => rootContainer.ComposeBy(compositionRoot);
    /// <inheritdoc />
    public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator>? configurator = null) => rootContainer.RegisterDecorator(typeFrom, typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>>? configurator = null) where TFrom : class where TTo : class, TFrom => rootContainer.RegisterDecorator(configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterDecorator(Type typeTo, Action<DecoratorConfigurator>? configurator = null) => rootContainer.RegisterDecorator(typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterDecorator<TTo>(Action<DecoratorConfigurator<TTo, TTo>>? configurator = null) where TTo : class => rootContainer.RegisterDecorator<TTo>(configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => rootContainer.RegisterDecorator(typeTo, configurator);
    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<TService>(Func<IDependencyResolver, TService> factory, object? name = null) => rootContainer.RegisterFunc(factory, name);
    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, TService>(Func<T1, IDependencyResolver, TService> factory, object? name = null) => rootContainer.RegisterFunc(factory, name);
    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, T2, TService>(Func<T1, T2, IDependencyResolver, TService> factory, object? name = null) => rootContainer.RegisterFunc(factory, name);
    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, T2, T3, TService>(Func<T1, T2, T3, IDependencyResolver, TService> factory, object? name = null) => rootContainer.RegisterFunc(factory, name);
    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, T2, T3, T4, TService>(Func<T1, T2, T3, T4, IDependencyResolver, TService> factory, object? name = null) => rootContainer.RegisterFunc(factory, name);
}