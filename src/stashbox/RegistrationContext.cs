using Ronin.Common;
using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Entity.Events;
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
        private readonly Type typeFrom;
        private readonly Type typeTo;
        private readonly IContainerContext containerContext;
        private readonly IContainerExtensionManager containerExtensionManager;

        private string name;
        private Func<object> singleFactory;
        private Func<object, object> oneParameterFactory;
        private Func<object, object, object> twoParametersFactory;
        private Func<object, object, object, object> threeParametersFactory;
        private HashSet<InjectionParameter> injectionParameters;
        private ILifetime lifetime;
        private Type targetTypeCondition;
        private Func<TypeInformation, bool> resolutionCondition;
        private HashSet<Type> attributeConditions;

        public RegistrationContext(Type typeFrom, Type typeTo, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager)
        {
            Shield.EnsureNotNull(typeTo);
            Shield.EnsureNotNull(containerContext);
            Shield.EnsureNotNull(containerExtensionManager);

            this.typeFrom = typeFrom ?? typeTo;
            this.typeTo = typeTo;
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.attributeConditions = new HashSet<Type>();
        }

        public IStashboxContainer Register()
        {
            var registrationName = NameGenerator.GetRegistrationName(this.name);

            var registrationLifetime = lifetime ?? new TransientLifetime();

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };

            var registration = new ServiceRegistration(registrationLifetime,
                this.CreateObjectBuilder(), this.containerContext, this.attributeConditions, this.targetTypeCondition, this.resolutionCondition);

            this.containerContext.RegistrationRepository.AddRegistration(typeFrom, registration, registrationName);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, registrationInfo, injectionParameters == null ? null : new HashSet<InjectionParameter>(injectionParameters));
            this.containerContext.MessagePublisher.Broadcast(new RegistrationAdded { RegistrationInfo = registrationInfo });
            return this.containerContext.Container;
        }

        public IRegistrationContext WithFactoryParameters(Func<object, object, object, object> threeParametersFactory)
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

        public IRegistrationContext WithFactoryParameters(Func<object, object, object> twoParametersFactory)
        {
            this.twoParametersFactory = twoParametersFactory;
            return this;
        }

        public IRegistrationContext WithFactoryParameters(Func<object, object> oneParameterFactory)
        {
            this.oneParameterFactory = oneParameterFactory;
            return this;
        }

        public IRegistrationContext WithFactoryParameters(Func<object> singleFactory)
        {
            this.singleFactory = singleFactory;
            return this;
        }

        public IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            this.injectionParameters = new HashSet<InjectionParameter>(injectionParameters);
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

        private IObjectBuilder CreateObjectBuilder()
        {
            var metainfoProvider = new MetaInfoProvider(this.containerContext, this.typeTo);
            var objectExtender = new ObjectExtender(metainfoProvider, this.containerContext.MessagePublisher, this.injectionParameters);

            if (this.singleFactory != null)
                return new FactoryObjectBuilder(this.singleFactory, this.containerExtensionManager, objectExtender);

            if (this.twoParametersFactory != null)
                return new FactoryObjectBuilder(this.twoParametersFactory, this.containerExtensionManager, objectExtender);

            if (this.threeParametersFactory != null)
                return new FactoryObjectBuilder(this.threeParametersFactory, this.containerExtensionManager, objectExtender);

            if (this.oneParameterFactory != null)
                return new FactoryObjectBuilder(this.oneParameterFactory, this.containerExtensionManager, objectExtender);

            if (this.typeTo.GetTypeInfo().IsGenericTypeDefinition)
                return new GenericTypeObjectBuilder(new MetaInfoProvider(this.containerContext, this.typeTo));

            return new DefaultObjectBuilder(new MetaInfoProvider(this.containerContext, this.typeTo),
                this.containerExtensionManager, objectExtender, this.containerContext.MessagePublisher, this.injectionParameters);
        }
    }
}
