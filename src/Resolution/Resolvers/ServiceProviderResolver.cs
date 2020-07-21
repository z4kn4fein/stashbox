#if HAS_SERVICEPROVIDER
using Stashbox.Utils;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class ServiceProviderResolver : IResolver
    {
        public Expression GetExpression(IResolutionStrategy resolutionStrategy, TypeInformation typeInfo,
            ResolutionContext resolutionContext) => resolutionContext.CurrentScopeParameter;

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            typeInfo.Type == Constants.ServiceProviderType;
    }
}
#endif