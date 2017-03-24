using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Registration;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : ObjectBuilderBase
    {
        private readonly RegistrationContextData registrationContextData;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IContainerContext containerContext;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly bool isDecorator;

        public GenericTypeObjectBuilder(RegistrationContextData registrationContextData, IContainerContext containerContext,
            IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager, 
            IExpressionBuilder expressionBuilder, bool isDecorator = false)
            : base(containerContext, isDecorator)
        {
            this.registrationContextData = registrationContextData;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.expressionBuilder = expressionBuilder;
            this.isDecorator = isDecorator;
        }

        protected override Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType)
        {
            var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolveType.GetGenericArguments());
            var registration = this.RegisterConcreteGenericType(resolveType, genericType);
            return registration.GetExpression(resolutionInfo, resolveType);
        }

        private IServiceRegistration RegisterConcreteGenericType(Type resolveType, Type genericType)
        {
            var registrationContext = new RegistrationContext(resolveType, genericType, this.containerContext, this.expressionBuilder, this.containerExtensionManager);
            var newData = this.registrationContextData.CreateCopy();
            newData.Name = null;

            if (!this.isDecorator) return registrationContext.InitWithExistingData(newData);

            var registration = registrationContext.CreateServiceRegistration(newData, this.isDecorator);
            this.containerContext.DecoratorRepository.AddDecorator(resolveType, registration);
            return registration;
        }
    }
}
