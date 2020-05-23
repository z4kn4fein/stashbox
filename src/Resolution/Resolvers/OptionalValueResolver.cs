using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    class OptionalValueResolver : IResolver
    {
        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            typeInfo.DefaultValue.AsConstant(typeInfo.Type);

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.HasDefaultValue;
    }
}
