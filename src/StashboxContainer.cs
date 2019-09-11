using Stashbox.BuildUp;
using Stashbox.BuildUp.Expressions;
using Stashbox.Configuration;
using Stashbox.ContainerExtension;
using Stashbox.Entity;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Resolution.Resolvers;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Stashbox
{
    /// <summary>
    /// Represents the stashbox dependency injection container.
    /// </summary>
    public sealed partial class StashboxContainer : IStashboxContainer
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IResolverSupportedResolutionStrategy resolutionStrategy;
        private readonly IRegistrationRepository registrationRepository;
        private readonly IObjectBuilderSelector objectBuilderSelector;
        private readonly IDependencyResolver rootResolver;
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IRegistrationBuilder registrationBuilder;
        private readonly IContainerConfigurator containerConfigurator;

        private int disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>
        /// </summary>
        public StashboxContainer(Action<IContainerConfigurator> config = null)
            : this(new BuildExtensionManager(), new ResolutionStrategy(), new ContainerConfigurator(), new DecoratorRepository(), config)
        {
            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSupportedResolutionStrategy resolutionStrategy, IContainerConfigurator containerConfigurator, IDecoratorRepository decoratorRepository)
            : this(containerExtensionManager, resolutionStrategy, containerConfigurator, decoratorRepository)
        {
            this.ParentContainer = parentContainer;
            this.containerExtensionManager.ReinitalizeExtensions(this.ContainerContext);

        }

        internal StashboxContainer(IContainerExtensionManager containerExtensionManager, IResolverSupportedResolutionStrategy resolutionStrategy,
            IContainerConfigurator containerConfigurator, IDecoratorRepository decoratorRepository, Action<IContainerConfigurator> config = null)
        {
            this.resolutionStrategy = resolutionStrategy;
            this.containerExtensionManager = containerExtensionManager;
            this.containerConfigurator = containerConfigurator;

            config?.Invoke(this.containerConfigurator);

            this.registrationRepository = new RegistrationRepository();

            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                this.containerConfigurator.ContainerConfiguration, decoratorRepository);

            var expressionBuilder = new ExpressionBuilder(this.containerExtensionManager,
                new ConstructorSelector(this.resolutionStrategy), this.resolutionStrategy);

            this.serviceRegistrator = new ServiceRegistrator(this.containerExtensionManager);
            this.objectBuilderSelector = new ObjectBuilderSelector(expressionBuilder, this.serviceRegistrator);

            this.RootScope = new ResolutionScope(this.resolutionStrategy, expressionBuilder, this.ContainerContext);
            this.registrationBuilder = new RegistrationBuilder(this.containerConfigurator.ContainerConfiguration, this.RootScope, this.objectBuilderSelector);
            this.rootResolver = (IDependencyResolver)this.RootScope;
        }

        /// <inheritdoc />
        public void RegisterExtension(IContainerExtension containerExtension)
        {
            containerExtension.Initialize(this.ContainerContext);
            this.containerExtensionManager.AddExtension(containerExtension);
        }

        /// <inheritdoc />
        public void RegisterResolver(IResolver resolver) =>
            this.resolutionStrategy.RegisterResolver(resolver);

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name) ||
                this.resolutionStrategy.CanResolveType(this.ContainerContext, new TypeInformation { Type = typeFrom, DependencyName = name }, ResolutionContext.New(this.RootScope));

        /// <inheritdoc />
        public bool IsRegistered<TFrom>(object name = null) =>
            this.IsRegistered(typeof(TFrom), name);

        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, object name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name);

        /// <inheritdoc />
        public void Validate()
        {
            foreach (var serviceRegistration in this.registrationRepository.GetRegistrationMappings())
                serviceRegistration.Value.GetExpression(this.ContainerContext, ResolutionContext.New(this.RootScope), serviceRegistration.Key);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IResolutionScope RootScope { get; }

        /// <inheritdoc />
        public IStashboxContainer CreateChildContainer() =>
             new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolutionStrategy,
                 this.containerConfigurator, this.ContainerContext.DecoratorRepository);

        /// <inheritdoc />
        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false) =>
            this.rootResolver.BeginScope(name, attachToParent);

        /// <inheritdoc />
        public void Configure(Action<IContainerConfigurator> config)
        {
            Shield.EnsureNotNull(config, "The config parameter cannot be null!");

            config.Invoke(this.containerConfigurator);
            this.containerConfigurator.ContainerConfiguration.ConfigurationChangedEvent?
                .Invoke(this.containerConfigurator.ContainerConfiguration);
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<Type, IServiceRegistration>> GetRegistrationMappings() =>
             this.ContainerContext.RegistrationRepository.GetRegistrationMappings();


        private void RegisterResolvers()
        {
            this.resolutionStrategy.RegisterResolver(new EnumerableResolver());
            this.resolutionStrategy.RegisterResolver(new LazyResolver());
            this.resolutionStrategy.RegisterResolver(new FuncResolver());
            this.resolutionStrategy.RegisterResolver(new TupleResolver());
            this.resolutionStrategy.RegisterResolver(new ScopedInstanceResolver());
            this.resolutionStrategy.RegisterResolver(new DefaultValueResolver());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.RootScope.Dispose();
            this.containerExtensionManager.CleanUp();
        }
    }
}
