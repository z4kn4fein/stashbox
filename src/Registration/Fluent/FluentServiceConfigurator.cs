using Stashbox.Entity;
using Stashbox.Lifetime;
using Stashbox.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TConfigurator"></typeparam>
    public class FluentServiceConfigurator<TService, TConfigurator> : FluentServiceConfigurator<TConfigurator>, IFluentServiceConfigurator<TService, TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TService, TConfigurator>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        public FluentServiceConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        /// <inheritdoc />
        public TConfigurator InjectMember<TResult>(Expression<Func<TService, TResult>> expression, object dependencyName = null)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                this.Context.InjectionMemberNames.Add(memberExpression.Member.Name, dependencyName);
                return (TConfigurator)this;
            }

            throw new ArgumentException("The expression must be a member expression (Property or Field)", nameof(expression));
        }

        /// <inheritdoc />
        public TConfigurator WithFinalizer(Action<TService> finalizer)
        {
            base.Context.Finalizer = finalizer;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithInitializer(Action<TService, IDependencyResolver> initializer)
        {
            base.Context.Initializer = initializer;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator AsServiceAlso<TAdditionalService>()
        {
            base.AsServiceAlso(typeof(TAdditionalService));
            return (TConfigurator)this;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TConfigurator"></typeparam>
    public class FluentServiceConfigurator<TConfigurator> : BaseFluentConfigurator<TConfigurator>, IFluentServiceConfigurator<TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TConfigurator>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        internal FluentServiceConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        /// <inheritdoc />
        public TConfigurator WhenDependantIs<TTarget>() where TTarget : class
        {
            this.Context.TargetTypeCondition = typeof(TTarget);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WhenDependantIs(Type targetType)
        {
            this.Context.TargetTypeCondition = targetType;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WhenHas<TAttribute>() where TAttribute : Attribute
        {
            var store = (ArrayStore<Type>)this.Context.AttributeConditions;
            this.Context.AttributeConditions = store.Add(typeof(TAttribute));
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WhenHas(Type attributeType)
        {
            var store = (ArrayStore<Type>)this.Context.AttributeConditions;
            this.Context.AttributeConditions = store.Add(attributeType);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator When(Func<TypeInformation, bool> resolutionCondition)
        {
            this.Context.ResolutionCondition = resolutionCondition;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<IDependencyResolver, object> containerFactory)
        {
            this.Context.ContainerFactory = containerFactory;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<object> singleFactory)
        {
            this.Context.SingleFactory = singleFactory;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithLifetime(ILifetime lifetime)
        {
            this.Context.Lifetime = lifetime;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithScopedLifetime()
        {
            this.Context.Lifetime = new ScopedLifetime();
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithSingletonLifetime()
        {
            this.Context.Lifetime = new SingletonLifetime();
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithName(object name)
        {
            this.Context.Name = name;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithInstance(object instance, bool wireUp = false)
        {
            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator AsImplementedTypes()
        {
            this.AdditionalServiceTypes = new ArrayStore<Type>(this.ImplementationType.GetRegisterableInterfaceTypes()
                    .Concat(this.ImplementationType.GetRegisterableBaseTypes()).CastToArray());
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator InNamedScope(object scopeName) =>
            this.WithLifetime(new NamedScopeLifetime(scopeName));

        /// <inheritdoc />
        public TConfigurator DefinesScope(object scopeName)
        {
            this.Context.DefinedScopeName = scopeName;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithPerResolutionRequestLifetime() =>
            this.WithLifetime(new ResolutionRequestLifetime());

        /// <inheritdoc />
        public TConfigurator AsServiceAlso(Type serviceType)
        {
            if (!this.ImplementationType.Implements(serviceType))
                throw new ArgumentException("The given service type is not assignable from the current implementation type.");

            this.AdditionalServiceTypes = ((ArrayStore<Type>)this.AdditionalServiceTypes).Add(serviceType);
            return (TConfigurator)this;
        }
    }
}
