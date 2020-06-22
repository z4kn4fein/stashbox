using Stashbox.Configuration;
using Stashbox.Expressions;
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
    /// Represents the Stashbox dependency injection container.
    /// </summary>
    public sealed partial class StashboxContainer : IStashboxContainer
    {
        private readonly IDependencyResolver rootResolver;
        private readonly ContainerConfigurator containerConfigurator;
        private readonly ServiceRegistrator serviceRegistrator;
        private readonly RegistrationBuilder registrationBuilder;
        private readonly ExpressionFactory expressionFactory;
        private readonly ExpressionBuilder expressionBuilder;
        private readonly ResolutionStrategy resolutionStrategy;
        private int disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>.
        /// </summary>
        public StashboxContainer(Action<ContainerConfigurator> config = null)
            : this(new ExpressionFactory(new MethodExpressionFactory(), new MemberExpressionFactory()),
                new ServiceRegistrator(), new RegistrationBuilder(), new ContainerConfigurator(), config)
        {
            this.expressionBuilder = new ExpressionBuilder(this.expressionFactory, this.serviceRegistrator);
            this.resolutionStrategy = new ResolutionStrategy(this.expressionBuilder);

            this.ContainerContext = new ContainerContext(null, resolutionStrategy,
                expressionFactory, this.containerConfigurator.ContainerConfiguration);

            this.rootResolver = (IDependencyResolver)this.ContainerContext.RootScope;

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, ServiceRegistrator serviceRegistrator,
            RegistrationBuilder registrationBuilder, ResolutionStrategy resolutionStrategy, ExpressionFactory expressionFactory,
            ExpressionBuilder expressionBuilder, ContainerConfigurator containerConfigurator)
            : this(expressionFactory, serviceRegistrator, registrationBuilder, containerConfigurator)
        {
            this.resolutionStrategy = resolutionStrategy;
            this.expressionBuilder = expressionBuilder;

            this.ContainerContext = new ContainerContext(parentContainer.ContainerContext, resolutionStrategy,
                expressionFactory, this.containerConfigurator.ContainerConfiguration);

            this.rootResolver = (IDependencyResolver)this.ContainerContext.RootScope;
        }

        internal StashboxContainer(ExpressionFactory expressionFactory, ServiceRegistrator serviceRegistrator,
            RegistrationBuilder registrationBuilder, ContainerConfigurator containerConfigurator, Action<ContainerConfigurator> config = null)
        {
            this.expressionFactory = expressionFactory;
            this.containerConfigurator = containerConfigurator;
            this.serviceRegistrator = serviceRegistrator;
            this.registrationBuilder = registrationBuilder;

            config?.Invoke(this.containerConfigurator);
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
                this.resolutionStrategy.CanResolveType(new TypeInformation(typeFrom, name),
                    new ResolutionContext(this.ContainerContext.RootScope.GetActiveScopeNames(), this.ContainerContext, this.resolutionStrategy, false));

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
                this.expressionBuilder.BuildExpressionAndApplyLifetime(serviceRegistration.Value,
                    new ResolutionContext(this.ContainerContext.RootScope.GetActiveScopeNames(),
                    this.ContainerContext, this.resolutionStrategy, false), serviceRegistration.Key);
        }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IStashboxContainer CreateChildContainer() =>
             new StashboxContainer(this, this.serviceRegistrator, this.registrationBuilder, this.resolutionStrategy,
                 this.expressionFactory, this.expressionBuilder, new ContainerConfigurator(this.ContainerContext.ContainerConfiguration.Clone()));

        /// <inheritdoc />
        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false) =>
            this.rootResolver.BeginScope(name, attachToParent);

        /// <inheritdoc />
        public void Configure(Action<ContainerConfigurator> config)
        {
            Shield.EnsureNotNull(config, "The config parameter cannot be null!");

            config.Invoke(this.containerConfigurator);
            this.containerConfigurator.ContainerConfiguration.ConfigurationChangedEvent?
                .Invoke(this.containerConfigurator.ContainerConfiguration);

            this.ContainerContext.RootScope.InvalidateDelegateCache();
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings() =>
             this.ContainerContext.RegistrationRepository.GetRegistrationMappings();


        private void RegisterResolvers()
        {
            this.resolutionStrategy.RegisterResolver(new EnumerableResolver());
            this.resolutionStrategy.RegisterResolver(new LazyResolver());
            this.resolutionStrategy.RegisterResolver(new FuncResolver());
            this.resolutionStrategy.RegisterResolver(new TupleResolver());
            this.resolutionStrategy.RegisterResolver(new OptionalValueResolver());
            this.resolutionStrategy.RegisterResolver(new DefaultValueResolver());

            this.resolutionStrategy.RegisterLastChanceResolver(new ParentContainerResolver());
            this.resolutionStrategy.RegisterLastChanceResolver(new UnknownTypeResolver(this.serviceRegistrator, this.registrationBuilder, this.expressionBuilder));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.ContainerContext.RootScope.Dispose();
        }
    }
}
