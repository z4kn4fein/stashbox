using Stashbox.Utils.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public class FluentServiceConfigurator<TService, TImplementation, TConfigurator> : FluentServiceConfigurator<TConfigurator>, IFluentCompositor<TImplementation, TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TService, TImplementation, TConfigurator>
    {
        internal FluentServiceConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        internal FluentServiceConfigurator(Type serviceType, Type implementationType, RegistrationContext registrationContext)
            : base(serviceType, implementationType, registrationContext)
        { }

        /// <inheritdoc />
        public TConfigurator InjectMember<TResult>(Expression<Func<TImplementation, TResult>> expression, object dependencyName = null)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                this.Context.InjectionMemberNames.Add(memberExpression.Member.Name, dependencyName);
                return (TConfigurator)this;
            }

            throw new ArgumentException("The expression must be a member expression (Property or Field)", nameof(expression));
        }

        /// <inheritdoc />
        public TConfigurator WithFinalizer(Action<TImplementation> finalizer)
        {
            base.Context.Finalizer = finalizer;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithInitializer(Action<TImplementation, IDependencyResolver> initializer)
        {
            base.Context.Initializer = initializer;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<TImplementation> singleFactory, bool isCompiledLambda = false)
        {
            this.Context.SingleFactory = singleFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<IDependencyResolver, TImplementation> containerFactory, bool isCompiledLambda = false)
        {
            this.Context.ContainerFactory = containerFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return (TConfigurator)this;
        }
    }

    /// <summary>
    /// Represents the fluent service registration api.
    /// </summary>
    public class FluentServiceConfigurator<TConfigurator> : BaseFluentConfigurator<TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TConfigurator>
    {
        internal FluentServiceConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        internal FluentServiceConfigurator(Type serviceType, Type implementationType, RegistrationContext registrationContext)
            : base(serviceType, implementationType, registrationContext)
        { }

        /// <summary>
        /// Sets the name of the registration.
        /// </summary>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithName(object name)
        {
            this.Context.Name = name;
            return (TConfigurator)this;
        }

        /// <summary>
        /// It means this registration would be used as a logical scope for it's dependencies, the dependencies registered with the <see cref="BaseFluentConfigurator{TConfigurator}.InNamedScope"/> and with the same name as it's param will be preferred during resolution.
        /// </summary>
        /// <param name="scopeName">The name of the scope. When the name == null, the type which defines the scope is used as name.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator DefinesScope(object scopeName = null)
        {
            this.Context.DefinedScopeName = scopeName ?? this.ImplementationType;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Registers the given service by all of it's implemented types.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator AsImplementedTypes()
        {
            this.Context.AdditionalServiceTypes = this.ImplementationType.GetRegisterableInterfaceTypes()
                .Concat(this.ImplementationType.GetRegisterableBaseTypes()).AsExpandableArray();
            return (TConfigurator)this;
        }

        /// <summary>
        /// Binds the currently configured registration to an additional service type.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator AsServiceAlso<TAdditionalService>() =>
            this.AsServiceAlso(typeof(TAdditionalService));

        /// <summary>
        /// Binds the currently configured registration to an additional service type.
        /// </summary>
        /// <param name="serviceType">The additional service type.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator AsServiceAlso(Type serviceType)
        {
            if (!this.ImplementationType.Implements(serviceType))
                throw new ArgumentException($"The implementation type {base.ImplementationType} does not implement the given service type {serviceType}.");

            ((ExpandableArray<Type>)this.Context.AdditionalServiceTypes).Add(serviceType);
            return (TConfigurator)this;
        }
    }
}
