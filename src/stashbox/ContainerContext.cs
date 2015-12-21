using Stashbox.Infrastructure;

namespace Stashbox
{
    public class ContainerContext : IContainerContext
    {
        public ContainerContext(IRegistrationRepository registrationRepository,
            IStashboxContainer container, IResolutionStrategy resolutionStrategy)
        {
            this.RegistrationRepository = registrationRepository;
            this.Container = container;
            this.ResolutionStrategy = resolutionStrategy;
        }

        public IRegistrationRepository RegistrationRepository { get; }
        public IStashboxContainer Container { get; }
        public IResolutionStrategy ResolutionStrategy { get; }
    }
}
