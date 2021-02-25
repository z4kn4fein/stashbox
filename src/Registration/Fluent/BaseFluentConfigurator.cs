using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data;
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

        internal BaseFluentConfigurator(Type serviceType, Type implementationType, RegistrationContext registrationContext)
            : base(serviceType, implementationType, registrationContext)
        { }

        /// <summary>
        /// Sets the lifetime of the registration.
        /// </summary>
        /// <param name="lifetime">An <see cref="LifetimeDescriptor"/> implementation.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithLifetime(LifetimeDescriptor lifetime)
        {
            this.Context.Lifetime = lifetime;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets scoped lifetime for the registration.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithScopedLifetime() => this.WithLifetime(Lifetimes.Scoped);

        /// <summary>
        /// Sets singleton lifetime for the registration.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithSingletonLifetime() => this.WithLifetime(Lifetimes.Singleton);

        /// <summary>
        /// Sets transient lifetime for the registration.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithTransientLifetime() => this.WithLifetime(Lifetimes.Transient);

        /// <summary>
        /// Sets the lifetime to <see cref="PerScopedRequestLifetime"/>. This lifetime will create a new instance between scoped services. This means
        /// that every scoped service will get a different instance but within their dependency tree it will behave as a singleton. 
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithPerScopedRequestLifetime() => this.WithLifetime(Lifetimes.PerScopedRequest);

        /// <summary>
        /// Sets a scope name condition for the registration, it will be used only when a scope with the given name requests it.
        /// </summary>
        /// <param name="scopeName">The name of the scope.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator InNamedScope(object scopeName) => this.WithLifetime(Lifetimes.NamedScope(scopeName));

        /// <summary>
        /// Sets a condition for the registration that it will be used only within the scope defined by the given type.
        /// </summary>
        /// <param name="type">The type which defines the scope.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator InScopeDefinedBy(Type type) => this.WithLifetime(Lifetimes.NamedScope(type));

        /// <summary>
        /// Sets a condition for the registration that it will be used only within the scope defined by the given type.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator InScopeDefinedBy<TScopeDefiner>() => this.WithLifetime(Lifetimes.NamedScope(typeof(TScopeDefiner)));

        /// <summary>
        /// Binds a constructor/method parameter or a property/field to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithDependencyBinding<TDependency>(object dependencyName = null) =>
            this.WithDependencyBinding(typeof(TDependency), dependencyName);

        /// <summary>
        /// Binds a constructor/method parameter or a property/field to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="dependencyType">The type of the dependency to search for.</param>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithDependencyBinding(Type dependencyType, object dependencyName = null)
        {
            Shield.EnsureNotNull(dependencyType, nameof(dependencyType));

            this.Context.DependencyBindings.Add(dependencyType, dependencyName);

            return (TConfigurator)this;
        }

        /// <summary>
        /// Binds a constructor/method parameter or a property/field to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="parameterName">The parameter name of the dependency to search for.</param>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithDependencyBinding(string parameterName, object dependencyName = null)
        {
            Shield.EnsureNotNull(parameterName, nameof(parameterName));

            this.Context.DependencyBindings.Add(parameterName, dependencyName);

            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parent target condition for the registration.
        /// </summary>
        /// <typeparam name="TTarget">The type of the parent.</typeparam>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenDependantIs<TTarget>() where TTarget : class => this.WhenDependantIs(typeof(TTarget));

        /// <summary>
        /// Sets a parent target condition for the registration.
        /// </summary>
        /// <param name="targetType">The type of the parent.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenDependantIs(Type targetType)
        {
            Shield.EnsureNotNull(targetType, nameof(targetType));

            this.Context.TargetTypeConditions.Add(targetType);
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenHas<TAttribute>() where TAttribute : Attribute => this.WhenHas(typeof(TAttribute));

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenHas(Type attributeType)
        {
            this.Context.AttributeConditions.Add(attributeType);
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a generic condition for the registration.
        /// </summary>
        /// <param name="resolutionCondition">The predicate.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator When(Func<TypeInformation, bool> resolutionCondition)
        {
            this.Context.ResolutionConditions.Add(resolutionCondition);
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithInjectionParameters(params KeyValuePair<string, object>[] injectionParameters)
        {
            this.Context.InjectionParameters.AddRange(injectionParameters);
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
            this.Context.InjectionParameters.Add(new KeyValuePair<string, object>(name, value));
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
            this.Context.AutoMemberInjectionFilter = filter;
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
                ThrowConstructorNotFoundException(this.ImplementationType, argumentTypes);

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
                ThrowConstructorNotFoundException(this.ImplementationType, argTypes);

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
        [Obsolete("Use WithDependencyBinding() instead.")]
        public TConfigurator InjectMember(string memberName, object dependencyName = null) =>
            this.WithDependencyBinding(memberName, dependencyName);

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
        /// Tells the container that it should replace an existing registration with the current one, or add it if there is no existing found.
        /// </summary>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator ReplaceExisting()
        {
            this.Context.ReplaceExistingRegistration = true;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Tells the container that it should replace an existing registration with the current one, but only if there is an existing registration.
        /// </summary>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator ReplaceOnlyIfExists()
        {
            this.Context.ReplaceExistingRegistrationOnlyIfExists = true;
            return (TConfigurator)this;
        }

        private protected void SetFactory(Delegate factory, bool isCompiledLambda, params Type[] parameterTypes)
        {
            this.Context.Factory = factory;
            this.Context.FactoryParameters = parameterTypes;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
        }

        private static void ThrowConstructorNotFoundException(Type type, params Type[] argTypes)
        {
            throw argTypes.Length switch
            {
                0 => new ConstructorNotFoundException(type),
                1 => new ConstructorNotFoundException(type, argTypes[0]),
                _ => new ConstructorNotFoundException(type, argTypes)
            };
        }
    }
}
