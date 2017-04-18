using System;
using Stashbox.Registration;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a service registrator.
    /// </summary>
    public interface IServiceRegistrator
    {
        /// <summary>
        /// Creates an <see cref="IRegistrationContext"/>.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The <see cref="IRegistrationContext"/>.</returns>
        IRegistrationContext PrepareContext(Type serviceType, Type implementationType);

        /// <summary>
        /// Creates an <see cref="IRegistrationContext"/>.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="registrationContextData">Existing registration context data.</param>
        /// <returns>The <see cref="IRegistrationContext"/>.</returns>
        IRegistrationContext PrepareContext(Type serviceType, Type implementationType, RegistrationContextData registrationContextData);

        /// <summary>
        /// Creates an <see cref="IDecoratorRegistrationContext"/>.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/>.</returns>
        IDecoratorRegistrationContext PrepareDecoratorContext(Type serviceType, Type implementationType);

        /// <summary>
        /// Registers an <see cref="IRegistrationContext"/>.
        /// </summary>
        /// <param name="registrationContextMeta">The registration context meta.</param>
        /// <param name="isDecorator">True if the requested registration is a decorator.</param>
        /// <param name="replace">True if the container should replace an existing registration.</param>
        /// <returns>The <see cref="IStashboxContainer"/>.</returns>
        IStashboxContainer Register(IRegistrationContextMeta registrationContextMeta, bool isDecorator, bool replace);

        /// <summary>
        /// Remaps an <see cref="IRegistrationContext"/>.
        /// </summary>
        /// <param name="registrationContextMeta">The registration context meta.</param>
        /// <param name="isDecorator">True if the requested registration is a decorator.</param>
        /// <returns>The <see cref="IStashboxContainer"/>.</returns>
        IStashboxContainer ReMap(IRegistrationContextMeta registrationContextMeta, bool isDecorator);

        /// <summary>
        /// Creates a service registration.
        /// </summary>
        /// <param name="registrationContextMeta">The registration context meta.</param>
        /// <param name="isDecorator">True if the requested registration is a decorator.</param>
        /// <returns>The <see cref="IServiceRegistration"/>.</returns>
        IServiceRegistration CreateServiceRegistration(IRegistrationContextMeta registrationContextMeta, bool isDecorator);
    }
}
