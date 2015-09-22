using Sendstorm.Infrastructure;
using Stashbox.Infrastructure;

namespace Stashbox
{
    public class BuilderContext : IBuilderContext
    {
        public BuilderContext(IRegistrationRepository registrationRepository, IMessagePublisher messagePublisher,
            IStashboxContainer container, IResolverSelector resolverSelector)
        {
            this.RegistrationRepository = registrationRepository;
            this.MessagePublisher = messagePublisher;
            this.Container = container;
            this.ResolverSelector = resolverSelector;
        }

        public IRegistrationRepository RegistrationRepository { get; }
        public IMessagePublisher MessagePublisher { get; }
        public IStashboxContainer Container { get; }
        public IResolverSelector ResolverSelector { get; }
    }
}
