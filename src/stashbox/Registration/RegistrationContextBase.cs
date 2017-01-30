using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Utils;
using System;
using System.Reflection;

namespace Stashbox.Registration
{
    internal class RegistrationContextBase
    {
        protected Type TypeFrom { get; }
        protected Type TypeTo { get; }

        public IContainerContext ContainerContext { get; }
        public RegistrationContextData RegistrationContextData { get; protected set; }

        public RegistrationContextBase(Type typeFrom, Type typeTo, IContainerContext containerContext)
        {
            this.RegistrationContextData = new RegistrationContextData();
            this.TypeFrom = typeFrom ?? typeTo;
            this.TypeTo = typeTo;
            this.ContainerContext = containerContext;
        }

        protected RegistrationInfo PrepareRegistration(IContainerExtensionManager containerExtensionManager, bool update = false)
        {
            var registrationName = this.RegistrationContextData.Name = NameGenerator.GetRegistrationName(this.TypeFrom, this.TypeTo, this.RegistrationContextData.Name);

            var registrationLifetime = this.RegistrationContextData.ExistingInstance != null ? new TransientLifetime() :
                                            RegistrationContextData.ScopeManagementEnabled ? new SingletonLifetime() :
                                                this.RegistrationContextData.Lifetime ?? this.GetTransientLifeTime();

            var registrationInfo = new RegistrationInfo { TypeFrom = this.TypeFrom, TypeTo = this.TypeTo };

            var cache = new MetaInfoCache(this.ContainerContext.ContainerConfiguration, this.TypeTo);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, cache);

            if (this.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.RegistrationContextData, this.ContainerContext,
                   metaInfoProvider, containerExtensionManager);

                var registration = this.CreateServiceRegistration(registrationName, new TransientLifetime(), objectBuilder, metaInfoProvider);
                if (update)
                    this.ContainerContext.RegistrationRepository.AddOrUpdateGenericDefinition(this.TypeFrom, registration, registrationName);
                else
                    this.ContainerContext.RegistrationRepository.AddGenericDefinition(this.TypeFrom, registration, registrationName);
            }
            else
            {
                var registration = this.CreateServiceRegistration(registrationName, registrationLifetime, this.CreateObjectBuilder(containerExtensionManager, metaInfoProvider), metaInfoProvider);
                if (update)
                    this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(this.TypeFrom, registration, registrationName);
                else
                    this.ContainerContext.RegistrationRepository.AddRegistration(this.TypeFrom, registration, registrationName);
            }

            if (!this.RegistrationContextData.ScopeManagementEnabled &&
                (!this.ContainerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled || !registrationLifetime.IsTransient))
                return registrationInfo;

            var registrationItem = new ScopedRegistrationItem(this.TypeFrom, this.TypeTo, this.RegistrationContextData);

            if (update)
                this.ContainerContext.ScopedRegistrations.AddOrUpdate(this.RegistrationContextData.Name, registrationItem, (oldValue, newValue) => newValue);
            else
                this.ContainerContext.ScopedRegistrations.AddOrUpdate(this.RegistrationContextData.Name, registrationItem);

            return registrationInfo;
        }

        private IObjectBuilder CreateObjectBuilder(IContainerExtensionManager containerExtensionManager, MetaInfoProvider metaInfoProvider)
        {
            if (this.RegistrationContextData.ExistingInstance != null)
                return new InstanceObjectBuilder(this.RegistrationContextData.ExistingInstance);

            var objectExtender = new ObjectExtender(metaInfoProvider, this.RegistrationContextData.InjectionParameters);

            if (this.RegistrationContextData.ContainerFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.ContainerFactory, containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.SingleFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.SingleFactory, containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.TwoParametersFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.TwoParametersFactory, containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.ThreeParametersFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.ThreeParametersFactory,containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.OneParameterFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.OneParameterFactory, containerExtensionManager, objectExtender);

            return new DefaultObjectBuilder(this.ContainerContext, metaInfoProvider,
                containerExtensionManager, this.RegistrationContextData.InjectionParameters);
        }

        private IServiceRegistration CreateServiceRegistration(string name, ILifetime lifeTime, IObjectBuilder objectBuilder, MetaInfoProvider metaInfoProvider)
        {
            return new ServiceRegistration(name, this.ContainerContext, lifeTime, objectBuilder, metaInfoProvider, this.RegistrationContextData.AttributeConditions,
                this.RegistrationContextData.TargetTypeCondition, this.RegistrationContextData.ResolutionCondition);
        }

        private ILifetime GetTransientLifeTime()
        {
            if (this.ContainerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled) return new ContainerTrackedTransientLifetime(); else return new TransientLifetime();
        }
    }
}
