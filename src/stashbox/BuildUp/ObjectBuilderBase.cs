using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        public Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.IsDecorator || resolutionContext.IsCurrentlyDecorating(resolveType))
                return this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionContext, resolveType);

            var decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (!resolveType.IsClosedGenericType())
                    return this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionContext, resolveType);

                decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                if (decorators == null)
                    return this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionContext, resolveType);
            }

            resolutionContext.AddCurrentlyDecoratingType(resolveType);
            var expression = this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionContext, resolveType);

            if (expression == null)
                return null;

            var length = decorators.Length;

            for (int i = 0; i < length; i++)
            {
                var decorator = decorators[i];
                resolutionContext.SetExpressionOverride(resolveType, expression);
                expression = decorator.Value.GetExpression(containerContext, resolutionContext, resolveType);
                if (expression == null)
                    return null;
            }

            resolutionContext.ClearCurrentlyDecoratingType(resolveType);
            return expression;
        }

        private Expression GetExpressionAndHandleDisposal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            var expr = this.GetExpressionInternal(containerContext, serviceRegistration, resolutionContext, resolveType);

            if (expr == null)
                return null;

            if (!this.HandlesObjectLifecycle && serviceRegistration.RegistrationContext.Finalizer != null)
                expr = this.HandleFinalizer(expr, serviceRegistration, resolutionContext);

            if (!serviceRegistration.ShouldHandleDisposal || this.HandlesObjectLifecycle || !expr.Type.IsDisposable())
                return expr;

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expr.Type);
            return Expression.Call(resolutionContext.CurrentScopeParameter, method, expr);
        }

        protected Expression HandleFinalizer(Expression instanceExpression, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            var addFinalizerMethod = Constants.AddWithFinalizerMethod.MakeGenericMethod(instanceExpression.Type);
            return Expression.Call(resolutionContext.CurrentScopeParameter, addFinalizerMethod, instanceExpression,
                Expression.Constant(serviceRegistration.RegistrationContext.Finalizer));
        }

        protected abstract Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType);

        public virtual bool HandlesObjectLifecycle => false;

        public virtual IObjectBuilder Produce() => this;
    }
}
