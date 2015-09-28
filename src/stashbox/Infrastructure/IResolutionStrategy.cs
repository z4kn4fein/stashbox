using Stashbox.Entity;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IResolutionStrategy
    {
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInformation,
            HashSet<InjectionParameter> injectionParameters, string targetName);

        ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            HashSet<InjectionParameter> injectionParameters, string targetName);

        object EvaluateResolutionTarget(IContainerContext containerContext, ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo);
    }
}
