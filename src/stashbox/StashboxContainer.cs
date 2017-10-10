using Stashbox.BuildUp;
using Stashbox.BuildUp.Expressions;
using Stashbox.BuildUp.Resolution;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents the stashbox dependency injection container.
    /// </summary>
    public partial class StashboxContainer : IStashboxContainer
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IResolverSelector resolverSelector;
        private readonly IRegistrationRepository registrationRepository = new RegistrationRepository();
        private readonly IExpressionBuilder expressionBuilder;
        private readonly AtomicBool disposed;
        private readonly IActivationContext activationContext;
        private readonly IObjectBuilderSelector objectBuilderSelector;

        private readonly IResolutionScope rootScope;
        private readonly IDependencyResolver rootResolver;

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
            this.disposed = new AtomicBool();
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;

            config?.Invoke(containerConfigurator);

            this.ContainerContext = new ContainerContext(this.registrationRepository, new DelegateRepository(), this,
                new ResolutionStrategy(this.resolverSelector), containerConfigurator, decoratorRepository);

            this.activationContext = new Resolution.ActivationContext(this.ContainerContext, this.resolverSelector);
            this.expressionBuilder = new ExpressionBuilder(this.containerExtensionManager);
            this.objectBuilderSelector = new ObjectBuilderSelector(this.expressionBuilder);
            this.ServiceRegistrator = new ServiceRegistrator(this.ContainerContext, this.containerExtensionManager, this.objectBuilderSelector);

            this.rootScope = new ResolutionScope(this.activationContext,
            this.ServiceRegistrator, this.expressionBuilder, this.ContainerContext);

            this.rootResolver = (IDependencyResolver)this.rootScope;
        }

        /// <inheritdoc />
        public void RegisterExtension(IContainerExtension containerExtension)
        {
            containerExtension.Initialize(this.ContainerContext);
            this.containerExtensionManager.AddExtension(containerExtension);
        }

        /// <inheritdoc />
        public void RegisterResolver(Resolver resolver) =>
            this.resolverSelector.AddResolver(resolver);

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name) ||
                this.resolverSelector.CanResolve(this.ContainerContext, new TypeInformation { Type = typeFrom, DependencyName = name }, ResolutionContext.New(this.rootScope));

        /// <inheritdoc />
        public bool IsRegistered<TFrom>(object name = null) =>
            this.IsRegistered(typeof(TFrom), name);

        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, object name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name);

        /// <inheritdoc />
        public void Validate()
        {
            foreach (var serviceRegistration in this.registrationRepository.GetAllRegistrations())
                serviceRegistration.GetExpression(this.ContainerContext, ResolutionContext.New(this.rootScope), serviceRegistration.ServiceType);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IServiceRegistrator ServiceRegistrator { get; }

        /// <inheritdoc />
        public IStashboxContainer CreateChildContainer() =>
             new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector,
                 this.ContainerContext.ContainerConfigurator, this.ContainerContext.DecoratorRepository);

        /// <inheritdoc />
        public IDependencyResolver BeginScope(object name = null) => new ResolutionScope(this.activationContext,
            this.ServiceRegistrator, this.expressionBuilder, this.ContainerContext, this.rootScope, this.rootScope, name);

        /// <inheritdoc />
        public void Configure(Action<IContainerConfigurator> config) =>
            config?.Invoke(this.ContainerContext.ContainerConfigurator);

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
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
        /// <param name="disposing">Indicates the container is disposing or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed.CompareExchange(false, true) || !disposing) return;
            this.rootScope.Dispose();
            this.containerExtensionManager.CleanUp();
        }
    }
}
