using Stashbox.Configuration;
using Stashbox.Registration;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public interface IContainerContext
    {
        /// <summary>
        /// The service registration repository.
        /// </summary>
        IRegistrationRepository RegistrationRepository { get; }

        /// <summary>
        /// The service decorator registration repository.
        /// </summary>
        IDecoratorRepository DecoratorRepository { get; }

        /// <summary>
        /// The parent container context.
        /// </summary>
        IContainerContext ParentContext { get; }

        /// <summary>
        /// The parent container context.
        /// </summary>
        IResolutionScope RootScope { get; }

        /// <summary>
        /// The container configuration.
        /// </summary>
        ContainerConfiguration ContainerConfiguration { get; }
    }
}
