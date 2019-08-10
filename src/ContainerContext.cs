using Stashbox.Configuration;
using Stashbox.Registration;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public class ContainerContext : IContainerContext
    {
        internal ContainerContext(IRegistrationRepository registrationRepository, IStashboxContainer container,
             ContainerConfiguration containerConfiguration, IDecoratorRepository decoratorRepository)
        {
            this.RegistrationRepository = registrationRepository;
            this.Container = container;
            this.ContainerConfiguration = containerConfiguration;
            this.DecoratorRepository = decoratorRepository;
        }

        /// <inheritdoc />
        public IRegistrationRepository RegistrationRepository { get; }

        /// <inheritdoc />
        public IDecoratorRepository DecoratorRepository { get; }

        /// <inheritdoc />
        public IStashboxContainer Container { get; }

        /// <inheritdoc />
        public ContainerConfiguration ContainerConfiguration { get; internal set; }
    }
}
