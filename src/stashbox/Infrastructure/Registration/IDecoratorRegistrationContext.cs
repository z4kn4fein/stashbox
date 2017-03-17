using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a decorator registration context.
    /// </summary>
    public interface IDecoratorRegistrationContext
    {
        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IDecoratorRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters);

        /// <summary>
        /// Sets a parameterless factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IDecoratorRegistrationContext WithFactory(Func<object> singleFactory);

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IDecoratorRegistrationContext WithFactory(Func<IStashboxContainer, object> containerFactory);

        /// <summary>
        /// Enables auto member injection on the registration.
        /// </summary>
        /// <param name="rule">The auto member injection rule.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IDecoratorRegistrationContext WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter);

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        /// <param name="rule">The constructor selection rule.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IDecoratorRegistrationContext WithConstructorSelectionRule(Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> rule);

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IDecoratorRegistrationContext WithInstance(object instance);

        /// <summary>
        /// Registers the registration into the container.
        /// </summary>
        /// <returns>The container.</returns>
        IStashboxContainer Register();
    }
}
