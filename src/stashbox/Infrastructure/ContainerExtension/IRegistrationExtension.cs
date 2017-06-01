using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure.ContainerExtension
{
    /// <summary>
    /// Represents a registration extension.
    /// </summary>
    public interface IRegistrationExtension : IContainerExtension
    {
        /// <summary>
        /// Executes when a new service being registered.
        /// </summary>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        /// <param name="serviceRegistration">The service registration.</param>
        void OnRegistration(IContainerContext containerContext, IServiceRegistration serviceRegistration);
    }
}
