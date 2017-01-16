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
        public IContainerContext ContainerContext { get; }
        public RegistrationContextData RegistrationContextData { get; protected set; }

        public RegistrationContextBase(Type typeFrom, Type typeTo, IContainerContext containerContext)
        {
            this.RegistrationContextData = new RegistrationContextData()
            {
                TypeFrom = typeFrom ?? typeTo,
                TypeTo = typeTo
            };

            this.ContainerContext = containerContext;
        }

        protected RegistrationInfo PrepareRegistration(IContainerExtensionManager containerExtensionManager, bool update = false)
        {
            var registrationName = this.RegistrationContextData.Name = NameGenerator.GetRegistrationName(this.RegistrationContextData.TypeTo, this.RegistrationContextData.Name);

            var registrationLifetime = RegistrationContextData.ScopeManagementEnabled ?
                new SingletonLifetime() :
                this.RegistrationContextData.Lifetime ?? this.GetTransientLifeTime();

            var registrationInfo = new RegistrationInfo { TypeFrom = this.RegistrationContextData.TypeFrom, TypeTo = this.RegistrationContextData.TypeTo };

            if (this.RegistrationContextData.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.ContainerContext,
                    new MetaInfoProvider(this.ContainerContext,
                        this.ContainerContext.MetaInfoRepository.GetOrAdd(this.RegistrationContextData.TypeTo,
                            () => new MetaInfoCache(this.RegistrationContextData.TypeTo))));

                var registration = this.CreateServiceRegistration(registrationLifetime, objectBuilder);
                if (!update)
                    this.ContainerContext.RegistrationRepository.AddGenericDefinition(this.RegistrationContextData.TypeFrom, registration, registrationName);
                else
                    this.ContainerContext.RegistrationRepository.AddOrUpdateGenericDefinition(this.RegistrationContextData.TypeFrom, registration, registrationName);
            }
            else
            {
                var registration = this.CreateServiceRegistration(registrationLifetime, this.CreateObjectBuilder(registrationName, containerExtensionManager));
                if (!update)
                    this.ContainerContext.RegistrationRepository.AddRegistration(this.RegistrationContextData.TypeFrom, registration, registrationName);
                else
                    this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(this.RegistrationContextData.TypeFrom, registration, registrationName);
            }

            if (this.RegistrationContextData.ScopeManagementEnabled)
            {
                if (!update)
                    this.ContainerContext.ScopedRegistrations.Add(this.RegistrationContextData.Name, this.RegistrationContextData, false);
                else
                    this.ContainerContext.ScopedRegistrations.AddOrUpdate(this.RegistrationContextData.Name,
                        () => this.RegistrationContextData, (o, n) => this.RegistrationContextData);
            }

            return registrationInfo;
        }

        private IObjectBuilder CreateObjectBuilder(string name, IContainerExtensionManager containerExtensionManager)
        {
            var metainfoProvider = new MetaInfoProvider(this.ContainerContext, this.ContainerContext.MetaInfoRepository.
                GetOrAdd(this.RegistrationContextData.TypeTo, () => new MetaInfoCache(this.RegistrationContextData.TypeTo)));

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

            return new DefaultObjectBuilder(this.ContainerContext, new MetaInfoProvider(this.ContainerContext,
                this.ContainerContext.MetaInfoRepository.GetOrAdd(this.RegistrationContextData.TypeTo, () => new MetaInfoCache(this.RegistrationContextData.TypeTo))),
                containerExtensionManager, name, this.RegistrationContextData.InjectionParameters);
        }

        private IServiceRegistration CreateServiceRegistration(ILifetime lifeTime, IObjectBuilder objectBuilder)
        {
            return new ServiceRegistration(this.ContainerContext, lifeTime, objectBuilder, this.RegistrationContextData.AttributeConditions,
                this.RegistrationContextData.TargetTypeCondition, this.RegistrationContextData.ResolutionCondition);
        }

        private ILifetime GetTransientLifeTime()
        {
            if (this.ContainerContext.TrackTransientsForDisposal) return new ContainerTrackedTransientLifetime(); else return new TransientLifetime();
        }
    }
}
