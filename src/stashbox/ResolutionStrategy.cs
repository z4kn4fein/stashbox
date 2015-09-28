using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox
{
    internal class ResolutionStrategy : IResolutionStrategy
    {
        private readonly IResolverSelector resolverSelector;

        public ResolutionStrategy(IResolverSelector resolverSelector)
        {
            this.resolverSelector = resolverSelector;
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInformation,
            HashSet<InjectionParameter> injectionParameters, string targetName)
        {
            return this.resolverSelector.CanResolve(containerContext, typeInformation) ||
                                          (injectionParameters != null &&
                                           injectionParameters.Any(injectionParameter =>
                                           injectionParameter.Name == targetName));
        }

        public ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation, HashSet<InjectionParameter> injectionParameters, string targetName)
        {
            Resolver resolver;
            this.resolverSelector.TryChooseResolver(containerContext, typeInformation, out resolver);

            return new ResolutionTarget
            {
                Resolver = resolver,
                TypeInformation = typeInformation,
                ResolutionTargetValue = injectionParameters?.FirstOrDefault(param => param.Name == targetName)?.Value,
                ResolutionTargetName = targetName
            };
        }

        public object EvaluateResolutionTarget(IContainerContext containerContext, ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo)
        {
            if (resolutionInfo.OverrideManager.ContainsValue(resolutionTarget.TypeInformation))
                return resolutionInfo.OverrideManager.GetOverriddenValue(resolutionTarget.TypeInformation.Type, resolutionTarget.TypeInformation.DependencyName);
            else if (resolutionTarget.ResolutionTargetValue != null)
                return resolutionTarget.ResolutionTargetValue;
            else
                return resolutionTarget.Resolver.Resolve(resolutionInfo);
        }
    }
}
