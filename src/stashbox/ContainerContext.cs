using Sendstorm.Infrastructure;
using Stashbox.Infrastructure;

namespace Stashbox
{
    public class ContainerContext : IContainerContext
    {
        public ContainerContext(IRegistrationRepository registrationRepository, IMessagePublisher messagePublisher,
            IStashboxContainer container, IResolutionStrategy resolutionStrategy)
        {
            this.RegistrationRepository = registrationRepository;
            this.MessagePublisher = messagePublisher;
            this.Container = container;
            this.ResolutionStrategy = resolutionStrategy;
        }

        public IRegistrationRepository RegistrationRepository { get; }
        public IMessagePublisher MessagePublisher { get; }
        public IStashboxContainer Container { get; }
        public IResolutionStrategy ResolutionStrategy { get; }
    }
}
