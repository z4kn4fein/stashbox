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
    internal class GenericTypeObjectBuilder : ObjectBuilderBase
    {
        private readonly RegistrationContextData registrationContextData;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IContainerContext containerContext;
        private readonly IExpressionBuilder expressionBuilder;

        public GenericTypeObjectBuilder(RegistrationContextData registrationContextData, IContainerContext containerContext,
            IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager, IExpressionBuilder expressionBuilder)
            : base(containerContext)
        {
            this.registrationContextData = registrationContextData;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType)
        {
            var genericType = this.metaInfoProvider.TypeTo.MakeGenericType(resolveType.GetGenericArguments());
            var typeInfo = new TypeInformation
            {
                Type = resolveType,
                DependencyName = NameGenerator.GetRegistrationName(resolveType, genericType)
            };

            this.RegisterConcreteGenericType(resolveType, genericType);

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            if (registration == null)
                throw new ResolutionFailedException(genericType.FullName);

            return registration.GetExpression(resolutionInfo, resolveType);
        }

        private void RegisterConcreteGenericType(Type resolveType, Type genericType)
        {
            var registrationContext = new ScopedRegistrationContext(resolveType, genericType, this.containerContext, this.expressionBuilder, this.containerExtensionManager);
            var newData = this.registrationContextData.CreateCopy();
            newData.Name = null;
            registrationContext.InitFromScope(newData);
        }
    }
}
