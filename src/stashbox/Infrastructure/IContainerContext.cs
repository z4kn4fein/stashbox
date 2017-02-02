using Stashbox.Configuration;
using Stashbox.Registration;
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
        /// The container itself.
        /// </summary>
        IStashboxContainer Container { get; }

        /// <summary>
        /// The resolution strategy.
        /// </summary>
        IResolutionStrategy ResolutionStrategy { get; }
        
        /// <summary>
        /// Repository of scoped registrations.
        /// </summary>
        ConcurrentTree<string, ScopedRegistrationItem> ScopedRegistrations { get; }

        /// <summary>
        /// A generic key-value store.
        /// </summary>
        ConcurrentKeyValueStore<object, object> Bag { get; }

        /// <summary>
        /// The transient objects which are tracked by the container for disposal.
        /// </summary>
        ConcurrentStore<object> TrackedTransientObjects { get; }

        /// <summary>
        /// Indicates that the container should track transient objects for disposal or not.
        /// </summary>
        ContainerConfiguration ContainerConfiguration { get; }

        /// <summary>
        /// Reserves a new registration number.
        /// </summary>
        /// <returns>The registration number.</returns>
        int ReserveRegistrationNumber();
    }
}
