using Sendstorm.Infrastructure;
using Stashbox.Extensions;

namespace Stashbox.Infrastructure
{
    public interface IBuilderContext
    {
        Bag Bag { get; }
        IRegistrationRepository RegistrationRepository { get; }
        IMessagePublisher MessagePublisher { get; }
        IStashboxContainer Container { get; }
    }
}
