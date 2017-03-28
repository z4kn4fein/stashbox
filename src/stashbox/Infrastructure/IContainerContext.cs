using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Utils;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public interface IContainerContext
    {
        /// <summary>
        /// The repository of the registrations.
        /// </summary>
        IRegistrationRepository RegistrationRepository { get; }

        /// <summary>
        /// The delegate repository.
        /// </summary>
        IDelegateRepository DelegateRepository { get; }

        /// <summary>
        /// The decorator repository.
        /// </summary>
        IDecoratorRepository DecoratorRepository { get; }

        /// <summary>
        /// The container itself.
        /// </summary>
        IStashboxContainer Container { get; }

        /// <summary>
        /// The root scope.
        /// </summary>
        IResolutionScope RootScope { get; }

        /// <summary>
        /// The resolution strategy.
        /// </summary>
        IResolutionStrategy ResolutionStrategy { get; }

        /// <summary>
        /// A generic key-value store.
        /// </summary>
        ConcurrentKeyValueStore<object, object> Bag { get; }

        /// <summary>
        /// Indicates that the container should track transient objects for disposal or not.
        /// </summary>
        IContainerConfigurator ContainerConfigurator { get; }

        /// <summary>
        /// Reserves a new registration number.
        /// </summary>
        /// <returns>The registration number.</returns>
        int ReserveRegistrationNumber();
    }
}
