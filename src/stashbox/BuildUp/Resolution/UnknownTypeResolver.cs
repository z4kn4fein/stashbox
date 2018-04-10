using Stashbox.Entity;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class UnknownTypeResolver : IResolver
    {
        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfigurator.ContainerConfiguration.UnknownTypeResolutionEnabled &&
                       typeInfo.Type.IsValidForRegistration();

        public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var context = containerContext.Container.ServiceRegistrator.PrepareContext(typeInfo.Type, typeInfo.Type);
            context.WithName(typeInfo.DependencyName);
            containerContext.ContainerConfigurator.ContainerConfiguration.UnknownTypeConfigurator?.Invoke(context);
            context.Register();

            return containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext, typeInfo, null);
        }
    }
}
