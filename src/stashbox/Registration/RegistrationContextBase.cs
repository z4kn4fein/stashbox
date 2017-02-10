using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using Stashbox.Lifetime;
using Stashbox.MetaInfo;
using Stashbox.Utils;
using System;

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
            var originalDependencyName = this.RegistrationContextData.Name;
            var registrationName = this.RegistrationContextData.Name = NameGenerator.GetRegistrationName(this.TypeFrom, this.TypeTo, this.RegistrationContextData.Name);

            var registrationLifetime = this.ChooseLifeTime();

            var registrationInfo = new RegistrationInfo { TypeFrom = this.TypeFrom, TypeTo = this.TypeTo };

            var cache = new MetaInfoCache(this.ContainerContext.ContainerConfigurator, this.TypeTo);
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, cache);

            var objectBuilder = this.CompleteRegistration(containerExtensionManager, update, metaInfoProvider, registrationName, originalDependencyName, registrationLifetime);

            this.CompleteScopeManagement(update, registrationLifetime, objectBuilder);

            return registrationInfo;
        }

        private IObjectBuilder CompleteRegistration(IContainerExtensionManager containerExtensionManager, bool update,
            MetaInfoProvider metaInfoProvider, string registrationName, string dependencyName, ILifetime registrationLifetime)
        {
            var objectBuilder = this.TypeTo.IsOpenGenericType() ? new GenericTypeObjectBuilder(this.RegistrationContextData, this.ContainerContext,
                    metaInfoProvider, containerExtensionManager) : this.CreateObjectBuilder(containerExtensionManager, metaInfoProvider);

            var registration = this.CreateServiceRegistration(dependencyName, this.TypeTo.IsOpenGenericType() ? new TransientLifetime() : registrationLifetime, objectBuilder,
                    metaInfoProvider);

            this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(this.TypeFrom, registrationName, update, registration);

            return objectBuilder;
        }

        private void CompleteScopeManagement(bool update, ILifetime registrationLifetime, IObjectBuilder objectBuilder)
        {
            if (!registrationLifetime.IsScoped &&
                (!this.ContainerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                 !registrationLifetime.IsTransient || objectBuilder.HandlesObjectDisposal)) return;

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

            if (this.RegistrationContextData.ContainerFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.ContainerFactory, this.ContainerContext, containerExtensionManager, metaInfoProvider, this.RegistrationContextData.InjectionParameters);

            if (this.RegistrationContextData.SingleFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.SingleFactory, this.ContainerContext, containerExtensionManager, metaInfoProvider, this.RegistrationContextData.InjectionParameters);

            return new DefaultObjectBuilder(this.ContainerContext, metaInfoProvider,
                containerExtensionManager, this.RegistrationContextData.InjectionParameters);
        }

        private IServiceRegistration CreateServiceRegistration(string name, ILifetime lifeTime, IObjectBuilder objectBuilder, MetaInfoProvider metaInfoProvider)
        {
            return new ServiceRegistration(name, this.TypeFrom, this.ContainerContext, lifeTime, objectBuilder, metaInfoProvider, this.RegistrationContextData.AttributeConditions,
                this.RegistrationContextData.TargetTypeCondition, this.RegistrationContextData.ResolutionCondition);
        }

        private ILifetime ChooseLifeTime() => this.RegistrationContextData.ExistingInstance != null ? new TransientLifetime() :
                                                this.RegistrationContextData.Lifetime ?? new TransientLifetime();
    }
}
