using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IResolutionStrategy
    {
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInformation,
            HashSet<InjectionParameter> injectionParameters);

        ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            HashSet<InjectionParameter> injectionParameters);

        object EvaluateResolutionTarget(IContainerContext containerContext, ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo);
    }
}
