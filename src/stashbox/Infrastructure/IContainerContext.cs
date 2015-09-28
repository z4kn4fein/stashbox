using Sendstorm.Infrastructure;

namespace Stashbox.Infrastructure
{
    public interface IContainerContext
    {
        IRegistrationRepository RegistrationRepository { get; }
        IMessagePublisher MessagePublisher { get; }
        IStashboxContainer Container { get; }
        IResolutionStrategy ResolutionStrategy { get; }
    }
}
