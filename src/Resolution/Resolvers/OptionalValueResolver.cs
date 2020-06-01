using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class OptionalValueResolver : IResolver
    {
        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            typeInfo.DefaultValue.AsConstant(typeInfo.Type);

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.HasDefaultValue;
    }
}
