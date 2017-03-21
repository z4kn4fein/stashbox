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

        protected ObjectBuilderBase(IContainerContext containerContext, bool isDecorator)
        {
            this.containerContext = containerContext;
            this.isDecorator = isDecorator;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.isDecorator)
                return this.GetExpressionInternal(resolutionInfo, resolveType);

            var decoratedType = resolutionInfo.CurrentlyDecoratingTypes.GetOrDefault(resolveType.GetHashCode());
            if (decoratedType != null)
                return this.GetExpressionInternal(resolutionInfo, resolveType);

            var decorators = this.containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (resolveType.IsClosedGenericType())
                {
                    decorators = this.containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                    if (decorators == null)
                        return this.GetExpressionInternal(resolutionInfo, resolveType);
                }
                else
                    return this.GetExpressionInternal(resolutionInfo, resolveType);
            }

            resolutionInfo.CurrentlyDecoratingTypes.AddOrUpdate(resolveType.GetHashCode(), resolveType);
            var expression = this.GetExpressionInternal(resolutionInfo, resolveType);

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

        protected abstract Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType);

        public virtual bool HandlesObjectDisposal => false;

        public virtual void CleanUp()
        { }
    }
}
