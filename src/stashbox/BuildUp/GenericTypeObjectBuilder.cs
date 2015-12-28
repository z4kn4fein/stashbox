using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : IObjectBuilder
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IContainerContext containerContext;
        private readonly object syncObject = new object();

        public GenericTypeObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider)
        {
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
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

                var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolveType.Type.GenericTypeArguments);
                this.containerContext.Container.RegisterType(resolveType.Type, genericType);

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

                var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolveType.Type.GenericTypeArguments);
                this.containerContext.Container.RegisterType(resolveType.Type, genericType);

                this.containerContext.RegistrationRepository
                    .TryGetRegistrationWithConditionsWithoutGenericDefinitionExtraction(resolveType, out registration);

                return registration.GetExpression(resolutionInfo, resolutionInfoExpression, resolveType);
            }
        }

        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
        }

        public void CleanUp()
        {
        }
    }
}
