using System;
using System.Linq.Expressions;
using Stashbox.Utils;

namespace Stashbox.Resolution.Resolvers;

internal class ServiceProviderResolver : IServiceResolver
{
    public ServiceContext GetExpression(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext) =>
        resolutionContext.CurrentScopeParameter.AsServiceContext();

    public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
        typeInfo.Type == TypeCache<IServiceProvider>.Type;
}