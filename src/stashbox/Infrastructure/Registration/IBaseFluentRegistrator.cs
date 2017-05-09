using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Exceptions;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a fluent registrator base.
    /// </summary>
    public interface IBaseFluentRegistrator<out TFluentRegistrator>
    {
        /// <summary>
        /// The service type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The fluent registrator.</returns>
        TFluentRegistrator WithInjectionParameters(params InjectionParameter[] injectionParameters);

        /// <summary>
        /// Enables auto member injection on the registration.
        /// </summary>
        /// <param name="rule">The auto member injection rule.</param>
        /// <returns>The fluent registrator.</returns>
        TFluentRegistrator WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter);

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        /// <param name="rule">The constructor selection rule.</param>
        /// <returns>The fluent registrator.</returns>
        TFluentRegistrator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule);

        /// <summary>
        /// Sets the selected constructor.
        /// </summary>
        /// <param name="argumentTypes">The constructor argument types.</param>
        /// <returns>The fluent registrator.</returns>
        /// <exception cref="ConstructorNotFoundException" />
        TFluentRegistrator WithConstructorByArgumentTypes(params Type[] argumentTypes);

        /// <summary>
        /// Sets the selected constructor.
        /// </summary>
        /// <param name="arguments">The constructor arguments.</param>
        /// <returns>The fluent registrator.</returns>
        /// <exception cref="ConstructorNotFoundException" />
        TFluentRegistrator WithConstructorByArguments(params object[] arguments);

        /// <summary>
        /// Tells the container that it shouldn't track the resolved transient object for disposal.
        /// </summary>
        /// <returns>The fluent registrator.</returns>
        TFluentRegistrator WithoutDisposalTracking();

        /// <summary>
        /// Tells the container that it should replace an existing registration with this one.
        /// </summary>
        /// <returns>The fluent registrator.</returns>
        TFluentRegistrator ReplaceExisting();
    }
}
