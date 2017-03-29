using Stashbox.BuildUp;
using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using Stashbox.MetaInfo;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration
{
    internal class RegistrationContextBase
    {
        private readonly IExpressionBuilder expressionBuilder;

        public Type TypeFrom { get; }

        public Type TypeTo { get; }

        public IContainerContext ContainerContext { get; }

        public RegistrationContextData RegistrationContextData { get; protected set; }

        protected IContainerExtensionManager ContainerExtensionManager { get; }

        public RegistrationContextBase(Type typeFrom, Type typeTo, IContainerContext containerContext, IExpressionBuilder expressionBuilder,
            IContainerExtensionManager containerExtensionManager)
        {
            this.RegistrationContextData = new RegistrationContextData();
            this.ContainerExtensionManager = containerExtensionManager;
            this.TypeFrom = typeFrom ?? typeTo;
            this.TypeTo = typeTo;
            this.ContainerContext = containerContext;
            this.expressionBuilder = expressionBuilder;
        }

        public IServiceRegistration InitWithExistingData(RegistrationContextData data)
        {
            this.RegistrationContextData = data;
            return this.CompleteRegistration();
        }

        public IServiceRegistration CreateServiceRegistration(RegistrationContextData data, bool isDecorator = false)
        {
            this.RegistrationContextData = data;
            return this.CreateServiceRegistration(isDecorator);
        }

        protected IServiceRegistration CompleteRegistration(bool update = false)
        {
            var registration = this.CreateServiceRegistration();
            this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(this.TypeFrom, this.RegistrationContextData.Name, update, registration);
            return registration;
        }

        protected IServiceRegistration CreateServiceRegistration(bool isDecorator = false)
        {
            this.RegistrationContextData.Name = NameGenerator.GetRegistrationName(this.TypeFrom, this.TypeTo, this.RegistrationContextData.Name);
            var registrationLifetime = this.ChooseLifeTime();
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, this.RegistrationContextData, this.TypeTo, this.RegistrationContextData.InjectionParameters);

            var shouldHandleDisposal = this.ShouldHandleDisposal(registrationLifetime);

            var objectBuilder = this.TypeTo.IsOpenGenericType() ? new GenericTypeObjectBuilder(this.RegistrationContextData, this.ContainerContext,
                    metaInfoProvider, this.ContainerExtensionManager, this.expressionBuilder, isDecorator, shouldHandleDisposal) : 
                    this.CreateObjectBuilder(metaInfoProvider, isDecorator, shouldHandleDisposal);

            return this.ProduceServiceRegistration(registrationLifetime, objectBuilder,
                    metaInfoProvider);
        }

        private bool ShouldHandleDisposal(ILifetime registrationLifetime)
        {
            if (this.RegistrationContextData.IsLifetimeExternallyOwned)
                return false;

            if (registrationLifetime == null && this.ContainerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled)
                return true;

            return registrationLifetime != null;
        }

        private IObjectBuilder CreateObjectBuilder(IMetaInfoProvider metaInfoProvider, bool isDecorator, bool shouldHandleDisposal)
        {
            if (this.RegistrationContextData.ExistingInstance != null)
                return new InstanceObjectBuilder(this.RegistrationContextData.ExistingInstance, this.ContainerContext, isDecorator, shouldHandleDisposal);

            if (this.RegistrationContextData.ContainerFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.ContainerFactory, this.ContainerContext, metaInfoProvider,
                    this.expressionBuilder, this.RegistrationContextData.InjectionParameters, isDecorator, shouldHandleDisposal);

            if (this.RegistrationContextData.SingleFactory != null)
                return new FactoryObjectBuilder(this.RegistrationContextData.SingleFactory, this.ContainerContext, metaInfoProvider,
                    this.expressionBuilder, this.RegistrationContextData.InjectionParameters, isDecorator, shouldHandleDisposal);

            return new DefaultObjectBuilder(this.ContainerContext, metaInfoProvider,
                this.expressionBuilder, this.RegistrationContextData.InjectionParameters, isDecorator, shouldHandleDisposal);
        }

        private IServiceRegistration ProduceServiceRegistration(ILifetime lifeTime, IObjectBuilder objectBuilder, IMetaInfoProvider metaInfoProvider)
        {
            return new ServiceRegistration(this.TypeFrom, this.TypeTo, this.ContainerContext, lifeTime, objectBuilder, metaInfoProvider, this.RegistrationContextData.AttributeConditions,
                this.RegistrationContextData.TargetTypeCondition, this.RegistrationContextData.ResolutionCondition);
        }

        private ILifetime ChooseLifeTime() => this.RegistrationContextData.ExistingInstance != null ? null :
            this.RegistrationContextData.Lifetime;
    }
}
