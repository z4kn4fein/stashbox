using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Lifetime;

namespace Stashbox.Registration
{
    internal class RegistrationContext<TService> : RegistrationContext, IRegistrationContext<TService>
    {
        public RegistrationContext(Type serviceType, Type implementationType, IServiceRegistrator registrator)
            : base(serviceType, implementationType, registrator)
        { }

        public IFluentServiceRegistrator<TService> WithFinalizer(Action<TService> finalizer)
        {
            base.Context.Finalizer = finalizer;
            return this;
        }

        public IFluentServiceRegistrator<TService> WithInitializer(Action<TService> initializer)
        {
            base.Context.Initializer = initializer;
            return this;
        }
    }

    internal class RegistrationContext : IRegistrationContext, IRegistrationContextMeta
    {
        private readonly IServiceRegistrator registrator;

        public Type ServiceType { get; }

        public Type ImplementationType { get; }

        public RegistrationContextData Context { get; }

        private bool replaceExistingRegistration;

        private bool withImplementedTypes;

        public RegistrationContext(Type serviceType, Type implementationType, IServiceRegistrator registrator)
            : this(serviceType, implementationType, registrator, RegistrationContextData.New())
        { }

        public RegistrationContext(Type serviceType, Type implementationType, IServiceRegistrator registrator, RegistrationContextData registrationContextData)
        {
            this.registrator = registrator;
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Context = registrationContextData;
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
            this.Context.AttributeConditions.Add(typeof(TAttribute));
            return this;
        }

        public IFluentServiceRegistrator WhenHas(Type attributeType)
        {
            this.Context.AttributeConditions.Add(attributeType);
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
            this.Context.InjectionParameters = injectionParameters;
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

        public IFluentServiceRegistrator WithInstance(object instance)
        {
            this.Context.ExistingInstance = instance;
            return this;
        }

        public IFluentServiceRegistrator WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter)
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

        public IServiceRegistration CreateServiceRegistration(bool isDecorator) =>
            this.registrator.CreateServiceRegistration(this, isDecorator);

        public IFluentServiceRegistrator ReplaceExisting()
        {
            this.replaceExistingRegistration = true;
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
            this.withImplementedTypes = true;
            return this;
        }

        public IStashboxContainer Register()
        {
            if (this.withImplementedTypes)
            {
                var interfaceTypes = this.ImplementationType.GetRegisterableInterfaceTypes();
                foreach (var interfaceType in interfaceTypes)
                {
                    var context = new RegistrationContext(interfaceType, this.ImplementationType, this.registrator, this.Context.CreateCopy());
                    this.registrator.Register(context, false, this.replaceExistingRegistration);
                }

                var baseTypes = this.ImplementationType.GetRegisterableBaseTypes();
                foreach (var baseType in baseTypes)
                {
                    var context = new RegistrationContext(baseType, this.ImplementationType, this.registrator, this.Context.CreateCopy());
                    this.registrator.Register(context, false, this.replaceExistingRegistration);
                }
            }

            return this.registrator.Register(this, false, this.replaceExistingRegistration);
        }

        public IStashboxContainer ReMap()
        {
            if (this.withImplementedTypes)
            {
                var interfaceTypes = this.ImplementationType.GetRegisterableInterfaceTypes();
                foreach (var interfaceType in interfaceTypes)
                {
                    var context = new RegistrationContext(interfaceType, this.ImplementationType, this.registrator, this.Context.CreateCopy());
                    this.registrator.ReMap(context, false);
                }

                var baseTypes = this.ImplementationType.GetRegisterableBaseTypes();
                foreach (var baseType in baseTypes)
                {
                    var context = new RegistrationContext(baseType, this.ImplementationType, this.registrator, this.Context.CreateCopy());
                    this.registrator.ReMap(context, false);
                }
            }

            return this.registrator.ReMap(this, false);
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
