using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public class ContainerContext : IContainerContext
    {
        internal ContainerContext(IRegistrationRepository registrationRepository, IStashboxContainer container,
            IResolutionStrategy resolutionStrategy, ContainerConfiguration containerConfiguration, IDecoratorRepository decoratorRepository)
        {
            this.ResolutionStrategy = resolutionStrategy;
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
        public IResolutionStrategy ResolutionStrategy { get; }

        /// <inheritdoc />
        public ContainerConfiguration ContainerConfiguration { get; internal set; }
    }
}
