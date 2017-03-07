using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class GenericTypeObjectBuilder : IObjectBuilder
    {
        private readonly RegistrationContextData registrationContextData;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IContainerContext containerContext;
        private readonly IExpressionBuilder expressionBuilder;

        public GenericTypeObjectBuilder(RegistrationContextData registrationContextData, IContainerContext containerContext,
            IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager, IExpressionBuilder expressionBuilder)
        {
            this.registrationContextData = registrationContextData;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.expressionBuilder = expressionBuilder;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolveType.Type.GetGenericArguments());
            resolveType.DependencyName = NameGenerator.GetRegistrationName(resolveType.Type, genericType);

            this.RegisterConcreteGenericType(resolveType, genericType);

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(resolveType);
            if(registration == null)
                throw new ResolutionFailedException(genericType.FullName);

            return registration.GetExpression(resolutionInfo, resolveType);
        }

        private void RegisterConcreteGenericType(TypeInformation resolveType, Type genericType)
        {
            var registrationContext = new ScopedRegistrationContext(resolveType.Type, genericType, this.containerContext, this.expressionBuilder, this.containerExtensionManager);
            var newData = this.registrationContextData.CreateCopy();
            newData.Name = null;
            registrationContext.InitFromScope(newData);
        }

        public bool HandlesObjectDisposal => false;

        public void CleanUp()
        { }
    }
}
