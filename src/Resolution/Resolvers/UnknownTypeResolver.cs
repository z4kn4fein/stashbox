using Stashbox.Registration;
using Stashbox.Registration.Fluent;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class UnknownTypeResolver : IResolver
    {
        private readonly ServiceRegistrator serviceRegistrator;
        private readonly RegistrationBuilder registrationBuilder;

        public UnknownTypeResolver(ServiceRegistrator serviceRegistrator, RegistrationBuilder registrationBuilder)
        {
            this.serviceRegistrator = serviceRegistrator;
            this.registrationBuilder = registrationBuilder;
        }

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled &&
            !resolutionContext.UnknownTypeCheckDisabled &&
            typeInfo.Type.IsResolvableType() ||
            resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator != null;

        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var name = typeInfo.DependencyName;
            var configurator = name != null
                ? context =>
                {
                    context.WithName(name);
                    resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator?.Invoke(context);
                }
            : resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator;

            var registrationConfigurator = new UnknownRegistrationConfigurator(typeInfo.Type, typeInfo.Type);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out _) || registrationConfigurator.RegistrationShouldBeSkipped)
                return null;

            var registration = this.registrationBuilder.BuildServiceRegistration(resolutionContext.RequestInitiatorContainerContext,
                registrationConfigurator, false);
            this.serviceRegistrator.Register(resolutionContext.RequestInitiatorContainerContext, registration, typeInfo.Type);

            return resolutionStrategy.BuildExpressionForRegistration(registration, resolutionContext.ShouldFallBackToRequestInitiatorContext
                ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
                : resolutionContext,
                typeInfo);
        }
    }
}
