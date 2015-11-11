using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox
{
    public class ResolutionStrategy : IResolutionStrategy
    {
        private readonly IResolverSelector resolverSelector;

        internal ResolutionStrategy(IResolverSelector resolverSelector)
        {
            this.resolverSelector = resolverSelector;
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            return this.resolverSelector.CanResolve(containerContext, typeInformation) ||
                                          (injectionParameters != null &&
                                           injectionParameters.Any(injectionParameter =>
                                           injectionParameter.Name == typeInformation.MemberName));
        }

        public ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            Resolver resolver;
            this.resolverSelector.TryChooseResolver(containerContext, typeInformation, out resolver);

            return new ResolutionTarget
            {
                Resolver = resolver,
                TypeInformation = typeInformation,
                ResolutionTargetValue = injectionParameters?.FirstOrDefault(param => param.Name == typeInformation.MemberName)?.Value
            };
        }

        public object EvaluateResolutionTarget(IContainerContext containerContext, ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo)
        {
            if (resolutionInfo.OverrideManager != null && resolutionInfo.OverrideManager.ContainsValue(resolutionTarget.TypeInformation))
                return resolutionInfo.OverrideManager.GetOverriddenValue(resolutionTarget.TypeInformation.Type, resolutionTarget.TypeInformation.DependencyName);
            return resolutionTarget.ResolutionTargetValue ?? resolutionTarget.Resolver.Resolve(resolutionInfo);
        }
    }
}
