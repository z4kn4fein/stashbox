using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal interface IResolverSelector
    {
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo,
            ResolutionContext resolutionContext);

        Expression GetResolverExpression(IContainerContext containerContext, TypeInformation typeInfo,
            ResolutionContext resolutionContext, bool forceSkipUnknownTypeCheck = false);

        Expression[] GetResolverExpressions(IContainerContext containerContext,
            TypeInformation typeInfo, ResolutionContext resolutionContext);

        void AddResolver(IResolver resolver);
    }
}
