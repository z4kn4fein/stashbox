using Stashbox.Entity;
using Stashbox.Resolution;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ScopedInstanceResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.CurrentScopeParameter
                .CallMethod(Constants.GetScopedInstanceMethod, typeInfo.Type.AsConstant())
                .ConvertTo(typeInfo.Type);

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.ResolutionScope.HasScopedInstances && resolutionContext.ResolutionScope.GetScopedInstanceOrDefault(typeInfo.Type) != null;
    }
}
