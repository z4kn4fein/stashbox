using Sendstorm;
using Sendstorm.Infrastructure;
using Stashbox.Build.Resolution;
using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.Registration;
using System;

namespace Stashbox
{
    public partial class StashboxContainer : IStashboxContainer
    {
        private readonly IBuildExtensionManager buildExtensionManager;
        private readonly IResolverSelector resolverSelector;
        private readonly IRegistrationRepository registrationRepository;
        private readonly IMessagePublisher messagePublisher;
        private readonly IBuilderContext builderContext;
        private readonly Bag bag;

        public StashboxContainer()
        {
            this.buildExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
            this.registrationRepository = new RegistrationRepository();
            this.messagePublisher = new MessagePublisher();
            this.bag = new Bag();
            this.builderContext = new BuilderContext(this.registrationRepository, this.messagePublisher, this, this.bag);

            this.RegisterResolvers();
        }

        public void RegisterBuildExtension(BuildExtension buildExtension)
        {
            this.buildExtensionManager.AddExtension(buildExtension);
        }

        public void RegisterResolver(Func<IBuilderContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory)
        {
            this.resolverSelector.AddResolverStrategy(resolverPredicate, factory);
        }

        private void RegisterResolvers()
        {
            this.resolverSelector.AddResolverStrategy((builderContext, typeInfo) =>
            builderContext.RegistrationRepository.ConstainsTypeKey(typeInfo.Type),
            new ContainerResolverFactory());
        }
    }
}
