using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the base of the fluent registration api.
    /// </summary>
    /// <typeparam name="TConfigurator"></typeparam>
    public class BaseFluentConfigurator<TConfigurator> : RegistrationConfiguration
        where TConfigurator : BaseFluentConfigurator<TConfigurator>
    {
        internal BaseFluentConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithInjectionParameters(params KeyValuePair<string, object>[] injectionParameters)
        {
            var length = injectionParameters.Length;
            for (var i = 0; i < length; i++)
                this.AddInjectionParameter(injectionParameters[i]);
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="name">The name of the injection parameter.</param>
        /// <param name="value">The value of the injection parameter.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithInjectionParameter(string name, object value)
        {
            this.AddInjectionParameter(new KeyValuePair<string, object>(name, value));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Enables auto member injection on the registration.
        /// </summary>
        /// <param name="rule">The auto member injection rule.</param>
        /// <param name="filter">A filter delegate used to determine which members should be auto injected and which are not.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<MemberInfo, bool> filter = null)
        {
            this.Context.AutoMemberInjectionEnabled = true;
            this.Context.AutoMemberInjectionRule = rule;
            this.Context.MemberInjectionFilter = filter;
            return (TConfigurator)this;
        }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        /// <param name="rule">The constructor selection rule.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> rule)
        {
            this.Context.ConstructorSelectionRule = rule;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets the selected constructor.
        /// </summary>
        /// <param name="argumentTypes">The constructor argument types.</param>
        /// <returns>The fluent configurator.</returns>
        /// <exception cref="ConstructorNotFoundException" />
        public TConfigurator WithConstructorByArgumentTypes(params Type[] argumentTypes)
        {
            var constructor = this.ImplementationType.GetConstructorByArguments(argumentTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argumentTypes);

            this.Context.SelectedConstructor = constructor;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets the selected constructor.
        /// </summary>
        /// <param name="arguments">The constructor arguments.</param>
        /// <returns>The fluent configurator.</returns>
        /// <exception cref="ConstructorNotFoundException" />
        public TConfigurator WithConstructorByArguments(params object[] arguments)
        {
            var argTypes = arguments.Select(arg => arg.GetType()).ToArray();
            var constructor = this.ImplementationType.GetConstructorByArguments(argTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argTypes);

            this.Context.SelectedConstructor = constructor;
            this.Context.ConstructorArguments = arguments;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Set a member (property / field) with the given name as a dependency that should be filled by the container.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator InjectMember(string memberName, object dependencyName = null)
        {
            this.Context.InjectionMemberNames.Add(memberName, dependencyName);
            return (TConfigurator)this;
        }

        /// <summary>
        /// Binds a constructor or method parameter to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="dependencyType">The type of the dependency to search for.</param>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithDependencyBinding(Type dependencyType, object dependencyName)
        {
            Shield.EnsureNotNull(dependencyType, nameof(dependencyType));
            Shield.EnsureNotNull(dependencyName, nameof(dependencyName));

            this.Context.DependencyBindings.Add(dependencyType, dependencyName);

            return (TConfigurator)this;
        }

        /// <summary>
        /// Binds a constructor or method parameter to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="parameterName">The parameter name of the dependency to search for.</param>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithDependencyBinding(string parameterName, object dependencyName)
        {
            Shield.EnsureNotNull(parameterName, nameof(parameterName));
            Shield.EnsureNotNull(dependencyName, nameof(dependencyName));

            this.Context.DependencyBindings.Add(parameterName, dependencyName);

            return (TConfigurator)this;
        }

        /// <summary>
        /// Tells the container that it shouldn't track the resolved transient object for disposal.
        /// </summary>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithoutDisposalTracking()
        {
            this.Context.IsLifetimeExternallyOwned = true;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Tells the container that it should replace an existing registration with this one.
        /// </summary>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator ReplaceExisting()
        {
            this.Context.ReplaceExistingRegistration = true;
            return (TConfigurator)this;
        }

        private void AddInjectionParameter(KeyValuePair<string, object> injectionParameter)
        {
            var store = (ExpandableArray<KeyValuePair<string, object>>)this.Context.InjectionParameters;
            store.Add(injectionParameter);
        }

        private void ThrowConstructorNotFoundException(Type type, params Type[] argTypes)
        {
            if (argTypes.Length == 0)
                throw new ConstructorNotFoundException(type);

            if (argTypes.Length == 1)
                throw new ConstructorNotFoundException(type, argTypes[0]);

            throw new ConstructorNotFoundException(type, argTypes);
        }
    }
}
