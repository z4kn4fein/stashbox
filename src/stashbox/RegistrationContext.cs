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
        private readonly IBuilderContext builderContext;
        private readonly IContainerExtensionManager containerExtensionManager;

        private string name;
        private Func<object> singleFactory;
        private Func<object, object> oneParameterFactory;
        private Func<object, object, object> twoParametersFactory;
        private Func<object, object, object, object> threeParametersFactory;
        private HashSet<InjectionParameter> injectionParameters;
        private ILifetime lifetime;

        public RegistrationContext(Type typeFrom, Type typeTo, IBuilderContext builderContext, IContainerExtensionManager containerExtensionManager)
        {
            Shield.EnsureNotNull(typeTo);
            Shield.EnsureNotNull(builderContext);
            Shield.EnsureNotNull(containerExtensionManager);

            this.typeFrom = typeFrom ?? typeTo;
            this.typeTo = typeTo;
            this.builderContext = builderContext;
            this.containerExtensionManager = containerExtensionManager;
        }

        public IStashboxContainer Register()
        {
            var name = NameGenerator.GetRegistrationName(this.name);

            var registrationLifetime = lifetime ?? new TransientLifetime();

            var registrationInfo = new RegistrationInfo { TypeFrom = typeFrom, TypeTo = typeTo };

            var registration = new ServiceRegistration(registrationLifetime,
                this.CreateObjectBuilder(), this.builderContext, registrationInfo);

            this.builderContext.RegistrationRepository.AddRegistration(typeFrom, registration, name);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.builderContext, registrationInfo);
            this.builderContext.MessagePublisher.Broadcast(new RegistrationAdded { RegistrationInfo = registrationInfo });
            return this.builderContext.Container;
        }

        public IRegistrationContext WithFactoryParameters(Func<object, object, object, object> threeParametersFactory)
        {
            this.threeParametersFactory = threeParametersFactory;
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
            if (this.singleFactory != null)
                return new FactoryObjectBuilder(this.singleFactory, this.containerExtensionManager);

            if (this.twoParametersFactory != null)
                return new FactoryObjectBuilder(this.twoParametersFactory, this.containerExtensionManager);

            if (this.threeParametersFactory != null)
                return new FactoryObjectBuilder(this.threeParametersFactory, this.containerExtensionManager);

            if (this.oneParameterFactory != null)
                return new FactoryObjectBuilder(this.oneParameterFactory, this.containerExtensionManager);

            if (this.typeTo.GetTypeInfo().IsGenericTypeDefinition)
                return new GenericTypeObjectBuilder(new MetaInfoProvider(this.builderContext, typeTo));

            return new DefaultObjectBuilder(new MetaInfoProvider(this.builderContext, typeTo),
                this.containerExtensionManager, this.builderContext.MessagePublisher, injectionParameters);
        }
    }
}
