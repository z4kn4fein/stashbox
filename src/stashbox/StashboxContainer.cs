using Ronin.Common;
using Stashbox.BuildUp.Resolution;
using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.MetaInfo;
using Stashbox.Registration;
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
        public StashboxContainer()
        {
            this.delegateRepository = new ExtendedImmutableTree<Func<ResolutionInfo, object>>();
            this.metaInfoRepository = new ExtendedImmutableTree<MetaInfoCache>();
            this.disposed = new AtomicBool();
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new ResolutionStrategy(this.resolverSelector), this.metaInfoRepository, this.delegateRepository);

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector, ExtendedImmutableTree<MetaInfoCache> metaInfoRepository,
            ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository)
        {
            this.metaInfoRepository = metaInfoRepository;
            this.delegateRepository = delegateRepository;
            this.disposed = new AtomicBool();
            this.ParentContainer = parentContainer;
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;
            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new CheckParentResolutionStrategyDecorator(new ResolutionStrategy(this.resolverSelector)), this.metaInfoRepository, this.delegateRepository);

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
        public IStashboxContainer CreateChildContainer()
        {
            return new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector.CreateCopy(),
                this.metaInfoRepository, this.delegateRepository);
        }

        /// <inheritdoc />
        public bool IsRegistered<TFrom>(string name = null)
        {
            return this.IsRegistered(typeof(TFrom), name);
        }

        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, string name = null)
        {
            return this.registrationRepository.Constains(typeFrom, name);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

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
        }
    }
}
