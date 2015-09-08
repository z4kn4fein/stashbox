using Sendstorm.Infrastructure;
using Stashbox.Extensions;
using Stashbox.Infrastructure;

namespace Stashbox
{
    public class BuilderContext : IBuilderContext
    {
        public BuilderContext(IRegistrationRepository registrationRepository, IMessagePublisher messagePublisher, IStashboxContainer container, Bag bag)
        {
            this.RegistrationRepository = registrationRepository;
            this.MessagePublisher = messagePublisher;
            this.Container = container;
            this.Bag = bag;
        }

        public Bag Bag { get; }
        public IRegistrationRepository RegistrationRepository { get; }
        public IMessagePublisher MessagePublisher { get; }
        public IStashboxContainer Container { get; }
    }
}
