using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.Entity.Resolution;
using Stashbox.Lifetime;

namespace Stashbox.Registration
{
    internal class RegistrationContext : IRegistrationContext, IRegistrationContextMeta
    {
        private readonly IServiceRegistrator registrator;

        public Type ServiceType { get; }

        public Type ImplementationType { get; }

        public RegistrationContextData Context { get; }

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

        public IStashboxContainer Register() => this.registrator.Register(this, false);

        public IStashboxContainer ReMap() => this.registrator.ReMap(this);

        public IRegistrationContext WhenDependantIs<TTarget>() where TTarget : class
        {
            this.Context.TargetTypeCondition = typeof(TTarget);
            return this;
        }

        public IRegistrationContext WhenDependantIs(Type targetType)
        {
            this.Context.TargetTypeCondition = targetType;
            return this;
        }

        public IRegistrationContext WhenHas<TAttribute>() where TAttribute : Attribute
        {
            this.Context.AttributeConditions.Add(typeof(TAttribute));
            return this;
        }

        public IRegistrationContext WhenHas(Type attributeType)
        {
            this.Context.AttributeConditions.Add(attributeType);
            return this;
        }

        public IRegistrationContext When(Func<TypeInformation, bool> resolutionCondition)
        {
            this.Context.ResolutionCondition = resolutionCondition;
            return this;
        }

        public IRegistrationContext WithFactory(Func<IDependencyResolver, object> containerFactory)
        {
            this.Context.ContainerFactory = containerFactory;
            return this;
        }
        public IRegistrationContext WithFactory(Func<object> singleFactory)
        {
            this.Context.SingleFactory = singleFactory;
            return this;
        }

        public IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            this.Context.InjectionParameters = injectionParameters;
            return this;
        }

        public IRegistrationContext WithLifetime(ILifetime lifetime)
        {
            this.Context.Lifetime = lifetime;
            return this;
        }

        public IRegistrationContext WithScopedLifetime()
        {
            this.Context.Lifetime = new ScopedLifetime();
            return this;
        }

        public IRegistrationContext WithSingletonLifetime()
        {
            this.Context.Lifetime = new SingletonLifetime();
            return this;
        }

        public IRegistrationContext WithName(string name)
        {
            this.Context.Name = name;
            return this;
        }

        public IRegistrationContext WithInstance(object instance)
        {
            this.Context.ExistingInstance = instance;
            return this;
        }

        public IRegistrationContext WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter)
        {
            this.Context.AutoMemberInjectionEnabled = true;
            this.Context.AutoMemberInjectionRule = rule;
            return this;
        }

        public IRegistrationContext WithConstructorSelectionRule(Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> rule)
        {
            this.Context.ConstructorSelectionRule = rule;
            return this;
        }

        public IRegistrationContext WithoutDisposalTracking()
        {
            this.Context.IsLifetimeExternallyOwned = true;
            return this;
        }

        public IServiceRegistration CreateServiceRegistration(bool isDecorator) =>
            this.registrator.CreateServiceRegistration(this, isDecorator);
    }
}
