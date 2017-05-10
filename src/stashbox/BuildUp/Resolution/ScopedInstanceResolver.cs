using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.BuildUp.Resolution
{
    internal class ScopedInstanceResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            Expression.Convert(Expression.Call(Constants.ScopeExpression, Constants.GetScopedInstanceMethod, Expression.Constant(typeInfo.Type)), typeInfo.Type);

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            resolutionInfo.ResolutionScope.HasScopedInstances && resolutionInfo.ResolutionScope.GetScopedInstanceOrDefault(typeInfo.Type) != null;
    }
}
