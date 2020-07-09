using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public class FluentServiceConfigurator<TService, TImplementation, TConfigurator> : FluentServiceConfigurator<TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TService, TImplementation, TConfigurator>
    {
        internal FluentServiceConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Set a member (property / field) as a dependency should be filled by the container.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator InjectMember<TResult>(Expression<Func<TImplementation, TResult>> expression, object dependencyName = null)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                this.Context.InjectionMemberNames.Add(memberExpression.Member.Name, dependencyName);
                return (TConfigurator)this;
            }

            throw new ArgumentException("The expression must be a member expression (Property or Field)", nameof(expression));
        }

        /// <summary>
        /// Sets a delegate which will be called when the container is being disposed.
        /// </summary>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithFinalizer(Action<TImplementation> finalizer)
        {
            base.Context.Finalizer = finalizer;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a delegate which will be called when the service is being constructed.
        /// </summary>
        /// <param name="initializer">The initializer delegate.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithInitializer(Action<TImplementation, IDependencyResolver> initializer)
        {
            base.Context.Initializer = initializer;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithFactory(Func<TService> singleFactory, bool isCompiledLambda = false)
        {
            this.Context.SingleFactory = singleFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithFactory(Func<IDependencyResolver, TService> containerFactory, bool isCompiledLambda = false)
        {
            this.Context.ContainerFactory = containerFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="wireUp">If true, the instance will be wired into the container, it will perform member and method injection on it.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithInstance(TService instance, bool wireUp = false)
        {
            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            this.ImplementationType = instance.GetType();
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

        /// <summary>
        /// Binds a constructor or method parameter to a named registration, so the container will perform a named resolution on the bound dependency.  
        /// </summary>
        /// <param name="dependencyName">The name of the bound named registration.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithDependencyBinding<TDependency>(object dependencyName) =>
            this.WithDependencyBinding(typeof(TDependency), dependencyName);

        /// <summary>
        /// Sets a parent target condition for the registration.
        /// </summary>
        /// <typeparam name="TTarget">The type of the parent.</typeparam>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenDependantIs<TTarget>() where TTarget : class
        {
            this.Context.TargetTypeCondition = typeof(TTarget);
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parent target condition for the registration.
        /// </summary>
        /// <param name="targetType">The type of the parent.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenDependantIs(Type targetType)
        {
            this.Context.TargetTypeCondition = targetType;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenHas<TAttribute>() where TAttribute : Attribute
        {
            var store = (ExpandableArray<Type>)this.Context.AttributeConditions;
            store.Add(typeof(TAttribute));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WhenHas(Type attributeType)
        {
            var store = (ExpandableArray<Type>)this.Context.AttributeConditions;
            store.Add(attributeType);
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a generic condition for the registration.
        /// </summary>
        /// <param name="resolutionCondition">The predicate.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator When(Func<TypeInformation, bool> resolutionCondition)
        {
            this.Context.ResolutionCondition = resolutionCondition;
            return (TConfigurator)this;
        }

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
        /// Sets a scoped lifetime for the registration.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithScopedLifetime() => this.WithLifetime(Lifetimes.Scoped);

        /// <summary>
        /// Sets a singleton lifetime for the registration.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithSingletonLifetime() => this.WithLifetime(Lifetimes.Singleton);

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
        /// Registers the given service by all of it's implemented types.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator AsImplementedTypes()
        {
            this.Context.AdditionalServiceTypes = ExpandableArray<Type>.FromEnumerable(this.ImplementationType.GetRegisterableInterfaceTypes()
                    .Concat(this.ImplementationType.GetRegisterableBaseTypes()));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a scope name condition for the registration, it will be used only when a scope with the given name requests it.
        /// </summary>
        /// <param name="scopeName">The name of the scope.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator InNamedScope(object scopeName)
        {
            this.Context.NamedScopeRestrictionIdentifier = scopeName;
            return this.WithLifetime(Lifetimes.NamedScope);
        }

        /// <summary>
        /// Sets a condition for the registration that it will be used only within the scope defined by the given type.
        /// </summary>
        /// <param name="type">The type which defines the scope.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator InScopeDefinedBy(Type type)
        {
            this.Context.NamedScopeRestrictionIdentifier = type;
            return this.WithLifetime(Lifetimes.NamedScope);
        }

        /// <summary>
        /// Sets a condition for the registration that it will be used only within the scope defined by the given type.
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator InScopeDefinedBy<TScopeDefiner>()
        {
            this.Context.NamedScopeRestrictionIdentifier = typeof(TScopeDefiner);
            return this.WithLifetime(Lifetimes.NamedScope);
        }

        /// <summary>
        /// It means this registration would be used as a logical scope for it's dependencies, the dependencies registered with the <see cref="InNamedScope"/> and with the same name as it's param will be preferred during resolution.
        /// </summary>
        /// <param name="scopeName">The name of the scope. When the name == null, the type which defines the scope is used as name.</param>
        /// <returns>The configurator itself.</returns>
        public TConfigurator DefinesScope(object scopeName = null)
        {
            this.Context.DefinedScopeName = scopeName ?? this.ImplementationType;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets the lifetime to <see cref="PerScopedRequestLifetime"/>. This lifetime will create a new instance between scoped services. This means
        /// that every scoped service will get a different instance but within their dependency tree it will behave as a singleton. 
        /// </summary>
        /// <returns>The configurator itself.</returns>
        public TConfigurator WithPerScopedRequestLifetime() =>
            this.WithLifetime(Lifetimes.PerScopedRequest);

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
