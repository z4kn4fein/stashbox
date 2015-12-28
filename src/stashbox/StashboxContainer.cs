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
    public partial class StashboxContainer : IStashboxContainer
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IResolverSelector resolverSelector;
        private readonly IResolverSelector resolverSelectorContainerExcluded;
        private readonly IRegistrationRepository registrationRepository;
        private readonly ExtendedImmutableTree<MetaInfoCache> metaInfoRepository;
        private readonly ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository;
        private readonly AtomicBool disposed;

        public StashboxContainer()
        {
            this.delegateRepository = new ExtendedImmutableTree<Func<ResolutionInfo, object>>();
            this.metaInfoRepository = new ExtendedImmutableTree<MetaInfoCache>();
            this.disposed = new AtomicBool();
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
            this.resolverSelectorContainerExcluded = new ResolverSelector();
            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new ResolutionStrategy(this.resolverSelector), this.metaInfoRepository, this.delegateRepository);

            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector, IResolverSelector resolverSelectorContainerExcluded, ExtendedImmutableTree<MetaInfoCache> metaInfoRepository,
            ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository)
        {
            this.metaInfoRepository = metaInfoRepository;
            this.delegateRepository = delegateRepository;
            this.disposed = new AtomicBool();
            this.ParentContainer = parentContainer;
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;
            this.resolverSelectorContainerExcluded = resolverSelectorContainerExcluded;
            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, this,
                new CheckParentResolutionStrategyDecorator(new ResolutionStrategy(this.resolverSelector)), this.metaInfoRepository, this.delegateRepository);
        }

        public void RegisterExtension(IContainerExtension containerExtension)
        {
            containerExtension.Initialize(this.ContainerContext);
            this.containerExtensionManager.AddExtension(containerExtension);
        }

        public void RegisterResolver(Func<IContainerContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory)
        {
            this.resolverSelector.AddResolver(new ResolverRegistration
            {
                ResolverFactory = factory,
                Predicate = resolverPredicate
            });
        }

        public IStashboxContainer CreateChildContainer()
        {
            return new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector.CreateCopy(),
                this.resolverSelectorContainerExcluded.CreateCopy(), this.metaInfoRepository, this.delegateRepository);
        }

        public IStashboxContainer ParentContainer { get; }
        public IContainerContext ContainerContext { get; }

        private void RegisterResolvers()
        {
            var containerResolver = new ResolverRegistration
            {
                ResolverFactory = new ContainerResolverFactory(),
                Predicate = (context, typeInfo) => context.RegistrationRepository.ConstainsTypeKeyWithConditions(typeInfo)
            };

            var lazyResolver = new ResolverRegistration
            {
                ResolverFactory = new LazyResolverFactory(),
                Predicate = (context, typeInfo) => typeInfo.Type.IsConstructedGenericType && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>)
            };

            var enumerableResolver = new ResolverRegistration
            {
                ResolverFactory = new EnumerableResolverFactory(),
                Predicate = (context, typeInfo) => typeInfo.Type.GetEnumerableType() != null &&
                             context.RegistrationRepository.ConstainsTypeKeyWithConditions(new TypeInformation
                             {
                                 Type = typeInfo.Type.GetEnumerableType()
                             })
            };

            this.resolverSelector.AddResolver(containerResolver);
            this.resolverSelector.AddResolver(lazyResolver);
            this.resolverSelectorContainerExcluded.AddResolver(lazyResolver);
            this.resolverSelector.AddResolver(enumerableResolver);
            this.resolverSelectorContainerExcluded.AddResolver(enumerableResolver);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed.CompareExchange(false, true)) return;
            if (!disposing) return;
            this.registrationRepository.CleanUp();
        }
    }
}
