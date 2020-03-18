using Stashbox.Configuration;
using Stashbox.Registration;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public class ContainerContext : IContainerContext
    {
        internal ContainerContext(IStashboxContainer container, ContainerConfiguration containerConfiguration)
        {
            this.Container = container;
            this.ContainerConfiguration = containerConfiguration;
            this.RegistrationRepository = new RegistrationRepository();
            this.DecoratorRepository = new DecoratorRepository();
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
