using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Resolution.Resolvers
{
    internal class DefaultValueResolver : IResolver
    {
        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            typeInfo.Type.AsDefault();

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.CurrentContainerContext.ContainerConfiguration.DefaultValueInjectionEnabled &&
                 (typeInfo.Type.IsValueType
                    || typeInfo.Type == typeof(string));
    }
}
