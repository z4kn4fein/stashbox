using Stashbox.Entity;
using System;

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
        /// <param name="typeTo">The implementation type.</param>
        /// <param name="typeFrom">The interface type.</param>
        /// <param name="injectionParameters">The injection parameters.</param>
        void OnRegistration(IContainerContext containerContext, Type typeTo, Type typeFrom, InjectionParameter[] injectionParameters = null);
    }
}
