using Stashbox.Utils;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Registration.Fluent;
using Stashbox.Resolution;

namespace Stashbox.Multitenant
{
    /// <summary>
    /// Represents a tenant distributor that manages tenants in a multi-tenant environment.
    /// </summary>
    public sealed class TenantDistributor : ITenantDistributor, IStashboxContainer
    {
        private int disposed;
        private ImmutableTree<object, IStashboxContainer> tenantRepository = ImmutableTree<object, IStashboxContainer>.Empty;

        /// <inheritdoc />
        public IStashboxContainer RootContainer { get; }

        /// <summary>
        /// Constructs a <see cref="TenantDistributor"/>.
        /// </summary>
        /// <param name="rootContainer">A pre-configured root container, used to create child tenant containers. If not set, a new will be created.</param>
        public TenantDistributor(IStashboxContainer? rootContainer = null)
        {
            this.RootContainer = rootContainer ?? new StashboxContainer();
        }

        /// <inheritdoc />
        public void ConfigureTenant(object tenantId, Action<IStashboxContainer> tenantConfig)
        {
            var tenantContainer = this.CreateChildContainer();

            if (!Swap.SwapValue(ref this.tenantRepository,
                (id, container, _, _, repo) => repo.AddOrUpdate(id, container, false, false),
                tenantId,
                tenantContainer,
                Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder)) return;
            tenantConfig(tenantContainer);
            this.ContainerContext.RootScope.AddDisposableTracking(tenantContainer);
        }

        /// <inheritdoc />
        public IDependencyResolver? GetTenant(object tenantId) => this.tenantRepository.GetOrDefaultByValue(tenantId);
        /// <inheritdoc />
        public void RegisterResolver(IResolver resolver) => RootContainer.RegisterResolver(resolver);
        /// <inheritdoc />
        public IStashboxContainer CreateChildContainer(Action<ContainerConfigurator>? config = null) => RootContainer.CreateChildContainer(config);
        /// <inheritdoc />
        public IContainerContext ContainerContext => RootContainer.ContainerContext;
        /// <inheritdoc />
        public bool IsRegistered<TFrom>(object? name = null) => RootContainer.IsRegistered<TFrom>(name);
        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, object? name = null) => RootContainer.IsRegistered(typeFrom, name);
        /// <inheritdoc />
        public void Configure(Action<ContainerConfigurator> config) => RootContainer.Configure(config);
        /// <inheritdoc />
        public void Validate()
        {
            var exceptions = new ExpandableArray<Exception>();

            try
            {
                this.RootContainer.Validate();
            }
            catch (AggregateException ex)
            {
                exceptions.Add(new AggregateException($"Root container validation failed. See the inner exceptions for details.", ex.InnerExceptions));
            }

            foreach (var tenant in this.tenantRepository.Walk())
            {
                try
                {
                    tenant.Value.Validate();
                }
                catch (AggregateException ex)
                {
                    exceptions.Add(new AggregateException($"Tenant validation failed for '{tenant.Key}'. See the inner exceptions for details.", ex.InnerExceptions));
                }
            }

            if (exceptions.Length > 0)
                throw new AggregateException("Tenant distributor validation failed. See the inner exceptions for details.", exceptions);
        }
        /// <inheritdoc />
        public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() => RootContainer.GetRegistrationMappings();
        /// <inheritdoc />
        public IEnumerable<RegistrationDiagnosticsInfo> GetRegistrationDiagnostics() => RootContainer.GetRegistrationDiagnostics();
        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.RootContainer.Dispose();
        }
#if HAS_ASYNC_DISPOSABLE
        /// <inheritdoc />
        public ValueTask DisposeAsync() => Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0 ? new ValueTask(Task.CompletedTask) : this.RootContainer.DisposeAsync();
#endif
        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>> configurator) where TFrom : class where TTo : class, TFrom => RootContainer.Register(configurator);
        /// <inheritdoc />
        public IStashboxContainer Register<TFrom, TTo>(object? name = null) where TFrom : class where TTo : class, TFrom => RootContainer.Register<TFrom, TTo>(name);
        /// <inheritdoc />
        public IStashboxContainer Register<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => RootContainer.Register(typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer Register(Type typeFrom, Type typeTo, Action<RegistrationConfigurator>? configurator = null) => RootContainer.Register(typeFrom, typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(Action<RegistrationConfigurator<TTo, TTo>> configurator) where TTo : class => RootContainer.Register<TTo>(configurator);
        /// <inheritdoc />
        public IStashboxContainer Register<TTo>(object? name = null) where TTo : class => RootContainer.Register<TTo>(name);
        /// <inheritdoc />
        public IStashboxContainer Register(Type typeTo, Action<RegistrationConfigurator>? configurator = null) => RootContainer.Register(typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TFrom, TTo>(object? name = null) where TFrom : class where TTo : class, TFrom => RootContainer.RegisterSingleton<TFrom, TTo>(name);
        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton<TTo>(object? name = null) where TTo : class => RootContainer.RegisterSingleton<TTo>(name);
        /// <inheritdoc />
        public IStashboxContainer RegisterSingleton(Type typeFrom, Type typeTo, object? name = null) => RootContainer.RegisterSingleton(typeFrom, typeTo, name);
        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TFrom, TTo>(object? name = null) where TFrom : class where TTo : class, TFrom => RootContainer.RegisterScoped<TFrom, TTo>(name);
        /// <inheritdoc />
        public IStashboxContainer RegisterScoped(Type typeFrom, Type typeTo, object? name = null) => RootContainer.RegisterScoped(typeFrom, typeTo, name);
        /// <inheritdoc />
        public IStashboxContainer RegisterScoped<TTo>(object? name = null) where TTo : class => RootContainer.RegisterScoped<TTo>(name);
        /// <inheritdoc />
        public IStashboxContainer RegisterInstance<TInstance>(TInstance instance, object? name = null,
            bool withoutDisposalTracking = false, Action<TInstance>? finalizerDelegate = null) where TInstance : class =>
            RootContainer.RegisterInstance(instance, name, withoutDisposalTracking, finalizerDelegate);
        /// <inheritdoc />
        public IStashboxContainer RegisterInstance(object instance, Type serviceType, object? name = null,
            bool withoutDisposalTracking = false) =>
            RootContainer.RegisterInstance(instance, serviceType, name, withoutDisposalTracking);
        /// <inheritdoc />
        public IStashboxContainer WireUp<TInstance>(TInstance instance, object? name = null, bool withoutDisposalTracking = false,
            Action<TInstance>? finalizerDelegate = null) where TInstance : class =>
            RootContainer.WireUp(instance, name, withoutDisposalTracking, finalizerDelegate);
        /// <inheritdoc />
        public IStashboxContainer WireUp(object instance, Type serviceType, object? name = null, bool withoutDisposalTracking = false) => RootContainer.WireUp(instance, serviceType, name, withoutDisposalTracking);
        /// <inheritdoc />
        public object? GetService(Type serviceType) => RootContainer.GetService(serviceType);
        /// <inheritdoc />
        public object Resolve(Type typeFrom) => RootContainer.Resolve(typeFrom);
        /// <inheritdoc />
        public object Resolve(Type typeFrom, object[] dependencyOverrides) => RootContainer.Resolve(typeFrom, dependencyOverrides);
        /// <inheritdoc />
        public object Resolve(Type typeFrom, object? name) => RootContainer.Resolve(typeFrom, name);
        /// <inheritdoc />
        public object Resolve(Type typeFrom, object? name, object[] dependencyOverrides) => RootContainer.Resolve(typeFrom, name, dependencyOverrides);
        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom) => RootContainer.ResolveOrDefault(typeFrom);
        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom, object[] dependencyOverrides) => RootContainer.ResolveOrDefault(typeFrom, dependencyOverrides);
        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom, object? name) => RootContainer.ResolveOrDefault(typeFrom, name);
        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom, object? name, object[] dependencyOverrides) => RootContainer.ResolveOrDefault(typeFrom, name, dependencyOverrides);
        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() => RootContainer.ResolveAll<TKey>();
        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object? name) => RootContainer.ResolveAll<TKey>(name);
        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides) => RootContainer.ResolveAll<TKey>(dependencyOverrides);
        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object? name, object[] dependencyOverrides) => RootContainer.ResolveAll<TKey>(name, dependencyOverrides);
        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom) => RootContainer.ResolveAll(typeFrom);
        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object? name) => RootContainer.ResolveAll(typeFrom, name);
        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides) => RootContainer.ResolveAll(typeFrom, dependencyOverrides);
        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object? name, object[] dependencyOverrides) => RootContainer.ResolveAll(typeFrom, name, dependencyOverrides);
        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, object? name = null, params Type[] parameterTypes) => RootContainer.ResolveFactory(typeFrom, name, parameterTypes);
        /// <inheritdoc />
        public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, params Type[] parameterTypes) => RootContainer.ResolveFactoryOrDefault(typeFrom, name, parameterTypes);
        /// <inheritdoc />
        public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false) => RootContainer.BeginScope(name, attachToParent);
        /// <inheritdoc />
        public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null) => RootContainer.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking, name);
        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance) where TTo : class => RootContainer.BuildUp(instance);
        /// <inheritdoc />
        public object Activate(Type type, params object[] arguments) => RootContainer.Activate(type, arguments);
        /// <inheritdoc />
        public ValueTask InvokeAsyncInitializers(CancellationToken token = default) => RootContainer.InvokeAsyncInitializers(token);
        /// <inheritdoc />
        public bool CanResolve<TFrom>(object? name = null) => RootContainer.CanResolve<TFrom>(name);
        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object? name = null) => RootContainer.CanResolve(typeFrom, name);
        /// <inheritdoc />
        public IEnumerable<DelegateCacheEntry> GetDelegateCacheEntries() => RootContainer.GetDelegateCacheEntries();
        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom, TTo>(Action<RegistrationConfigurator<TFrom, TTo>>? configurator = null) where TFrom : class where TTo : class, TFrom => RootContainer.ReMap(configurator);
        /// <inheritdoc />
        public IStashboxContainer ReMap<TFrom>(Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => RootContainer.ReMap(typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer ReMap(Type typeFrom, Type typeTo, Action<RegistrationConfigurator>? configurator = null) => RootContainer.ReMap(typeFrom, typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer ReMap<TTo>(Action<RegistrationConfigurator<TTo, TTo>>? configurator = null) where TTo : class => RootContainer.ReMap<TTo>(configurator);
        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator>? configurator = null) => RootContainer.ReMapDecorator(typeFrom, typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>>? configurator = null) where TFrom : class where TTo : class, TFrom => RootContainer.ReMapDecorator(configurator);
        /// <inheritdoc />
        public IStashboxContainer ReMapDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => RootContainer.ReMapDecorator(typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterTypesAs(Type typeFrom, IEnumerable<Type> types, Func<Type, bool>? selector = null,
            Action<RegistrationConfigurator>? configurator = null) =>
            RootContainer.RegisterTypesAs(typeFrom, types, selector, configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterTypes(IEnumerable<Type> types, Func<Type, bool>? selector = null, Func<Type, Type, bool>? serviceTypeSelector = null,
            bool registerSelf = true, Action<RegistrationConfigurator>? configurator = null) =>
            RootContainer.RegisterTypes(types, selector, serviceTypeSelector, registerSelf, configurator);
        /// <inheritdoc />
        public IStashboxContainer ComposeBy(Type compositionRootType, params object[] compositionRootArguments) => RootContainer.ComposeBy(compositionRootType, compositionRootArguments);
        /// <inheritdoc />
        public IStashboxContainer ComposeBy(ICompositionRoot compositionRoot) => RootContainer.ComposeBy(compositionRoot);
        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeFrom, Type typeTo, Action<DecoratorConfigurator>? configurator = null) => RootContainer.RegisterDecorator(typeFrom, typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom, TTo>(Action<DecoratorConfigurator<TFrom, TTo>>? configurator = null) where TFrom : class where TTo : class, TFrom => RootContainer.RegisterDecorator(configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator(Type typeTo, Action<DecoratorConfigurator>? configurator = null) => RootContainer.RegisterDecorator(typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TTo>(Action<DecoratorConfigurator<TTo, TTo>>? configurator = null) where TTo : class => RootContainer.RegisterDecorator<TTo>(configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterDecorator<TFrom>(Type typeTo, Action<DecoratorConfigurator<TFrom, TFrom>>? configurator = null) where TFrom : class => RootContainer.RegisterDecorator(typeTo, configurator);
        /// <inheritdoc />
        public IStashboxContainer RegisterFunc<TService>(Func<IDependencyResolver, TService> factory, object? name = null) => RootContainer.RegisterFunc(factory, name);
        /// <inheritdoc />
        public IStashboxContainer RegisterFunc<T1, TService>(Func<T1, IDependencyResolver, TService> factory, object? name = null) => RootContainer.RegisterFunc(factory, name);
        /// <inheritdoc />
        public IStashboxContainer RegisterFunc<T1, T2, TService>(Func<T1, T2, IDependencyResolver, TService> factory, object? name = null) => RootContainer.RegisterFunc(factory, name);
        /// <inheritdoc />
        public IStashboxContainer RegisterFunc<T1, T2, T3, TService>(Func<T1, T2, T3, IDependencyResolver, TService> factory, object? name = null) => RootContainer.RegisterFunc(factory, name);
        /// <inheritdoc />
        public IStashboxContainer RegisterFunc<T1, T2, T3, T4, TService>(Func<T1, T2, T3, T4, IDependencyResolver, TService> factory, object? name = null) => RootContainer.RegisterFunc(factory, name);
    }
}
