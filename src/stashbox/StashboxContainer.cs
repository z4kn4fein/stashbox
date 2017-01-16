using Stashbox.BuildUp.Resolution;
using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.MetaInfo;
using Stashbox.Registration;
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
        private readonly ExtendedImmutableTree<MetaInfoCache> metaInfoRepository;
        private readonly ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository;
        private readonly AtomicBool disposed;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>
        /// </summary>
        public StashboxContainer(bool trackTransientsForDisposal = false)
        {
            this.delegateRepository = new ExtendedImmutableTree<Func<ResolutionInfo, object>>();
            this.metaInfoRepository = new ExtendedImmutableTree<MetaInfoCache>();
            this.disposed = new AtomicBool();
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new ResolutionStrategy(this.resolverSelector), this.metaInfoRepository, this.delegateRepository)
            { TrackTransientsForDisposal = trackTransientsForDisposal };

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector, ExtendedImmutableTree<MetaInfoCache> metaInfoRepository,
            ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository, bool trackTransientsForDisposal)
        {
            this.metaInfoRepository = metaInfoRepository;
            this.delegateRepository = delegateRepository;
            this.disposed = new AtomicBool();
            this.ParentContainer = parentContainer;
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;
            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new CheckParentResolutionStrategyDecorator(new ResolutionStrategy(this.resolverSelector)), this.metaInfoRepository, this.delegateRepository)
            { TrackTransientsForDisposal = trackTransientsForDisposal };

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
        public bool IsRegistered<TFrom>(string name = null)
        {
            return this.IsRegistered(typeof(TFrom), name);
        }

        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, string name = null)
        {
            return this.registrationRepository.Contains(typeFrom, name);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IStashboxContainer BeginScope()
        {
            var container = new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector.CreateCopy(),
                    this.metaInfoRepository, new ExtendedImmutableTree<Func<ResolutionInfo, object>>(), this.ContainerContext.TrackTransientsForDisposal);
            container.OpenScope();
            return container;
        }

        internal void OpenScope()
        {
            foreach (var registrationData in this.ParentContainer.ContainerContext.ScopedRegistrations.GetAll())
            {
                var registration = new ScopedRegistrationContext(registrationData.TypeFrom, registrationData.TypeTo,
                    this.ContainerContext, this.containerExtensionManager);

                registration.InitFromScope(registrationData);
            }
        }

        private void RegisterResolvers()
        {
            var containerResolver = new ResolverRegistration
            {
                ResolverType = typeof(ContainerResolver),
                ResolverFactory = (context, typeInfo) => new ContainerResolver(context, typeInfo),
                Predicate = (context, typeInfo) => context.RegistrationRepository.ConstainsTypeKeyWithConditions(typeInfo)
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
                Predicate = (context, typeInfo) => typeInfo.Type.GetEnumerableType() != null &&
                             context.RegistrationRepository.ConstainsTypeKeyWithConditions(new TypeInformation
                             {
                                 Type = typeInfo.Type.GetEnumerableType()
                             })
            };

            var funcResolver = new ResolverRegistration
            {
                ResolverType = typeof(FuncResolver),
                ResolverFactory = (context, typeInfo) => new FuncResolver(context, typeInfo),
                Predicate = (context, typeInfo) => typeInfo.Type.IsConstructedGenericType && typeInfo.Type.GetGenericTypeDefinition() == typeof(Func<>)
            };

            this.resolverSelector.AddResolver(containerResolver);
            this.resolverSelector.AddResolver(lazyResolver);
            this.resolverSelector.AddResolver(enumerableResolver);
            this.resolverSelector.AddResolver(funcResolver);
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
