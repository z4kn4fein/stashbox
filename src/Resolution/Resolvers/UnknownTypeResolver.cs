using Stashbox.Expressions;
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
        private readonly ExpressionBuilder expressionBuilder;

        public UnknownTypeResolver(ServiceRegistrator serviceRegistrator, RegistrationBuilder registrationBuilder, ExpressionBuilder expressionBuilder)
        {
            this.serviceRegistrator = serviceRegistrator;
            this.registrationBuilder = registrationBuilder;
            this.expressionBuilder = expressionBuilder;
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
            var configurator = typeInfo.DependencyName != null
                ? context =>
                {
                    context.WithName(typeInfo.DependencyName);
                    resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator?.Invoke(context);
                }
            : resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator;

            var registrationConfigurator = new UnknownRegistrationConfigurator(typeInfo.Type, typeInfo.Type);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out _))
                return null;

            var registration = this.registrationBuilder.BuildServiceRegistration(resolutionContext.RequestInitiatorContainerContext,
                registrationConfigurator, false);
            this.serviceRegistrator.Register(resolutionContext.RequestInitiatorContainerContext, registration, typeInfo.Type);

            return this.expressionBuilder.BuildExpressionAndApplyLifetime(registration, resolutionContext, typeInfo.Type);
        }
    }
}
