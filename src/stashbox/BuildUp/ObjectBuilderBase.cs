using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        public Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (serviceRegistration.IsDecorator || resolutionInfo.IsCurrentlyDecorating(resolveType))
                return this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionInfo, resolveType);

            var decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (!resolveType.IsClosedGenericType())
                    return this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionInfo, resolveType);

                decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                if (decorators == null)
                    return this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionInfo, resolveType);
            }

            resolutionInfo.AddCurrentlyDecoratingType(resolveType);
            var expression = this.GetExpressionAndHandleDisposal(containerContext, serviceRegistration, resolutionInfo, resolveType);

            if (expression == null)
                return null;

            var length = decorators.Length;

            for (int i = 0; i < length; i++)
            {
                var decorator = decorators[i];
                resolutionInfo.SetExpressionOverride(resolveType, expression);
                expression = decorator.Value.GetExpression(containerContext, resolutionInfo, resolveType);
                if (expression == null)
                    return null;
            }

            resolutionInfo.ClearCurrentlyDecoratingType(resolveType);
            return expression;
        }

        private Expression GetExpressionAndHandleDisposal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            var expr = this.GetExpressionInternal(containerContext, serviceRegistration, resolutionInfo, resolveType);

            if (expr == null)
                return null;

            if (!this.HandlesObjectLifecycle && serviceRegistration.RegistrationContext.Finalizer != null)
                expr = this.HandleFinalizer(expr, serviceRegistration);

            if (!serviceRegistration.ShouldHandleDisposal || this.HandlesObjectLifecycle || !expr.Type.IsDisposable())
                return expr;

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expr.Type);
            return Expression.Call(Constants.ScopeExpression, method, expr);
        }

        protected Expression HandleFinalizer(Expression instanceExpression, IServiceRegistration serviceRegistration)
        {
            var addFinalizerMethod = Constants.AddWithFinalizerMethod.MakeGenericMethod(instanceExpression.Type);
            return Expression.Call(Constants.ScopeExpression, addFinalizerMethod, instanceExpression,
                Expression.Constant(serviceRegistration.RegistrationContext.Finalizer));
        }

        protected abstract Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType);

        public virtual bool HandlesObjectLifecycle => false;

        public virtual IObjectBuilder Produce() => this;
    }
}
