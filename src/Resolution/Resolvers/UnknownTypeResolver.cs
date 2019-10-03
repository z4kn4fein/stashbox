using Stashbox.Entity;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class UnknownTypeResolver : IResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfiguration.UnknownTypeResolutionEnabled &&
                       typeInfo.Type.IsResolvableType();

        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var configurator = typeInfo.DependencyName != null
                ? context => { context.WithName(typeInfo.DependencyName); containerContext.ContainerConfiguration.UnknownTypeConfigurator?.Invoke(context); }
            : containerContext.ContainerConfiguration.UnknownTypeConfigurator;

            containerContext.Container.Register(typeInfo.Type, configurator);
            return resolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, typeInfo);
        }
    }
}
