using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ScopedInstanceResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            Expression.Convert(Expression.Call(resolutionInfo.CurrentScopeParameter, Constants.GetScopedInstanceMethod, Expression.Constant(typeInfo.Type)), typeInfo.Type);

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            resolutionInfo.ResolutionScope.HasScopedInstances && resolutionInfo.ResolutionScope.GetScopedInstanceOrDefault(typeInfo.Type) != null;
    }
}
