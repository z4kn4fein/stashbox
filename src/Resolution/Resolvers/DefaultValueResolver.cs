using Stashbox.Utils;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers;

internal class DefaultValueResolver : IServiceResolver
{
    public ServiceContext GetExpression(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext) =>
        typeInfo.Type.AsDefault().AsServiceContext();

    public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
        resolutionContext.CurrentContainerContext.ContainerConfiguration.DefaultValueInjectionEnabled &&
        (typeInfo.Type.IsValueType
         || typeInfo.Type == TypeCache<string>.Type);
}