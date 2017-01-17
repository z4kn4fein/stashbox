using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Registration;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : IObjectBuilder
    {
        private readonly RegistrationContextData registrationContextData;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IContainerContext containerContext;
        private readonly object syncObject = new object();

        public GenericTypeObjectBuilder(RegistrationContextData registrationContextData, IContainerContext containerContext,
            IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager)
        {
            this.registrationContextData = registrationContextData;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            IServiceRegistration registration;
            if (this.containerContext.RegistrationRepository
                .TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(resolveType, out registration))
                return registration.GetInstance(resolutionInfo, resolveType);

            lock (this.syncObject)
            {
                if (this.containerContext.RegistrationRepository
                    .TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(resolveType, out registration))
                    return registration.GetInstance(resolutionInfo, resolveType);

                this.RegisterConcreteGenericType(resolveType);

                this.containerContext.RegistrationRepository
                    .TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(resolveType, out registration);

                return registration.GetInstance(resolutionInfo, resolveType);
            }
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            IServiceRegistration registration;
            if (this.containerContext.RegistrationRepository
                .TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(resolveType, out registration))
                return registration.GetExpression(resolutionInfo, resolutionInfoExpression, resolveType);

            lock (this.syncObject)
            {
                if (this.containerContext.RegistrationRepository
                    .TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(resolveType, out registration))
                    return registration.GetExpression(resolutionInfo, resolutionInfoExpression, resolveType);

                this.RegisterConcreteGenericType(resolveType);

                this.containerContext.RegistrationRepository
                    .TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(resolveType, out registration);

                return registration.GetExpression(resolutionInfo, resolutionInfoExpression, resolveType);
            }
        }

        private void RegisterConcreteGenericType(TypeInformation resolveType)
        {
            var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolveType.Type.GenericTypeArguments);
            var registrationContext = new ScopedRegistrationContext(resolveType.Type, genericType, this.containerContext, this.containerExtensionManager);
            var newData = this.registrationContextData.CreateCopy();
            newData.Name = null;
            registrationContext.InitFromScope(newData);
        }

        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
        }

        public void CleanUp()
        {
        }
    }
}
