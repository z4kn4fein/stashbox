using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Resolution.Resolvers;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stashbox.Utils.Data;
using System.Runtime.CompilerServices;

namespace Stashbox
{
    /// <summary>
    /// Represents the Stashbox dependency injection container.
    /// </summary>
    public sealed partial class StashboxContainer : IStashboxContainer
    {
        private readonly ContainerConfigurator containerConfigurator;
        private readonly ServiceRegistrator serviceRegistrator;
        private readonly RegistrationBuilder registrationBuilder;
        private readonly ResolutionStrategy resolutionStrategy;
        private int disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>.
        /// </summary>
        public StashboxContainer(Action<ContainerConfigurator> config = null)
            : this(new ServiceRegistrator(), new RegistrationBuilder(), new ContainerConfigurator(), config)
        {
            this.resolutionStrategy = new ResolutionStrategy();

            this.ContainerContext = new ContainerContext(null, resolutionStrategy,
                this.containerConfigurator.ContainerConfiguration);

            this.RegisterResolvers();
        }

        private StashboxContainer(IStashboxContainer parentContainer, ServiceRegistrator serviceRegistrator,
            RegistrationBuilder registrationBuilder, ResolutionStrategy resolutionStrategy,
            ContainerConfigurator containerConfigurator, Action<ContainerConfigurator> config = null)
            : this(serviceRegistrator, registrationBuilder, containerConfigurator, config)
        {
            this.resolutionStrategy = resolutionStrategy;

            this.ContainerContext = new ContainerContext(parentContainer.ContainerContext, resolutionStrategy,
                this.containerConfigurator.ContainerConfiguration);
        }

        private StashboxContainer(ServiceRegistrator serviceRegistrator,
            RegistrationBuilder registrationBuilder, ContainerConfigurator containerConfigurator, Action<ContainerConfigurator> config = null)
        {
            this.containerConfigurator = containerConfigurator;
            this.serviceRegistrator = serviceRegistrator;
            this.registrationBuilder = registrationBuilder;

            config?.Invoke(this.containerConfigurator);
        }

        /// <inheritdoc />
        public void RegisterResolver(IResolver resolver)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(resolver, nameof(resolver));

            this.resolutionStrategy.RegisterResolver(resolver);
        }

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object name = null)
        {
            this.ThrowIfDisposed();

            return this.ContainerContext.RegistrationRepository.ContainsRegistration(typeFrom, name) ||
                this.resolutionStrategy.CanResolveType(new TypeInformation(typeFrom, name),
                    new ResolutionContext(this.ContainerContext.RootScope.GetActiveScopeNames(), this.ContainerContext, false));
        }

        /// <inheritdoc />
        public bool IsRegistered<TFrom>(object name = null) =>
            this.IsRegistered(typeof(TFrom), name);

        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, object name = null)
        {
            this.ThrowIfDisposed();

            return this.ContainerContext.RegistrationRepository.ContainsRegistration(typeFrom, name);
        }

        /// <inheritdoc />
        public void Validate()
        {
            this.ThrowIfDisposed();

            var exceptions = new ExpandableArray<Exception>();

            foreach (var serviceRegistration in this.ContainerContext.RegistrationRepository
                .GetRegistrationMappings()
                .Where(reg => !reg.Key.IsOpenGenericType()))
            {
                try
                {
                    this.resolutionStrategy.BuildExpressionForRegistration(serviceRegistration.Value,
                        new ResolutionContext(this.ContainerContext.RootScope.GetActiveScopeNames(),
                            this.ContainerContext, false, false, true),
                        new TypeInformation(serviceRegistration.Key, serviceRegistration.Value.RegistrationContext.Name));
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Length > 0)
                throw new AggregateException("Container validation failed. See the inner exceptions for details.", exceptions);
        }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IStashboxContainer CreateChildContainer(Action<ContainerConfigurator> config = null)
        {
            this.ThrowIfDisposed();

            return new StashboxContainer(this, this.serviceRegistrator, this.registrationBuilder, this.resolutionStrategy,
                    new ContainerConfigurator(this.ContainerContext.ContainerConfiguration.Clone()), config);
        }

        /// <inheritdoc />
        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false) =>
            this.ContainerContext.RootScope.BeginScope(name, attachToParent);

        /// <inheritdoc />
        public void Configure(Action<ContainerConfigurator> config)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(config, "The config parameter cannot be null!");

            config.Invoke(this.containerConfigurator);
            this.containerConfigurator.ContainerConfiguration.ConfigurationChangedEvent?
                .Invoke(this.containerConfigurator.ContainerConfiguration);

            this.ContainerContext.RootScope.InvalidateDelegateCache();
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings()
        {
            this.ThrowIfDisposed();

            return this.ContainerContext.RegistrationRepository.GetRegistrationMappings();
        }

        /// <inheritdoc />
        public IEnumerable<RegistrationDiagnosticsInfo> GetRegistrationDiagnostics()
        {
            this.ThrowIfDisposed();
            foreach (var reg in this.ContainerContext.RegistrationRepository.GetRegistrationMappings())
                yield return new RegistrationDiagnosticsInfo(reg.Key, reg.Value.ImplementationType, reg.Value.RegistrationContext.Name);
        }

        private void RegisterResolvers()
        {
            this.resolutionStrategy.RegisterResolver(new EnumerableResolver());
            this.resolutionStrategy.RegisterResolver(new LazyResolver());
            this.resolutionStrategy.RegisterResolver(new FuncResolver());
            this.resolutionStrategy.RegisterResolver(new TupleResolver());
            this.resolutionStrategy.RegisterResolver(new OptionalValueResolver());
            this.resolutionStrategy.RegisterResolver(new DefaultValueResolver());

            this.resolutionStrategy.RegisterLastChanceResolver(new ParentContainerResolver());
            this.resolutionStrategy.RegisterLastChanceResolver(new UnknownTypeResolver(this.serviceRegistrator, this.registrationBuilder));
        }

        private void ThrowIfDisposed([CallerMemberName] string caller = "<unknown>")
        {
            if (this.disposed == 1)
                Shield.ThrowDisposedException(this.GetType().FullName, caller);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.ContainerContext.RootScope.Dispose();
        }

#if HAS_ASYNC_DISPOSABLE
        /// <inheritdoc />
        public ValueTask DisposeAsync() =>
            Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0
                ? new ValueTask(Task.CompletedTask)
                : this.ContainerContext.RootScope.DisposeAsync();
#endif
    }
}
