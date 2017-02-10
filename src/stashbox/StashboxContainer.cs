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
using System.Reflection;

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
        private readonly AtomicBool disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>
        /// </summary>
        public StashboxContainer(Action<ContainerConfiguration> config = null)
        {
            this.disposed = new AtomicBool();
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();

            this.ContainerConfiguration = ContainerConfiguration.DefaultContainerConfiguration();
            config?.Invoke(this.ContainerConfiguration);

            this.registrationRepository = new RegistrationRepository(this);
            this.ContainerContext = new ContainerContext(this.registrationRepository, new DelegateRepository(), this,
                new ResolutionStrategy(this.resolverSelector), this.resolverSelector);
            this.ActivationContext = new ActivationContext(this.ContainerContext, this.resolverSelector);

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector, ContainerConfiguration containerConfiguration)
        {
            this.disposed = new AtomicBool();
            this.ParentContainer = parentContainer;
            this.ContainerConfiguration = containerConfiguration;
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;
            this.registrationRepository = new RegistrationRepository(this);
            this.ContainerContext = new ContainerContext(this.registrationRepository, new DelegateRepository(), this,
                new ResolutionStrategy(this.resolverSelector), this.resolverSelector);
            this.ActivationContext = new ActivationContext(this.ContainerContext, this.resolverSelector);
            this.containerExtensionManager.ReinitalizeExtensions(this.ContainerContext);
        }

        /// <inheritdoc />
        public void RegisterExtension(IContainerExtension containerExtension)
        {
            containerExtension.Initialize(this.ContainerContext);
            this.containerExtensionManager.AddExtension(containerExtension);
        }

        /// <inheritdoc />
        public void RegisterResolver(Func<IContainerContext, TypeInformation, bool> resolverPredicate,
            Func<IContainerContext, TypeInformation, Resolver> factory)
        {
            var resolver = new ResolverRegistration
            {
                ResolverFactory = factory,
                Predicate = resolverPredicate
            };
            this.resolverSelector.AddResolver(resolver);
        }

        /// <inheritdoc />
        public bool CanResolve<TFrom>(string name = null)
        {
            return this.CanResolve(typeof(TFrom), name);
        }

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, string name = null)
        {
            var typeInfo = new TypeInformation { Type = typeFrom, DependencyName = name };
            return this.registrationRepository.ContainsRegistration(typeInfo) || this.resolverSelector.CanResolve(this.ContainerContext, typeInfo);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IActivationContext ActivationContext { get; }

        /// <inheritdoc />
        public ContainerConfiguration ContainerConfiguration { get; }

        /// <inheritdoc />
        public IStashboxContainer BeginScope()
        {
            var container = new StashboxContainer(this, this.containerExtensionManager.CreateCopy(),
                this.resolverSelector.CreateCopy(), this.ContainerConfiguration);
            container.OpenScope();
            return container;
        }

        internal void OpenScope()
        {
            foreach (var registrationItem in this.ParentContainer.ContainerContext.ScopedRegistrations)
            {
                var registration = new ScopedRegistrationContext(registrationItem.TypeFrom, registrationItem.TypeTo,
                    this.ContainerContext, this.containerExtensionManager);

                registration.InitFromScope(registrationItem.RegistrationContextData.CreateCopy());
            }
        }

        private void RegisterResolvers()
        {
            var enumerableResolver = new ResolverRegistration
            {
                ResolverFactory = (context, typeInfo) => new EnumerableResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.Type.GetEnumerableType() != null
            };

            var lazyResolver = new ResolverRegistration
            {
                ResolverFactory = (context, typeInfo) => new LazyResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.Type.IsConstructedGenericType && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>)
            };

            var funcResolver = new ResolverRegistration
            {
                ResolverFactory = (context, typeInfo) => new FuncResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.Type.IsConstructedGenericType &&
                                                   FuncResolver.SupportedTypes.Contains(typeInfo.Type.GetGenericTypeDefinition())
            };

            var parentContainerResolver = new ResolverRegistration
            {
                ResolverFactory = (context, typeInfo) => new ParentContainerResolver(context, typeInfo),
                Predicate = (context, typeInfo) => context.Container.ParentContainer != null && context.Container.ParentContainer.CanResolve(typeInfo.Type, typeInfo.DependencyName)
            };

            var defaultValueResolver = new ResolverRegistration
            {
                ResolverFactory = (context, typeInfo) => new DefaultValueResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.HasDefaultValue || typeInfo.Type.GetTypeInfo().IsValueType || typeInfo.Type == typeof(string) || typeInfo.IsMember
            };

            var unknownTypeResolver = new ResolverRegistration
            {
                ResolverFactory = (context, typeInfo) => new UnknownTypeResolver(context, typeInfo),
                Predicate = (context, typeInfo) => !typeInfo.Type.GetTypeInfo().IsAbstract && !typeInfo.Type.GetTypeInfo().IsInterface
            };

            this.resolverSelector.AddResolver(enumerableResolver);
            this.resolverSelector.AddResolver(lazyResolver);
            this.resolverSelector.AddResolver(funcResolver);

            if (this.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled)
                this.resolverSelector.AddResolver(defaultValueResolver);

            if (this.ContainerConfiguration.UnknownTypeResolutionEnabled)
                this.resolverSelector.AddResolver(unknownTypeResolver);

            if (this.ContainerConfiguration.ParentContainerResolutionEnabled)
                this.resolverSelector.AddResolver(parentContainerResolver);
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
