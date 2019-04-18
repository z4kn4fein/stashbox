using Stashbox.BuildUp;
using Stashbox.BuildUp.Expressions;
using Stashbox.BuildUp.Resolution;
using Stashbox.Configuration;
using Stashbox.ContainerExtension;
using Stashbox.Entity;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Threading;

namespace Stashbox
{
    /// <summary>
    /// Represents the stashbox dependency injection container.
    /// </summary>
    public sealed partial class StashboxContainer : IStashboxContainer
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IResolverSelector resolverSelector;
        private readonly IRegistrationRepository registrationRepository;
        private readonly IObjectBuilderSelector objectBuilderSelector;
        private readonly IDependencyResolver rootResolver;
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IRegistrationBuilder registrationBuilder;

        private int disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>
        /// </summary>
        public StashboxContainer(Action<IContainerConfigurator> config = null)
            : this(new BuildExtensionManager(), new ResolverSelector(), new ContainerConfigurator(), new DecoratorRepository(), config)
        {
            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector, IContainerConfigurator containerConfigurator, IDecoratorRepository decoratorRepository)
            : this(containerExtensionManager, resolverSelector, containerConfigurator, decoratorRepository)
        {
            this.ParentContainer = parentContainer;
            this.containerExtensionManager.ReinitalizeExtensions(this.ContainerContext);

        }

        internal StashboxContainer(IContainerExtensionManager containerExtensionManager, IResolverSelector resolverSelector,
            IContainerConfigurator containerConfigurator, IDecoratorRepository decoratorRepository, Action<IContainerConfigurator> config = null)
        {
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;

            config?.Invoke(containerConfigurator);

            this.registrationRepository = new RegistrationRepository();

            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new ResolutionStrategy(this.resolverSelector), containerConfigurator, decoratorRepository);

            var expressionBuilder = new ExpressionBuilder(this.containerExtensionManager, new ConstructorSelector());
            this.serviceRegistrator = new ServiceRegistrator(this.ContainerContext, this.containerExtensionManager);
            this.objectBuilderSelector = new ObjectBuilderSelector(expressionBuilder, this.serviceRegistrator);
            this.registrationBuilder = new RegistrationBuilder(this.ContainerContext, this.objectBuilderSelector);

            this.RootScope = new ResolutionScope(this.resolverSelector, expressionBuilder, this.ContainerContext);
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
            this.resolverSelector.AddResolver(resolver);

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name) ||
                this.resolverSelector.CanResolve(this.ContainerContext, new TypeInformation { Type = typeFrom, DependencyName = name }, ResolutionContext.New(this.RootScope));

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
             new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector,
                 this.ContainerContext.ContainerConfigurator, this.ContainerContext.DecoratorRepository);

        /// <inheritdoc />
        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false) =>
            this.rootResolver.BeginScope(name, attachToParent);

        /// <inheritdoc />
        public void Configure(Action<IContainerConfigurator> config)
        {
            config?.Invoke(this.ContainerContext.ContainerConfigurator);
            this.ContainerContext.ContainerConfigurator.ContainerConfiguration.ConfigurationChangedEvent?
                .Invoke(this.ContainerContext.ContainerConfigurator.ContainerConfiguration);
        }

        private void RegisterResolvers()
        {
            this.resolverSelector.AddResolver(new EnumerableResolver());
            this.resolverSelector.AddResolver(new LazyResolver(this.resolverSelector));
            this.resolverSelector.AddResolver(new FuncResolver());
            this.resolverSelector.AddResolver(new TupleResolver());
            this.resolverSelector.AddResolver(new ScopedInstanceResolver());
            this.resolverSelector.AddResolver(new DefaultValueResolver());
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
