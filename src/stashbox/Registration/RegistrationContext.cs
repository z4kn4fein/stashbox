using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Collections.Generic;
using System.Reflection;
using Stashbox.Configuration;
using Stashbox.Lifetime;

namespace Stashbox.Registration
{
    internal class RegistrationContext : IRegistrationContext, IRegistrationContextMeta
    {
        private readonly IServiceRegistrator registrator;

        public Type ServiceType { get; }

        public Type ImplementationType { get; }

        public RegistrationContextData Context { get; }

        private bool replaceExistingRegistration;

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

        public IStashboxContainer Register() => this.registrator.Register(this, false, this.replaceExistingRegistration);

        public IStashboxContainer ReMap() => this.registrator.ReMap(this, false);

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

        public IFluentServiceRegistrator WithName(string name)
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

        public IFluentServiceRegistrator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> rule)
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
    }
}
