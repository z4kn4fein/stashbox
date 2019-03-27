using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Registration
{
    internal class RegistrationContext<TService> : RegistrationContext, IRegistrationContext<TService>
    {
        public RegistrationContext(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        public IFluentServiceRegistrator<TService> InjectMember<TResult>(Expression<Func<TService, TResult>> expression, object dependencyName = null)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                this.Context.InjectionMemberNames.Add(memberExpression.Member.Name, dependencyName);
                return this;
            }

            throw new ArgumentException("The expression must be a member expression (Property or Field)", nameof(expression));
        }

        public IFluentServiceRegistrator<TService> WithFinalizer(Action<TService> finalizer)
        {
            base.Context.Finalizer = finalizer;
            return this;
        }

        public IFluentServiceRegistrator<TService> WithInitializer(Action<TService, IDependencyResolver> initializer)
        {
            base.Context.Initializer = initializer;
            return this;
        }

        public IFluentServiceRegistrator AsServiceAlso<TAdditionalService>()
        {
            base.AsServiceAlso(typeof(TAdditionalService));
            return this;
        }
    }

    internal class RegistrationContext : IRegistrationContext
    {
        public Type ServiceType { get; }

        public Type ImplementationType { get; }

        public RegistrationContextData Context { get; }

        public bool ReplaceExistingRegistration { get; private set; }

        public ArrayStore<Type> AdditionalServiceTypes { get; private set; }

        public RegistrationContext(Type serviceType, Type implementationType)
            : this(serviceType, implementationType, RegistrationContextData.New())
        { }

        public RegistrationContext(Type serviceType, Type implementationType, RegistrationContextData registrationContextData)
        {
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Context = registrationContextData;
            this.AdditionalServiceTypes = ArrayStore<Type>.Empty;
        }

        public IFluentServiceRegistrator WhenDependantIs<TTarget>() where TTarget : class
        {
            this.Context.TargetTypeCondition = typeof(TTarget);
            return this;
        }

        public IFluentServiceRegistrator WhenDependantIs(Type targetType)
        {
            this.Context.TargetTypeCondition = targetType;
            return this;
        }

        public IFluentServiceRegistrator WhenHas<TAttribute>() where TAttribute : Attribute
        {
            var store = (ArrayStore<Type>)this.Context.AttributeConditions;
            this.Context.AttributeConditions = store.Add(typeof(TAttribute));
            return this;
        }

        public IFluentServiceRegistrator WhenHas(Type attributeType)
        {
            var store = (ArrayStore<Type>)this.Context.AttributeConditions;
            this.Context.AttributeConditions = store.Add(attributeType);
            return this;
        }

        public IFluentServiceRegistrator When(Func<TypeInformation, bool> resolutionCondition)
        {
            this.Context.ResolutionCondition = resolutionCondition;
            return this;
        }

        public IFluentServiceRegistrator WithFactory(Func<IDependencyResolver, object> containerFactory)
        {
            this.Context.ContainerFactory = containerFactory;
            return this;
        }
        public IFluentServiceRegistrator WithFactory(Func<object> singleFactory)
        {
            this.Context.SingleFactory = singleFactory;
            return this;
        }

        public IFluentServiceRegistrator WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            var length = injectionParameters.Length;
            for (int i = 0; i < length; i++)
                this.AddInjectionParameter(injectionParameters[i]);
            return this;
        }

        public IFluentServiceRegistrator WithInjectionParameter(string name, object value)
        {
            this.AddInjectionParameter(new InjectionParameter { Name = name, Value = value });
            return this;
        }

        public IFluentServiceRegistrator WithLifetime(ILifetime lifetime)
        {
            this.Context.Lifetime = lifetime;
            return this;
        }

        public IFluentServiceRegistrator WithScopedLifetime()
        {
            this.Context.Lifetime = new ScopedLifetime();
            return this;
        }

        public IFluentServiceRegistrator WithSingletonLifetime()
        {
            this.Context.Lifetime = new SingletonLifetime();
            return this;
        }

        public IFluentServiceRegistrator WithName(object name)
        {
            this.Context.Name = name;
            return this;
        }

        public IFluentServiceRegistrator WithInstance(object instance, bool wireUp = false)
        {
            this.Context.ExistingInstance = instance;
            this.Context.IsWireUp = wireUp;
            return this;
        }

        public IFluentServiceRegistrator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
        {
            this.Context.AutoMemberInjectionEnabled = true;
            this.Context.AutoMemberInjectionRule = rule;
            return this;
        }

        public IFluentServiceRegistrator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule)
        {
            this.Context.ConstructorSelectionRule = rule;
            return this;
        }

        public IFluentServiceRegistrator WithoutDisposalTracking()
        {
            this.Context.IsLifetimeExternallyOwned = true;
            return this;
        }

        public IFluentServiceRegistrator ReplaceExisting()
        {
            this.ReplaceExistingRegistration = true;
            return this;
        }

        public IFluentServiceRegistrator WithConstructorByArgumentTypes(params Type[] argumentTypes)
        {
            var constructor = this.ImplementationType.GetConstructorByTypes(argumentTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argumentTypes);

            this.Context.SelectedConstructor = constructor;
            return this;
        }

        public IFluentServiceRegistrator WithConstructorByArguments(params object[] arguments)
        {
            var argTypes = arguments.Select(arg => arg.GetType()).ToArray();
            var constructor = this.ImplementationType.GetConstructorByTypes(argTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argTypes);

            this.Context.SelectedConstructor = constructor;
            this.Context.ConstructorArguments = arguments;
            return this;
        }

        public IFluentServiceRegistrator AsImplementedTypes()
        {
            this.AdditionalServiceTypes = new ArrayStore<Type>(this.ImplementationType.GetRegisterableInterfaceTypes()
                    .Concat(this.ImplementationType.GetRegisterableBaseTypes()).CastToArray());
            return this;
        }

        public IFluentServiceRegistrator InNamedScope(object scopeName) =>
            this.WithLifetime(new NamedScopeLifetime(scopeName));

        public IFluentServiceRegistrator DefinesScope(object scopeName)
        {
            this.Context.DefinedScopeName = scopeName;
            return this;
        }

        public IFluentServiceRegistrator WithPerResolutionRequestLifetime() =>
            this.WithLifetime(new ResolutionRequestLifetime());

        public IFluentServiceRegistrator InjectMember(string memberName, object dependencyName = null)
        {
            this.Context.InjectionMemberNames.Add(memberName, dependencyName);
            return this;
        }

        public IFluentServiceRegistrator AsServiceAlso(Type serviceType)
        {
            if (!this.ImplementationType.Implements(serviceType))
                throw new ArgumentException("The given service type is not assignable from the current implementation type.");

            this.AdditionalServiceTypes = this.AdditionalServiceTypes.Add(serviceType);
            return this;
        }

        private void ThrowConstructorNotFoundException(Type type, params Type[] argTypes)
        {
            if (argTypes.Length == 0)
                throw new ConstructorNotFoundException(type);

            if (argTypes.Length == 1)
                throw new ConstructorNotFoundException(type, argTypes[0]);

            throw new ConstructorNotFoundException(type, argTypes);
        }

        private void AddInjectionParameter(InjectionParameter injectionParameter)
        {
            var store = (ArrayStore<InjectionParameter>)this.Context.InjectionParameters;
            this.Context.InjectionParameters = store.Add(injectionParameter);
        }
    }
}
