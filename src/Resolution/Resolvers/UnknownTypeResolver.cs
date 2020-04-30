using Stashbox.Entity;
using Stashbox.Registration.Fluent;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class UnknownTypeResolver : IResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfiguration.UnknownTypeResolutionEnabled &&
            typeInfo.Type.IsResolvableType() ||
            containerContext.ContainerConfiguration.UnknownTypeConfigurator != null;

        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var configurator = typeInfo.DependencyName != null
                ? context => { context.WithName(typeInfo.DependencyName); containerContext.ContainerConfiguration.UnknownTypeConfigurator?.Invoke(context); }
            : containerContext.ContainerConfiguration.UnknownTypeConfigurator;

            var registrationConfigurator = new RegistrationConfigurator(typeInfo.Type, typeInfo.Type);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out _))
                return null;

            containerContext.Container.Register(typeInfo.Type, configurator);
            return resolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, typeInfo);
        }
    }
}
