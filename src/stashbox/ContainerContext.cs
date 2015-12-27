using Sendstorm.Infrastructure;
using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.MetaInfo;
using System;

namespace Stashbox
{
    public class ContainerContext : IContainerContext
    {
        public ContainerContext(IRegistrationRepository registrationRepository, IStashboxContainer container,
            IResolutionStrategy resolutionStrategy, IMessagePublisher messagePublisher,
            ExtendedImmutableTree<MetaInfoCache> metaInfoRepository,
            ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository)
        {
            this.RegistrationRepository = registrationRepository;
            this.Container = container;
            this.ResolutionStrategy = resolutionStrategy;
            this.MetaInfoRepository = metaInfoRepository;
            this.MessagePublisher = messagePublisher;
            DelegateRepository = delegateRepository;
        }

        public IRegistrationRepository RegistrationRepository { get; }
        public IStashboxContainer Container { get; }
        public IResolutionStrategy ResolutionStrategy { get; }
        public IMessagePublisher MessagePublisher { get; }
        public ExtendedImmutableTree<MetaInfoCache> MetaInfoRepository { get; }
        public ExtendedImmutableTree<Func<ResolutionInfo, object>> DelegateRepository { get; }
    }
}
