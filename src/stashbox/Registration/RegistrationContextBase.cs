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

            var registrationLifetime = this.ChooseLifeTime();

            var registrationInfo = new RegistrationInfo { TypeFrom = this.TypeFrom, TypeTo = this.TypeTo };

            var cache = new MetaInfoCache(this.ContainerContext.ContainerConfiguration, this.TypeTo);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, cache);

            this.CompleteRegistration(containerExtensionManager, update, metaInfoProvider, registrationName, registrationLifetime);

            this.CompleteScopeManagement(update, registrationLifetime);

            return registrationInfo;
        }

        private void CompleteRegistration(IContainerExtensionManager containerExtensionManager, bool update,
            MetaInfoProvider metaInfoProvider, string registrationName, ILifetime registrationLifetime)
        {
            if (this.TypeTo.GetTypeInfo().IsGenericTypeDefinition)
            {
                var objectBuilder = new GenericTypeObjectBuilder(this.RegistrationContextData, this.ContainerContext,
                    metaInfoProvider, containerExtensionManager);

                var registration = this.CreateServiceRegistration(registrationName, new TransientLifetime(), objectBuilder,
                    metaInfoProvider);
                if (update)
                    this.ContainerContext.RegistrationRepository.AddOrUpdateGenericDefinition(this.TypeFrom, registration,
                        registrationName);
                else
                    this.ContainerContext.RegistrationRepository.AddGenericDefinition(this.TypeFrom, registration,
                        registrationName);
            }
            else
            {
                var registration = this.CreateServiceRegistration(registrationName, registrationLifetime,
                    this.CreateObjectBuilder(containerExtensionManager, metaInfoProvider), metaInfoProvider);
                if (update)
                    this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(this.TypeFrom, registration,
                        registrationName);
                else
                    this.ContainerContext.RegistrationRepository.AddRegistration(this.TypeFrom, registration, registrationName);
            }
        }

        private void CompleteScopeManagement(bool update, ILifetime registrationLifetime)
        {
            if (!this.RegistrationContextData.ScopeManagementEnabled &&
                (!this.ContainerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                 !registrationLifetime.IsTransient)) return;

            var registrationItem = new ScopedRegistrationItem(this.TypeFrom, this.TypeTo, this.RegistrationContextData);

            if (update)
                this.ContainerContext.ScopedRegistrations.AddOrUpdate(this.RegistrationContextData.Name, registrationItem,
                    (oldValue, newValue) => newValue);
            else
                this.ContainerContext.ScopedRegistrations.AddOrUpdate(this.RegistrationContextData.Name, registrationItem);
        }

        private IObjectBuilder CreateObjectBuilder(IContainerExtensionManager containerExtensionManager, MetaInfoProvider metaInfoProvider)
        {
            if (this.RegistrationContextData.ExistingInstance != null)
                return new InstanceObjectBuilder(this.RegistrationContextData.ExistingInstance);

            var objectExtender = new ObjectExtender(metaInfoProvider, this.RegistrationContextData.InjectionParameters);

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

            return new DefaultObjectBuilder(this.ContainerContext, metaInfoProvider,
                containerExtensionManager, this.RegistrationContextData.InjectionParameters);
        }

        private IServiceRegistration CreateServiceRegistration(string name, ILifetime lifeTime, IObjectBuilder objectBuilder, MetaInfoProvider metaInfoProvider)
        {
            return new ServiceRegistration(name, this.ContainerContext, lifeTime, objectBuilder, metaInfoProvider, this.RegistrationContextData.AttributeConditions,
                this.RegistrationContextData.TargetTypeCondition, this.RegistrationContextData.ResolutionCondition);
        }

        private ILifetime ChooseLifeTime() => this.RegistrationContextData.ExistingInstance != null ? new TransientLifetime() :
                                            RegistrationContextData.ScopeManagementEnabled ? new SingletonLifetime() :
                                                this.RegistrationContextData.Lifetime ?? this.GetTransientLifeTime();

        private ILifetime GetTransientLifeTime() => this.ContainerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled
                ? (ILifetime) new ContainerTrackedTransientLifetime()
                : new TransientLifetime();
    }
}
