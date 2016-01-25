using Stashbox.Entity;

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
        /// <param name="registrationInfo">Information about the registration.</param>
        /// <param name="injectionParameters">The injection parameters.</param>
        void OnRegistration(IContainerContext containerContext, RegistrationInfo registrationInfo, InjectionParameter[] injectionParameters = null);
    }
}
