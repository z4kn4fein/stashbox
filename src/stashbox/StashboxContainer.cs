using Sendstorm;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.Resolution;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
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
        private readonly IMessagePublisher messagePublisher;
        private readonly IContainerContext containerContext;

        public StashboxContainer()
        {
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
            this.resolverSelectorContainerExcluded = new ResolverSelector();
            this.registrationRepository = new RegistrationRepository();
            this.messagePublisher = new MessagePublisher();
            this.containerContext = new ContainerContext(this.registrationRepository, this.messagePublisher, this, new ResolutionStrategy(this.resolverSelector));

            this.RegisterResolvers();
        }

        public void RegisterExtension(IContainerExtension containerExtension)
        {
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
            if (!disposing) return;
            this.registrationRepository.CleanUp();
        }
    }
}
