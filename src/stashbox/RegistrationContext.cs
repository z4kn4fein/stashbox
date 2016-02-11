using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stashbox
{
    internal class RegistrationContext : IRegistrationContext
    {
        public Type TypeFrom { get; private set; }
        public Type TypeTo { get; private set; }
        public IContainerContext ContainerContext { get; private set; }
        private readonly IContainerExtensionManager containerExtensionManager;

        private string name;
        private Func<object> singleFactory;
        private Func<object, object> oneParameterFactory;
        private Func<object, object, object> twoParametersFactory;
        private Func<object, object, object, object> threeParametersFactory;
        private InjectionParameter[] injectionParameters;
        private ILifetime lifetime;
        private Type targetTypeCondition;
        private Func<TypeInformation, bool> resolutionCondition;
        private readonly HashSet<Type> attributeConditions;

        public RegistrationContext(Type typeFrom, Type typeTo, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager)
        {
            this.TypeFrom = typeFrom ?? typeTo;
            this.TypeTo = typeTo;
            this.ContainerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.attributeConditions = new HashSet<Type>();
        }

        public IStashboxContainer Register()
        {
            var registrationName = NameGenerator.GetRegistrationName(this.TypeTo, this.name);

            var registrationLifetime = lifetime ?? new TransientLifetime();

            var registrationInfo = new RegistrationInfo { TypeFrom = TypeFrom, TypeTo = TypeTo };

            if (this.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.ContainerContext,
                    new MetaInfoProvider(this.ContainerContext,
                        this.ContainerContext.MetaInfoRepository.GetOrAdd(this.TypeTo,
                            () => new MetaInfoCache(this.TypeTo))));

                var registration = new ServiceRegistration(registrationLifetime,
                    objectBuilder, this.attributeConditions, this.targetTypeCondition, this.resolutionCondition);

                this.ContainerContext.RegistrationRepository.AddGenericDefinition(TypeFrom, registration, registrationName);
            }
            else
            {
                var registration = new ServiceRegistration(registrationLifetime,
                    this.CreateObjectBuilder(registrationName), this.attributeConditions, this.targetTypeCondition, this.resolutionCondition);

                this.ContainerContext.RegistrationRepository.AddRegistration(TypeFrom, registration, registrationName);
            }

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo, this.injectionParameters);
            return this.ContainerContext.Container;
        }

        public IStashboxContainer ReMap()
        {
            var registrationName = NameGenerator.GetRegistrationName(this.TypeTo, this.name);

            var registrationLifetime = lifetime ?? new TransientLifetime();

            var registrationInfo = new RegistrationInfo { TypeFrom = TypeFrom, TypeTo = TypeTo };

            if (this.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.ContainerContext,
                    new MetaInfoProvider(this.ContainerContext,
                        this.ContainerContext.MetaInfoRepository.GetOrAdd(this.TypeTo,
                            () => new MetaInfoCache(this.TypeTo))));

                var registration = new ServiceRegistration(registrationLifetime,
                    objectBuilder, this.attributeConditions, this.targetTypeCondition, this.resolutionCondition);

                this.ContainerContext.RegistrationRepository.AddOrUpdateGenericDefinition(TypeFrom, registration, registrationName);
            }
            else
            {
                var registration = new ServiceRegistration(registrationLifetime,
                    this.CreateObjectBuilder(registrationName), this.attributeConditions, this.targetTypeCondition, this.resolutionCondition);

                this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(TypeFrom, registration, registrationName);
            }

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registrationInfo, this.injectionParameters);

            foreach (var serviceRegistration in this.ContainerContext.RegistrationRepository.GetAllRegistrations())
                serviceRegistration.ServiceUpdated(registrationInfo);

            return this.ContainerContext.Container;
        }

        public IRegistrationContext WithFactory(Func<object, object, object, object> threeParametersFactory)
        {
            this.threeParametersFactory = threeParametersFactory;
            return this;
        }

        public IRegistrationContext WhenDependantIs<TTarget>(string dependencyName = null) where TTarget : class
        {
            this.targetTypeCondition = typeof(TTarget);
            return this;
        }

        public IRegistrationContext WhenDependantIs(Type targetType, string dependencyName = null)
        {
            this.targetTypeCondition = targetType;
            return this;
        }

        public IRegistrationContext WhenHas<TAttribute>() where TAttribute : Attribute
        {
            this.attributeConditions.Add(typeof(TAttribute));
            return this;
        }

        public IRegistrationContext WhenHas(Type attributeType)
        {
            this.attributeConditions.Add(attributeType);
            return this;
        }

        public IRegistrationContext When(Func<TypeInformation, bool> resolutionCondition)
        {
            this.resolutionCondition = resolutionCondition;
            return this;
        }

        public IRegistrationContext WithFactory(Func<object, object, object> twoParametersFactory)
        {
            this.twoParametersFactory = twoParametersFactory;
            return this;
        }

        public IRegistrationContext WithFactory(Func<object, object> oneParameterFactory)
        {
            this.oneParameterFactory = oneParameterFactory;
            return this;
        }

        public IRegistrationContext WithFactory(Func<object> singleFactory)
        {
            this.singleFactory = singleFactory;
            return this;
        }

        public IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            this.injectionParameters = injectionParameters;
            return this;
        }

        public IRegistrationContext WithLifetime(ILifetime lifetime)
        {
            this.lifetime = lifetime;
            return this;
        }

        public IRegistrationContext WithName(string name)
        {
            this.name = name;
            return this;
        }

        private IObjectBuilder CreateObjectBuilder(string name)
        {
            var metainfoProvider = new MetaInfoProvider(this.ContainerContext, this.ContainerContext.MetaInfoRepository.GetOrAdd(this.TypeTo, () => new MetaInfoCache(this.TypeTo)));
            var objectExtender = new ObjectExtender(metainfoProvider, this.injectionParameters);

            if (this.singleFactory != null)
                return new FactoryObjectBuilder(this.singleFactory, this.ContainerContext, this.containerExtensionManager, objectExtender);

            if (this.twoParametersFactory != null)
                return new FactoryObjectBuilder(this.twoParametersFactory, this.ContainerContext, this.containerExtensionManager, objectExtender);

            if (this.threeParametersFactory != null)
                return new FactoryObjectBuilder(this.threeParametersFactory, this.ContainerContext, this.containerExtensionManager, objectExtender);

            if (this.oneParameterFactory != null)
                return new FactoryObjectBuilder(this.oneParameterFactory, this.ContainerContext, this.containerExtensionManager, objectExtender);

            return new DefaultObjectBuilder(this.ContainerContext, new MetaInfoProvider(this.ContainerContext, this.ContainerContext.MetaInfoRepository.GetOrAdd(this.TypeTo, () => new MetaInfoCache(this.TypeTo))),
                this.containerExtensionManager, name, this.injectionParameters);
        }
    }
}
