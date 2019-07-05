using Stashbox.Registration;
using Stashbox.Resolution;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public interface IContainerContext
    {
        /// <summary>
        /// The rservice registration repository.
        /// </summary>
        IRegistrationRepository RegistrationRepository { get; }

        /// <summary>
        /// The service decorator registration repository.
        /// </summary>
        IDecoratorRepository DecoratorRepository { get; }

        /// <summary>
        /// The container itself.
        /// </summary>
        IStashboxContainer Container { get; }

        /// <summary>
        /// The resolution strategy.
        /// </summary>
        IResolutionStrategy ResolutionStrategy { get; }

        /// <summary>
        /// The container configurator.
        /// </summary>
        IContainerConfigurator ContainerConfigurator { get; }
    }
}
