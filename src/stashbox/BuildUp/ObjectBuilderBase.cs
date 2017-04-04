using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        private readonly IContainerContext containerContext;

        protected ObjectBuilderBase(IContainerContext containerContext)
        {
            this.containerContext = containerContext;
        }

        public Expression GetExpression(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (serviceRegistration.IsDecorator)
                return this.GetExpressionAndHandleDisposal(serviceRegistration, resolutionInfo, resolveType);

            if (resolutionInfo.IsCurrentlyDecorating(resolveType))
                return this.GetExpressionAndHandleDisposal(serviceRegistration, resolutionInfo, resolveType);

            var decorators = this.containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (resolveType.IsClosedGenericType())
                {
                    decorators = this.containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
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

            foreach (var decoratorRegistration in decorators)
            {
                resolutionInfo.SetExpressionOverride(resolveType, expression);
                expression = decoratorRegistration.GetExpression(resolutionInfo, resolveType);
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
