using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface IResolutionStrategy
    {
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters);

        ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters);

        object EvaluateResolutionTarget(IContainerContext containerContext, ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo);
        Expression GetExpressionForResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo);
    }
}
