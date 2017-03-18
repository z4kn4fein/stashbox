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
            if (this.isDecorator || resolutionInfo.CurrentlyDecoratingTypes.Contains(resolveType))
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

            resolutionInfo.CurrentlyDecoratingTypes.Add(resolveType);
            var expression = this.GetExpressionInternal(resolutionInfo, resolveType);

            foreach (var decoratorRegistration in decorators)
            {
                resolutionInfo.ExpressionOverrides.AddOrUpdate(resolveType, expression, (oldValue, newValue) => newValue);
                expression = decoratorRegistration.GetExpression(resolutionInfo, resolveType);
            }

            resolutionInfo.CurrentlyDecoratingTypes.Remove(resolveType);
            return expression;
        }

        protected abstract Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType);

        public virtual bool HandlesObjectDisposal => false;

        public virtual void CleanUp()
        { }
    }
}
