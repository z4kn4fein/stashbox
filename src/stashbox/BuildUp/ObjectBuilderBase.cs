using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        protected readonly IContainerContext ContainerContext;

        protected ObjectBuilderBase(IContainerContext containerContext)
        {
            this.ContainerContext = containerContext;
        }

        public Expression GetExpression(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (serviceRegistration.IsDecorator)
                return this.GetExpressionAndHandleDisposal(serviceRegistration, resolutionInfo, resolveType);

            if (resolutionInfo.IsCurrentlyDecorating(resolveType))
                return this.GetExpressionAndHandleDisposal(serviceRegistration, resolutionInfo, resolveType);

            var decorators = this.ContainerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (resolveType.IsClosedGenericType())
                {
                    decorators = this.ContainerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                    if (decorators == null)
                        return this.GetExpressionAndHandleDisposal(serviceRegistration, resolutionInfo, resolveType);
                }
                else
                    return this.GetExpressionAndHandleDisposal(serviceRegistration, resolutionInfo, resolveType);
            }

            resolutionInfo.AddCurrentlyDecoratingType(resolveType);
            var expression = this.GetExpressionAndHandleDisposal(serviceRegistration, resolutionInfo, resolveType);

            if (expression == null)
                return null;

            var length = decorators.Length;

            for (int i = 0; i < length; i++)
            {
                var decorator = decorators[i];
                resolutionInfo.SetExpressionOverride(resolveType, expression);
                expression = decorator.Value.GetExpression(resolutionInfo, resolveType);
                if (expression == null)
                    return null;
            }

            resolutionInfo.ClearCurrentlyDecoratingType(resolveType);
            return expression;
        }

        private Expression GetExpressionAndHandleDisposal(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            var expr = this.GetExpressionInternal(serviceRegistration, resolutionInfo, resolveType);

            if (expr == null || !serviceRegistration.ShouldHandleDisposal || this.HandlesObjectDisposal || !expr.Type.IsDisposable())
                return expr;

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expr.Type);
            return Expression.Call(Constants.ScopeExpression, method, expr);
        }

        protected abstract Expression GetExpressionInternal(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType);

        public virtual bool HandlesObjectDisposal => false;
    }
}
