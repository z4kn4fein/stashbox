using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class OptionalValueResolver : IServiceResolver
    {
        public ServiceContext GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            typeInfo.DefaultValue.AsConstant(typeInfo.Type).AsContext();

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.HasDefaultValue;
    }
}
