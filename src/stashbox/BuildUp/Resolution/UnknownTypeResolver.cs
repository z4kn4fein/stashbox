using System;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp.Resolution
{
    internal class UnknownTypeResolver : Resolver
    {
        private readonly MethodInfo resolverMethodInfo;

        public UnknownTypeResolver(IContainerContext containerContext, TypeInformation typeInfo) 
            :base(containerContext, typeInfo)
        {
            this.resolverMethodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("Resolve");
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            base.BuilderContext.Container.RegisterType(base.TypeInfo.Type, base.TypeInfo.Type, base.TypeInfo.DependencyName);
            return base.BuilderContext.Container.Resolve(base.TypeInfo.Type, base.TypeInfo.DependencyName);
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            var callExpression = Expression.Call(Expression.Constant(this), resolverMethodInfo, resolutionInfoExpression);
            return Expression.Convert(callExpression, base.TypeInfo.Type);
        }
    }
}
