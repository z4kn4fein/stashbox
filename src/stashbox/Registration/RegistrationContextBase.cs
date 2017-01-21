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
            var registrationName = this.RegistrationContextData.Name = NameGenerator.GetRegistrationName(this.TypeTo, this.RegistrationContextData.Name);

            var registrationLifetime = this.RegistrationContextData.ExistingInstance != null ? new TransientLifetime() :
                                            RegistrationContextData.ScopeManagementEnabled ? new SingletonLifetime() :
                                                this.RegistrationContextData.Lifetime ?? this.GetTransientLifeTime();

            var registrationInfo = new RegistrationInfo { TypeFrom = this.TypeFrom, TypeTo = this.TypeTo };

            if (this.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.RegistrationContextData, this.ContainerContext,
                    new MetaInfoProvider(this.ContainerContext, new MetaInfoCache(this.TypeTo)), containerExtensionManager);

                var registration = this.CreateServiceRegistration(new TransientLifetime(), objectBuilder);
                if (update)
                    this.ContainerContext.RegistrationRepository.AddOrUpdateGenericDefinition(this.TypeFrom, registration, registrationName);
                else
                    this.ContainerContext.RegistrationRepository.AddGenericDefinition(this.TypeFrom, registration, registrationName);
            }
            else
            {
                var registration = this.CreateServiceRegistration(registrationLifetime, this.CreateObjectBuilder(registrationName, containerExtensionManager));
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
                this.ContainerContext.ScopedRegistrations.AddOrUpdate(this.RegistrationContextData.Name, () => registrationItem, (o, n) => registrationItem);
            else
                this.ContainerContext.ScopedRegistrations.Add(this.RegistrationContextData.Name, registrationItem, false);

            return registrationInfo;
        }

        private IObjectBuilder CreateObjectBuilder(string name, IContainerExtensionManager containerExtensionManager)
        {
            if (this.RegistrationContextData.ExistingInstance != null)
                return new InstanceObjectBuilder(this.RegistrationContextData.ExistingInstance);

            var metainfoProvider = new MetaInfoProvider(this.ContainerContext, new MetaInfoCache(this.TypeTo));

            var objectExtender = new ObjectExtender(metainfoProvider, this.RegistrationContextData.InjectionParameters);

            if (this.RegistrationContextData.ContainerFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.ContainerFactory, this.ContainerContext, containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.SingleFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.SingleFactory, this.ContainerContext, containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.TwoParametersFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.TwoParametersFactory, this.ContainerContext, containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.ThreeParametersFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.ThreeParametersFactory, this.ContainerContext, containerExtensionManager, objectExtender);

            if (this.RegistrationContextData.OneParameterFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.OneParameterFactory, this.ContainerContext, containerExtensionManager, objectExtender);

            return new DefaultObjectBuilder(this.ContainerContext, new MetaInfoProvider(this.ContainerContext, new MetaInfoCache(this.TypeTo)),
                containerExtensionManager, this.RegistrationContextData.InjectionParameters);
        }

        private IServiceRegistration CreateServiceRegistration(ILifetime lifeTime, IObjectBuilder objectBuilder)
        {
            return new ServiceRegistration(this.ContainerContext, lifeTime, objectBuilder, this.RegistrationContextData.AttributeConditions,
                this.RegistrationContextData.TargetTypeCondition, this.RegistrationContextData.ResolutionCondition);
        }

        private ILifetime GetTransientLifeTime()
        {
            if (this.ContainerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled) return new ContainerTrackedTransientLifetime(); else return new TransientLifetime();
        }
    }
}
