using Stashbox.Entity;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp.Resolution
{
    internal class DefaultValueResolver : Resolver
    {
        public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            if (typeInfo.HasDefaultValue)
                return Expression.Constant(typeInfo.DefaultValue, typeInfo.Type);

            return Expression.Default(typeInfo.Type);
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            containerContext.ContainerConfigurator.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled &&
                 (typeInfo.HasDefaultValue || typeInfo.Type.GetTypeInfo().IsValueType || typeInfo.Type == typeof(string) || typeInfo.IsMember);
    }
}
