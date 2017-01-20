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
        private readonly MethodInfo resolverMethodInfo;

        public DefaultValueResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.resolverMethodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("Resolve");
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            if (base.TypeInfo.HasDefaultValue)
                return base.TypeInfo.DefaultValue;

            if (base.TypeInfo.Type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(base.TypeInfo.Type);

            if (base.TypeInfo.Type == typeof(string))
                return null;

            throw new ResolutionFailedException(base.TypeInfo.Type.FullName);
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            var callExpression = Expression.Call(Expression.Constant(this), resolverMethodInfo, resolutionInfoExpression);
            return Expression.Convert(callExpression, base.TypeInfo.Type);
        }
    }
}
