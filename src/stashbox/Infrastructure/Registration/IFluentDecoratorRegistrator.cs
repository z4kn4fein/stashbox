using System;
using System.Collections.Generic;
using System.Reflection;
using Stashbox.Configuration;
using Stashbox.Entity;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a fluent decorator registrator.
    /// </summary>
    public interface IFluentDecoratorRegistrator
    {
        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IFluentDecoratorRegistrator WithInjectionParameters(params InjectionParameter[] injectionParameters);

        /// <summary>
        /// Enables auto member injection on the registration.
        /// </summary>
        /// <param name="rule">The auto member injection rule.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IFluentDecoratorRegistrator WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter);

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        /// <param name="rule">The constructor selection rule.</param>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IFluentDecoratorRegistrator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> rule);

        /// <summary>
        /// Tells the container that it shouldn't track the resolved transient object for disposal.
        /// </summary>
        /// <returns>The <see cref="IDecoratorRegistrationContext"/> which on this method was called.</returns>
        IFluentDecoratorRegistrator WithoutDisposalTracking();
    }
}
