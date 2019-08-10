using Stashbox.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Resolution.Resolvers
{
    internal class DefaultValueResolver : IResolver
    {
        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            if (typeInfo.HasDefaultValue)
                return typeInfo.DefaultValue.AsConstant(typeInfo.Type);

            return typeInfo.Type.AsDefault();
        }

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled &&
                 (typeInfo.HasDefaultValue
                    || typeInfo.Type.GetTypeInfo().IsValueType
                    || typeInfo.Type == typeof(string)
                    || typeInfo.MemberType != MemberType.None);
    }
}
