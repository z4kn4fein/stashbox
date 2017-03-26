using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity.Resolution;

namespace Stashbox.Registration
{
    internal class RegistrationContext : RegistrationContextBase, IRegistrationContext
    {
        public RegistrationContext(Type typeFrom, Type typeTo, IContainerContext containerContext,
            IExpressionBuilder expressionBuilder, IContainerExtensionManager containerExtensionManager)
            : base(typeFrom, typeTo, containerContext, expressionBuilder, containerExtensionManager)
        { }

        public IStashboxContainer Register()
        {
            base.CompleteRegistration();
            base.ContainerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, base.TypeTo, base.TypeFrom, base.RegistrationContextData.InjectionParameters);
            return this.ContainerContext.Container;
        }

        public IStashboxContainer ReMap()
        {
            base.CompleteRegistration(true);
            base.ContainerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, base.TypeTo, base.TypeFrom, base.RegistrationContextData.InjectionParameters);

            this.ContainerContext.DelegateRepository.InvalidateDelegateCache();
            return this.ContainerContext.Container;
        }

        public IRegistrationContext WhenDependantIs<TTarget>(string dependencyName = null) where TTarget : class
        {
            base.RegistrationContextData.TargetTypeCondition = typeof(TTarget);
            return this;
        }

        public IRegistrationContext WhenDependantIs(Type targetType, string dependencyName = null)
        {
            base.RegistrationContextData.TargetTypeCondition = targetType;
            return this;
        }

        public IRegistrationContext WhenHas<TAttribute>() where TAttribute : Attribute
        {
            base.RegistrationContextData.AttributeConditions.Add(typeof(TAttribute));
            return this;
        }

        public IRegistrationContext WhenHas(Type attributeType)
        {
            base.RegistrationContextData.AttributeConditions.Add(attributeType);
            return this;
        }

        public IRegistrationContext When(Func<TypeInformation, bool> resolutionCondition)
        {
            base.RegistrationContextData.ResolutionCondition = resolutionCondition;
            return this;
        }

        public IRegistrationContext WithFactory(Func<IStashboxContainer, object> containerFactory)
        {
            base.RegistrationContextData.ContainerFactory = containerFactory;
            return this;
        }
        public IRegistrationContext WithFactory(Func<object> singleFactory)
        {
            base.RegistrationContextData.SingleFactory = singleFactory;
            return this;
        }

        public IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            base.RegistrationContextData.InjectionParameters = injectionParameters;
            return this;
        }

        public IRegistrationContext WithLifetime(ILifetime lifetime)
        {
            base.RegistrationContextData.Lifetime = lifetime;
            return this;
        }

        public IRegistrationContext WithName(string name)
        {
            base.RegistrationContextData.Name = name;
            return this;
        }

        public IRegistrationContext WithInstance(object instance)
        {
            base.RegistrationContextData.ExistingInstance = instance;
            return this;
        }

        public IRegistrationContext WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter)
        {
            base.RegistrationContextData.AutoMemberInjectionEnabled = true;
            base.RegistrationContextData.AutoMemberInjectionRule = rule;
            return this;
        }

        public IRegistrationContext WithConstructorSelectionRule(Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> rule)
        {
            base.RegistrationContextData.ConstructorSelectionRule = rule;
            return this;
        }
        
        public IRegistrationContext WithoutDisposalTracking()
        {
            this.RegistrationContextData.IsLifetimeExternallyOwned = true;
            return this;
        }

        public new IServiceRegistration CreateServiceRegistration(bool isDecorator = false) => base.CreateServiceRegistration(isDecorator);
    }
}
