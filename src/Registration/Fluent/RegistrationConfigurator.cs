using Stashbox.Utils;
using System;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public class RegistrationConfigurator<TService, TImplementation> :
        FluentServiceConfigurator<TService, TImplementation, RegistrationConfigurator<TService, TImplementation>>
        where TService : class
        where TImplementation : class, TService
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The fluent configurator.</returns>
        public RegistrationConfigurator<TService, TImplementation> WithInstance(TService instance, bool wireUp = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            this.ImplementationType = instance.GetType();
            return this;
        }
    }

    /// <summary>
    /// Represents the fluent service registration api.
    /// </summary>
    public class RegistrationConfigurator : FluentServiceConfigurator<RegistrationConfigurator>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The fluent configurator.</returns>
        public RegistrationConfigurator WithInstance(object instance, bool wireUp = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            this.ImplementationType = instance.GetType();
            return this;
        }
    }
}
