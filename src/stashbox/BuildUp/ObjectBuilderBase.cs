using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        private readonly IContainerContext containerContext;
        private readonly bool isDecorator;
        private readonly bool shouldHandleDisposal;

        protected ObjectBuilderBase(IContainerContext containerContext, bool isDecorator, bool shouldHandleDisposal)
        {
            this.containerContext = containerContext;
            this.isDecorator = isDecorator;
            this.shouldHandleDisposal = shouldHandleDisposal;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.isDecorator)
                return this.GetExpressionAndHandleDisposal(resolutionInfo, resolveType);

            var decoratedType = resolutionInfo.CurrentlyDecoratingTypes.GetOrDefault(resolveType.GetHashCode());
            if (decoratedType != null)
                return this.GetExpressionAndHandleDisposal(resolutionInfo, resolveType);

            var decorators = this.containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (resolveType.IsClosedGenericType())
                {
                    decorators = this.containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                    if (decorators == null)
                        return this.GetExpressionAndHandleDisposal(resolutionInfo, resolveType);
                }
                else
                    return this.GetExpressionAndHandleDisposal(resolutionInfo, resolveType);
            }

            resolutionInfo.CurrentlyDecoratingTypes.AddOrUpdate(resolveType.GetHashCode(), resolveType);
            var expression = this.GetExpressionAndHandleDisposal(resolutionInfo, resolveType);

            if (expression == null)
                return null;

            foreach (var decoratorRegistration in decorators)
            {
                resolutionInfo.ExpressionOverrides.AddOrUpdate(resolveType, expression, (oldValue, newValue) => newValue);
                expression = decoratorRegistration.GetExpression(resolutionInfo, resolveType);
                if (expression == null)
                    return null;
            }

            resolutionInfo.CurrentlyDecoratingTypes.AddOrUpdate(resolveType.GetHashCode(), null, (oldValue, newValue) => newValue);
            return expression;
        }

        private Expression GetExpressionAndHandleDisposal(ResolutionInfo resolutionInfo, Type resolveType)
        {
            var expr = this.GetExpressionInternal(resolutionInfo, resolveType);
            
            if (expr == null || !this.shouldHandleDisposal || this.HandlesObjectDisposal || !expr.Type.IsDisposable())
                return expr;

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expr.Type);
            return Expression.Call(Constants.ScopeExpression, method, expr);
        }

        protected abstract Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType);

        public virtual bool HandlesObjectDisposal => false;
    }
}
