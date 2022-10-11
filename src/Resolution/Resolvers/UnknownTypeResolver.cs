using Stashbox.Registration;
using Stashbox.Registration.Fluent;
using System;

namespace Stashbox.Resolution.Resolvers
{
    internal class UnknownTypeResolver : IServiceResolver, ILookup
    {
        public bool CanLookupService(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.CanUseForResolution(typeInfo, resolutionContext);

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled &&
            !resolutionContext.UnknownTypeCheckDisabled &&
            typeInfo.Type.IsResolvableType() ||
            resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator != null;

        public ServiceContext GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var name = typeInfo.DependencyName;
            var configurator = resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator;

            var unknownRegistrationConfigurator = new UnknownRegistrationConfigurator(typeInfo.Type, typeInfo.Type, name, resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.DefaultLifetime);
            configurator?.Invoke(unknownRegistrationConfigurator);

            if (!unknownRegistrationConfigurator.ImplementationType.IsResolvableType() ||
                !unknownRegistrationConfigurator.ImplementationType.Implements(unknownRegistrationConfigurator.ServiceType) ||
                unknownRegistrationConfigurator.RegistrationShouldBeSkipped)
                return ServiceContext.Empty;

            ServiceRegistrator.Register(resolutionContext.RequestInitiatorContainerContext, unknownRegistrationConfigurator, typeInfo.Type);

            return resolutionStrategy.BuildExpressionForRegistration(unknownRegistrationConfigurator, resolutionContext.ShouldFallBackToRequestInitiatorContext
                ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
                : resolutionContext,
                typeInfo);
        }
    }
}
