using Sendstorm.Infrastructure;

namespace Stashbox.Infrastructure
{
    public interface IBuilderContext
    {
        IRegistrationRepository RegistrationRepository { get; }
        IMessagePublisher MessagePublisher { get; }
        IStashboxContainer Container { get; }
        IResolverSelector ResolverSelector { get; }
    }
}
