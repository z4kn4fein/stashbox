using Stashbox.Entity;
using Stashbox.Resolution;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class DefaultValueResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            if (typeInfo.HasDefaultValue)
                return typeInfo.DefaultValue.AsConstant(typeInfo.Type);

            return typeInfo.Type.AsDefault();
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfigurator.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled &&
                 (typeInfo.HasDefaultValue || typeInfo.Type.GetTypeInfo().IsValueType || typeInfo.Type == typeof(string) || typeInfo.IsMember);
    }
}
