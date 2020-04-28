using Stashbox.BuildUp;
using Stashbox.BuildUp.Expressions;
using Stashbox.Configuration;
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
        private readonly IResolverSupportedResolutionStrategy resolutionStrategy;
        private readonly IObjectBuilderSelector objectBuilderSelector;
        private readonly IDependencyResolver rootResolver;
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IRegistrationBuilder registrationBuilder;
        private readonly IContainerConfigurator containerConfigurator;
        private readonly IExpressionBuilder expressionBuilder;
        private int disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>.
        /// </summary>
        public StashboxContainer(Action<IContainerConfigurator> config = null)
            : this(new ResolutionStrategy(), new ServiceRegistrator(), new ContainerConfigurator(), config)
        {
            this.expressionBuilder = new ExpressionBuilder(new MethodExpressionBuilder(this.resolutionStrategy),
                new MemberExpressionBuilder(this.resolutionStrategy));
            this.objectBuilderSelector = new ObjectBuilderSelector(this.expressionBuilder, this.serviceRegistrator);

            this.RootScope = new ResolutionScope(this.resolutionStrategy, this.expressionBuilder, this.ContainerContext);
            this.registrationBuilder = new RegistrationBuilder(this.containerConfigurator.ContainerConfiguration,
                this.RootScope, this.objectBuilderSelector);
            this.rootResolver = (IDependencyResolver)this.RootScope;

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IServiceRegistrator serviceRegistrator,
            IResolverSupportedResolutionStrategy resolutionStrategy, IExpressionBuilder expressionBuilder,
            IObjectBuilderSelector objectBuilderSelector, IContainerConfigurator containerConfigurator)
            : this(resolutionStrategy, serviceRegistrator, containerConfigurator)
        {
            this.ParentContainer = parentContainer;
            this.expressionBuilder = expressionBuilder;
            this.objectBuilderSelector = objectBuilderSelector;

            this.RootScope = new ResolutionScope(this.resolutionStrategy, this.expressionBuilder, this.ContainerContext);
            this.registrationBuilder = new RegistrationBuilder(this.containerConfigurator.ContainerConfiguration,
                this.RootScope, this.objectBuilderSelector);
            this.rootResolver = (IDependencyResolver)this.RootScope;
        }

        internal StashboxContainer(IResolverSupportedResolutionStrategy resolutionStrategy, IServiceRegistrator serviceRegistrator,
            IContainerConfigurator containerConfigurator, Action<IContainerConfigurator> config = null)
        {
            this.resolutionStrategy = resolutionStrategy;
            this.containerConfigurator = containerConfigurator;
            this.serviceRegistrator = serviceRegistrator;

            config?.Invoke(this.containerConfigurator);
            this.ContainerContext = new ContainerContext(this, this.containerConfigurator.ContainerConfiguration);
        }

        /// <inheritdoc />
        public void RegisterResolver(IResolver resolver) =>
            this.resolutionStrategy.RegisterResolver(resolver);

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object name = null) =>
            this.ContainerContext.RegistrationRepository.ContainsRegistration(typeFrom, name) ||
                this.resolutionStrategy.CanResolveType(this.ContainerContext,
                    new TypeInformation { Type = typeFrom, DependencyName = name },
                    ResolutionContext.New(this.RootScope.GetActiveScopeNames(), this.ContainerContext));

        /// <inheritdoc />
        public bool IsRegistered<TFrom>(object name = null) =>
            this.IsRegistered(typeof(TFrom), name);

        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, object name = null) =>
            this.ContainerContext.RegistrationRepository.ContainsRegistration(typeFrom, name);

        /// <inheritdoc />
        public void Validate()
        {
            foreach (var serviceRegistration in this.ContainerContext.RegistrationRepository.GetRegistrationMappings())
                serviceRegistration.Value.GetExpression(this.ContainerContext, ResolutionContext.New(this.RootScope.GetActiveScopeNames(),
                    this.ContainerContext), serviceRegistration.Key);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IResolutionScope RootScope { get; }

        /// <inheritdoc />
        public IStashboxContainer CreateChildContainer() =>
             new StashboxContainer(this, this.serviceRegistrator, this.resolutionStrategy, this.expressionBuilder,
                 this.objectBuilderSelector, this.containerConfigurator);

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
            this.resolutionStrategy.RegisterResolver(new DefaultValueResolver());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.RootScope.Dispose();
        }
    }
}
