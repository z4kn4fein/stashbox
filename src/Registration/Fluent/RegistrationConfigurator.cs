using System;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public class RegistrationConfigurator<TService, TImplementation> :
        FluentServiceConfigurator<TService, TImplementation, RegistrationConfigurator<TService, TImplementation>>
    {
        internal RegistrationConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator<TService, TImplementation> WithInstance(TService instance, bool wireUp = false)
        {
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
        {
        }

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory(Func<IDependencyResolver, object> containerFactory, bool isCompiledLambda = false)
        {
            this.Context.ContainerFactory = containerFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return this;
        }

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithFactory(Func<object> singleFactory, bool isCompiledLambda = false)
        {
            this.Context.SingleFactory = singleFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return this;
        }

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The configurator itself.</returns>
        public RegistrationConfigurator WithInstance(object instance, bool wireUp = false)
        {
            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            this.ImplementationType = instance.GetType();
            return this;
        }
    }
}
