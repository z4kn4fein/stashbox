using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Resolution;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ScopedInstanceResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            Expression.Convert(Expression.Call(resolutionContext.CurrentScopeParameter, Constants.GetScopedInstanceMethod, Expression.Constant(typeInfo.Type)), typeInfo.Type);

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.ResolutionScope.HasScopedInstances && resolutionContext.ResolutionScope.GetScopedInstanceOrDefault(typeInfo.Type) != null;
    }
}
