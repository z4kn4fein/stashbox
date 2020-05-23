using Stashbox.Configuration;
using Stashbox.Registration;

namespace Stashbox
{
    internal class ContainerContext : IContainerContext
    {
        public ContainerContext(IStashboxContainer container, ContainerConfiguration containerConfiguration)
        {
            this.Container = container;
            this.ContainerConfiguration = containerConfiguration;
            this.RegistrationRepository = new RegistrationRepository(containerConfiguration);
            this.DecoratorRepository = new DecoratorRepository();
        }

        public IRegistrationRepository RegistrationRepository { get; }

        public IDecoratorRepository DecoratorRepository { get; }

        public IStashboxContainer Container { get; }

        public ContainerConfiguration ContainerConfiguration { get; internal set; }
    }
}
