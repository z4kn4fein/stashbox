using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Exceptions;
using System;
using System.Collections.Generic;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents a fluent registration configurator base.
    /// </summary>
    public interface IBaseFluentConfigurator<out TFluentConfigurator>
        where TFluentConfigurator : IBaseFluentConfigurator<TFluentConfigurator>
    {
        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator WithInjectionParameters(params InjectionParameter[] injectionParameters);

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="name">The name of the injection parameter.</param>
        /// <param name="value">The value of the injection parameter.</param>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator WithInjectionParameter(string name, object value);

        /// <summary>
        /// Enables auto member injection on the registration.
        /// </summary>
        /// <param name="rule">The auto member injection rule.</param>
        /// <param name="filter">A filter delegate used to determine which members should be auto injected and which are not.</param>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<TypeInformation, bool> filter = null);

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        /// <param name="rule">The constructor selection rule.</param>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule);

        /// <summary>
        /// Sets the selected constructor.
        /// </summary>
        /// <param name="argumentTypes">The constructor argument types.</param>
        /// <returns>The fluent configurator.</returns>
        /// <exception cref="ConstructorNotFoundException" />
        TFluentConfigurator WithConstructorByArgumentTypes(params Type[] argumentTypes);

        /// <summary>
        /// Sets the selected constructor.
        /// </summary>
        /// <param name="arguments">The constructor arguments.</param>
        /// <returns>The fluent configurator.</returns>
        /// <exception cref="ConstructorNotFoundException" />
        TFluentConfigurator WithConstructorByArguments(params object[] arguments);

        /// <summary>
        /// Set a member (property / field) with the given name as a dependency that should be filled by the container.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator InjectMember(string memberName, object dependencyName = null);

        /// <summary>
        /// Binds a constructor or method parameter to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="dependencyType">The type of the dependency to search for.</param>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The fluent configurator.</returns>
        /// <returns></returns>
        TFluentConfigurator WithDependencyBinding(Type dependencyType, object dependencyName);

        /// <summary>
        /// Binds a constructor or method parameter to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="parameterName">The parameter name of the dependency to search for.</param>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The fluent configurator.</returns>
        /// <returns></returns>
        TFluentConfigurator WithDependencyBinding(string parameterName, object dependencyName);

        /// <summary>
        /// Tells the container that it shouldn't track the resolved transient object for disposal.
        /// </summary>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator WithoutDisposalTracking();

        /// <summary>
        /// Tells the container that it should replace an existing registration with this one.
        /// </summary>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator ReplaceExisting();

        /// <summary>
        /// Tells the container that it shouldn't cache the built expression delegate for this registration.
        /// </summary>
        /// <returns>The fluent configurator.</returns>
        TFluentConfigurator WithoutFactoryCache();
    }
}
