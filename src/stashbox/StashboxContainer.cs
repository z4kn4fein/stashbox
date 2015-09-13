﻿using Sendstorm;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.Resolution.Container;
using Stashbox.Entity;
using Stashbox.Extensions;
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
        private readonly IBuilderContext builderContext;

        public StashboxContainer()
        {
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();
            this.registrationRepository = new RegistrationRepository();
            this.messagePublisher = new MessagePublisher();
            this.builderContext = new BuilderContext(this.registrationRepository, this.messagePublisher, this, new Bag());

            this.RegisterResolvers();
        }

        public void RegisterExtension(IContainerExtension containerExtension)
        {
            this.containerExtensionManager.AddExtension(containerExtension);
        }

        public void RegisterResolver(Func<IBuilderContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory)
        {
            this.resolverSelector.AddResolverStrategy(resolverPredicate, factory);
        }

        private void RegisterResolvers()
        {
            this.resolverSelector.AddResolverStrategy((context, typeInfo) =>
            context.RegistrationRepository.ConstainsTypeKey(typeInfo.Type),
            new ContainerResolverFactory());
        }
    }
}
