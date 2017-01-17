using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp.Resolution
{
    internal class ParentContainerResolver : Resolver
    {
        private readonly MethodInfo resolverMethodInfo;

        public ParentContainerResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.resolverMethodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("Resolve");
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            return base.BuilderContext.Container.ParentContainer.Resolve(base.TypeInfo.Type, base.TypeInfo.DependencyName,
                resolutionInfo.FactoryParams, resolutionInfo.OverrideManager?.GetOverrides());
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            var callExpression = Expression.Call(Expression.Constant(this), resolverMethodInfo, resolutionInfoExpression);
            return Expression.Convert(callExpression, base.TypeInfo.Type);
        }
    }
}
