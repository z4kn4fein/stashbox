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
        private readonly IRegistrationRepository registrationRepository;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly AtomicBool disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>
        /// </summary>
        public StashboxContainer(Action<IContainerConfigurator> config = null)
        {
            this.disposed = new AtomicBool();
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
            this.expressionBuilder = new ExpressionBuilder();

            var configurator = new ContainerConfigurator();
            config?.Invoke(configurator);

            this.registrationRepository = new RegistrationRepository(configurator);
            this.ContainerContext = new ContainerContext(this.registrationRepository, new DelegateRepository(), this,
                new ResolutionStrategy(this.resolverSelector), configurator, new DecoratorRepository());
            this.ActivationContext = new Resolution.ActivationContext(this.ContainerContext, this.resolverSelector);

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector, IExpressionBuilder expressionBuilder)
        {
            this.disposed = new AtomicBool();
            this.ParentContainer = parentContainer;
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;
            this.expressionBuilder = expressionBuilder;
            this.registrationRepository = new RegistrationRepository(parentContainer.ContainerContext.ContainerConfigurator);
            this.ContainerContext = new ContainerContext(this.registrationRepository, new DelegateRepository(), this, new ResolutionStrategy(this.resolverSelector),
                parentContainer.ContainerContext.ContainerConfigurator, parentContainer.ContainerContext.DecoratorRepository);
            this.ActivationContext = new Resolution.ActivationContext(this.ContainerContext, this.resolverSelector);
            this.containerExtensionManager.ReinitalizeExtensions(this.ContainerContext);
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
        public bool CanResolve<TFrom>(string name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, string name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name) || 
                this.resolverSelector.CanResolve(this.ContainerContext, new TypeInformation { Type = typeFrom, DependencyName = name });

        /// <inheritdoc />
        public void Validate()
        {
            foreach (var serviceRegistration in this.registrationRepository.GetAllRegistrations())
                serviceRegistration.GetExpression(ResolutionInfo.New(), serviceRegistration.ServiceType);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IActivationContext ActivationContext { get; }

        /// <inheritdoc />
        public IStashboxContainer BeginScope()
        {
            var container = new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector, this.expressionBuilder);
            container.OpenScope();
            return container;
        }

        /// <inheritdoc />
        public void Configure(Action<IContainerConfigurator> config) => 
            config?.Invoke(this.ContainerContext.ContainerConfigurator);

        internal void OpenScope()
        {
            foreach (var registrationItem in this.ParentContainer.ContainerContext.ScopedRegistrations)
            {
                var registration = new ScopedRegistrationContext(registrationItem.TypeFrom, registrationItem.TypeTo,
                    this.ContainerContext, this.expressionBuilder, this.containerExtensionManager);

                registration.InitFromScope(registrationItem.RegistrationContextData.CreateCopy());
            }
        }

        private void RegisterResolvers()
        {
            this.resolverSelector.AddResolver(new EnumerableResolver());
            this.resolverSelector.AddResolver(new LazyResolver(this.resolverSelector));
            this.resolverSelector.AddResolver(new FuncResolver());
            this.resolverSelector.AddResolver(new TupleResolver());
            this.resolverSelector.AddResolver(new DefaultValueResolver());
            this.resolverSelector.AddResolver(new UnknownTypeResolver());
            this.resolverSelector.AddResolver(new ParentContainerResolver());
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
        /// <param name="disposing">Indicates the container is disposing or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed.CompareExchange(false, true) || !disposing) return;
            this.registrationRepository.CleanUp();
            this.containerExtensionManager.CleanUp();

            var trackedObjects = this.ContainerContext.TrackedTransientObjects.GetAll();
            foreach (var trackedObject in trackedObjects)
            {
                var disposable = trackedObject as IDisposable;
                disposable?.Dispose();
            }
        }
    }
}
