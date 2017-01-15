using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;

namespace Stashbox.Registration
{
    internal class RegistrationContext : RegistrationContextBase, IRegistrationContext
    {
        private readonly IContainerExtensionManager containerExtensionManager;

        public RegistrationContext(Type typeFrom, Type typeTo, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager)
            : base(typeFrom, typeTo, containerContext)
        {
            this.containerExtensionManager = containerExtensionManager;
        }

        public IStashboxContainer Register()
        {
            var registrationInfo = base.PrepareRegistration(this.containerExtensionManager);

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo, base.RegistrationContextData.InjectionParameters);
            return this.ContainerContext.Container;
        }

        public IStashboxContainer ReMap()
        {
            var registrationInfo = base.PrepareRegistration(this.containerExtensionManager, true);

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo, base.RegistrationContextData.InjectionParameters);

            foreach (var serviceRegistration in this.ContainerContext.RegistrationRepository.GetAllRegistrations())
                serviceRegistration.ServiceUpdated(registrationInfo);

            return this.ContainerContext.Container;
        }

        public IRegistrationContext WithFactory(Func<object, object, object, object> threeParametersFactory)
        {
            base.RegistrationContextData.ThreeParametersFactory = threeParametersFactory;
            return this;
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

        public IRegistrationContext WithFactory(Func<object, object, object> twoParametersFactory)
        {
            base.RegistrationContextData.TwoParametersFactory = twoParametersFactory;
            return this;
        }

        public IRegistrationContext WithFactory(Func<object, object> oneParameterFactory)
        {
            base.RegistrationContextData.OneParameterFactory = oneParameterFactory;
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

        public IRegistrationContext WithScopeManagement()
        {
            base.RegistrationContextData.ScopeManagementEnabled = true;
            return this;
        }
    }
}
