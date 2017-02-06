using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Resolution
{
    internal class DefaultValueResolver : Resolver
    {
        public DefaultValueResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
        }
        
        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            if (base.TypeInfo.HasDefaultValue)
                return Expression.Constant(base.TypeInfo.DefaultValue, base.TypeInfo.Type);

            if (base.TypeInfo.Type.GetTypeInfo().IsValueType)
                return Expression.Constant(Activator.CreateInstance(base.TypeInfo.Type), base.TypeInfo.Type);

            if (base.TypeInfo.Type == typeof(string) || base.TypeInfo.IsMember)
                return Expression.Constant(null, base.TypeInfo.Type);

            throw new ResolutionFailedException(base.TypeInfo.Type.FullName);
        }
    }
}
