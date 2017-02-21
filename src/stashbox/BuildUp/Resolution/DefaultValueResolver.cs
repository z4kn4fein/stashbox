using Stashbox.Entity;
using Stashbox.Exceptions;
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

            if (typeInfo.Type.GetTypeInfo().IsValueType)
                return Expression.Constant(Activator.CreateInstance(typeInfo.Type), typeInfo.Type);

            if (typeInfo.Type == typeof(string) || typeInfo.IsMember)
                return Expression.Constant(null, typeInfo.Type);

            throw new ResolutionFailedException(typeInfo.Type.FullName);
        }

        public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo) =>
            containerContext.ContainerConfigurator.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled &&
                 (typeInfo.HasDefaultValue || typeInfo.Type.GetTypeInfo().IsValueType || typeInfo.Type == typeof(string) || typeInfo.IsMember);
    }
}
