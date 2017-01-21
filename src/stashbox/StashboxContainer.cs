using Stashbox.BuildUp.Resolution;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
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
        private readonly ExtendedImmutableTree<MetaInfoCache> metaInfoRepository;
        private readonly AtomicBool disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>
        /// </summary>
        public StashboxContainer(Action<ContainerConfiguration> config = null)
        {
            this.metaInfoRepository = new ExtendedImmutableTree<MetaInfoCache>();
            this.disposed = new AtomicBool();
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();

            var configuration = ContainerConfiguration.DefaultContainerConfiguration();
            config?.Invoke(configuration);

            this.registrationRepository = new RegistrationRepository(configuration);

            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new ResolutionStrategy(this.resolverSelector), this.metaInfoRepository,
                new ExtendedImmutableTree<Func<ResolutionInfo, object>>(), configuration);

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector, ExtendedImmutableTree<MetaInfoCache> metaInfoRepository,
            ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository)
        {
            this.metaInfoRepository = metaInfoRepository;
            this.disposed = new AtomicBool();
            this.ParentContainer = parentContainer;
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;
            this.registrationRepository = new RegistrationRepository(parentContainer.ContainerContext.ContainerConfiguration);
            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new CheckParentResolutionStrategyDecorator(new ResolutionStrategy(this.resolverSelector)),
                this.metaInfoRepository, delegateRepository, parentContainer.ContainerContext.ContainerConfiguration);

            this.containerExtensionManager.ReinitalizeExtensions(this.ContainerContext);
        }

        /// <inheritdoc />
        public void RegisterExtension(IContainerExtension containerExtension)
        {
            containerExtension.Initialize(this.ContainerContext);
            this.containerExtensionManager.AddExtension(containerExtension);
        }

        /// <inheritdoc />
        public void RegisterResolver<TResolverType>(Func<IContainerContext, TypeInformation, bool> resolverPredicate,
            Func<IContainerContext, TypeInformation, Resolver> factory)
        {
            var resolver = new ResolverRegistration
            {
                ResolverType = typeof(TResolverType),
                ResolverFactory = factory,
                Predicate = resolverPredicate
            };
            this.resolverSelector.AddResolver(resolver);
        }

        /// <inheritdoc />
        public void RegisterResolver(Type resolverType, Func<IContainerContext, TypeInformation, bool> resolverPredicate,
            Func<IContainerContext, TypeInformation, Resolver> factory)
        {
            var resolver = new ResolverRegistration
            {
                ResolverType = resolverType,
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
            return this.resolverSelector.CanResolve(this.ContainerContext, typeInfo);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IStashboxContainer BeginScope()
        {
            var container = new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector.CreateCopy(),
                    this.metaInfoRepository, new ExtendedImmutableTree<Func<ResolutionInfo, object>>());
            container.OpenScope();
            return container;
        }

        internal void OpenScope()
        {
            foreach (var registrationItem in this.ParentContainer.ContainerContext.ScopedRegistrations.GetAll())
            {
                var registration = new ScopedRegistrationContext(registrationItem.TypeFrom, registrationItem.TypeTo,
                    this.ContainerContext, this.containerExtensionManager);

                registration.InitFromScope(registrationItem.RegistrationContextData.CreateCopy());
            }
        }

        private void RegisterResolvers()
        {
            var containerResolver = new ResolverRegistration
            {
                ResolverType = typeof(ContainerResolver),
                ResolverFactory = (context, typeInfo) => new ContainerResolver(context, typeInfo),
                Predicate = (context, typeInfo) => context.RegistrationRepository.ConstainsRegistrationWithConditions(typeInfo)
            };

            var lazyResolver = new ResolverRegistration
            {
                ResolverType = typeof(LazyResolver),
                ResolverFactory = (context, typeInfo) => new LazyResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.Type.IsConstructedGenericType && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>)
            };

            var enumerableResolver = new ResolverRegistration
            {
                ResolverType = typeof(EnumerableResolver),
                ResolverFactory = (context, typeInfo) => new EnumerableResolver(context, typeInfo),
                Predicate = (context, typeInfo) => (typeInfo.Type.GetEnumerableType() != null || EnumerableResolver.IsAssignableToGenericType(typeInfo.Type, typeof(IEnumerable<>))) &&
                                                    typeInfo.Type != typeof(string)
            };

            var funcResolver = new ResolverRegistration
            {
                ResolverType = typeof(FuncResolver),
                ResolverFactory = (context, typeInfo) => new FuncResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.Type.IsConstructedGenericType &&
                                                   FuncResolver.SupportedTypes.Contains(typeInfo.Type.GetGenericTypeDefinition())
            };

            var parentContainerResolver = new ResolverRegistration
            {
                ResolverType = typeof(ParentContainerResolver),
                ResolverFactory = (context, typeInfo) => new ParentContainerResolver(context, typeInfo),
                Predicate = (context, typeInfo) => context.Container.ParentContainer != null && context.Container.ParentContainer.CanResolve(typeInfo.Type, typeInfo.DependencyName)
            };

            var defaultValueResolver = new ResolverRegistration
            {
                ResolverType = typeof(DefaultValueResolver),
                ResolverFactory = (context, typeInfo) => new DefaultValueResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.HasDefaultValue || typeInfo.Type.GetTypeInfo().IsValueType || typeInfo.Type == typeof(string) || typeInfo.IsMember
            };

            var unknownTypeResolver = new ResolverRegistration
            {
                ResolverType = typeof(UnknownTypeResolver),
                ResolverFactory = (context, typeInfo) => new UnknownTypeResolver(context, typeInfo),
                Predicate = (context, typeInfo) => !typeInfo.Type.GetTypeInfo().IsAbstract && !typeInfo.Type.GetTypeInfo().IsInterface
            };

            this.resolverSelector.AddResolver(containerResolver);
            this.resolverSelector.AddResolver(lazyResolver);
            this.resolverSelector.AddResolver(enumerableResolver);
            this.resolverSelector.AddResolver(funcResolver);

            if (this.ContainerContext.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled)
                this.resolverSelector.AddResolver(defaultValueResolver);

            if (this.ContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled)
                this.resolverSelector.AddResolver(unknownTypeResolver);

            if (this.ContainerContext.ContainerConfiguration.ParentContainerResolutionEnabled)
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
