using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public class ContainerContext : IContainerContext
    {
        internal ContainerContext(IRegistrationRepository registrationRepository, IStashboxContainer container, 
            IResolutionStrategy resolutionStrategy, IContainerConfigurator containerConfigurator, IDecoratorRepository decoratorRepository)
        {
            this.ResolutionStrategy = resolutionStrategy;
            this.RegistrationRepository = registrationRepository;
            this.Container = container;
            this.ContainerConfigurator = containerConfigurator;
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
        public IContainerConfigurator ContainerConfigurator { get; internal set; }
    }
}
