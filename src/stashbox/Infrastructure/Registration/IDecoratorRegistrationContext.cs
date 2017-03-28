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
        /// Tells the container that it shouldn't track the resolved transient object for disposal.
        /// </summary>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IDecoratorRegistrationContext WithoutDisposalTracking();

        /// <summary>
        /// Registers the registration into the container.
        /// </summary>
        /// <returns>The container.</returns>
        IStashboxContainer Register();
    }
}
