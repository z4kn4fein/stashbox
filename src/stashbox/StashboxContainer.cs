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
        private readonly IRegistrationRepository registrationRepository;
        private readonly IMessagePublisher messagePublisher;
        private readonly IContainerContext containerContext;

        public StashboxContainer()
        {
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
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
            this.resolverSelector.AddResolver(resolverPredicate, factory);
        }

        private void RegisterResolvers()
        {
            this.resolverSelector.AddResolver((context, typeInfo) => context.RegistrationRepository.ConstainsTypeKey(typeInfo),
                new ContainerResolverFactory());

            this.resolverSelector.AddResolver((context, typeInfo) => typeInfo.Type.IsConstructedGenericType && typeInfo.Type.GetGenericTypeDefinition() == typeof(Lazy<>),
                new LazyResolverFactory());

            this.resolverSelector.AddResolver((context, typeInfo) => typeInfo.Type.GetEnumerableType() != null &&
                                              context.RegistrationRepository.ConstainsTypeKey(new TypeInformation { Type = typeInfo.Type.GetEnumerableType() }),
                new EnumerableResolverFactory());
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
