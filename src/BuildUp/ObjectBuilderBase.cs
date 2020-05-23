using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        public virtual bool ProducesLifetimeManageableOutput => true;

        public Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.IsDecorator || resolutionContext.IsCurrentlyDecorating(resolveType))
                return this.BuildDisposalTrackingAndFinalizerExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            var decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (!resolveType.IsClosedGenericType())
                    return this.BuildDisposalTrackingAndFinalizerExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

                decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                if (decorators == null)
                    return this.BuildDisposalTrackingAndFinalizerExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
            }

            resolutionContext.AddCurrentlyDecoratingType(resolveType);
            var expression = this.BuildDisposalTrackingAndFinalizerExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            if (expression == null)
                return null;

            foreach (var decorator in decorators)
            {
                resolutionContext.SetExpressionOverride(resolveType, expression);
                expression = decorator.GetExpression(containerContext, resolutionContext, resolveType);
                if (expression == null)
                    return null;
            }

            resolutionContext.ClearCurrentlyDecoratingType(resolveType);
            return expression;
        }

        private Expression BuildDisposalTrackingAndFinalizerExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            var expr = this.GetExpressionInternal(containerContext, serviceRegistration, resolutionContext, resolveType);

            if (expr == null)
                return null;

            if (serviceRegistration.RegistrationContext.ExistingInstance == null && serviceRegistration.RegistrationContext.Finalizer != null)
                expr = this.BuildFinalizerExpression(expr, serviceRegistration, resolutionContext.CurrentScopeParameter);

            if (!serviceRegistration.ShouldHandleDisposal || !expr.Type.IsDisposable())
                return this.CheckRuntimeCircularDependencyExpression(expr, containerContext, serviceRegistration, resolutionContext, resolveType);

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expr.Type);
            return this.CheckRuntimeCircularDependencyExpression(resolutionContext.CurrentScopeParameter.CallMethod(method, expr),
                containerContext, serviceRegistration, resolutionContext, resolveType);
        }

        private Expression CheckRuntimeCircularDependencyExpression(Expression expression, IContainerContext containerContext,
            IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (!containerContext.ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled)
                return expression;

            var variable = resolveType.AsVariable();
            var exprs = new ExpandableArray<Expression>
            {
                resolutionContext.CurrentScopeParameter.CallMethod(
                    Constants.CheckRuntimeCircularDependencyBarrierMethod,
                    serviceRegistration.RegistrationId.AsConstant(), resolveType.AsConstant()),
                variable.AssignTo(expression),
                resolutionContext.CurrentScopeParameter.CallMethod(
                    Constants.ResetRuntimeCircularDependencyBarrierMethod,
                    serviceRegistration.RegistrationId.AsConstant()),
                variable
            };

            return exprs.AsBlock(variable);
        }

        private Expression BuildFinalizerExpression(Expression instanceExpression, IServiceRegistration serviceRegistration, Expression scopeExpression)
        {
            var addFinalizerMethod = Constants.AddWithFinalizerMethod.MakeGenericMethod(instanceExpression.Type);
            return scopeExpression.CallMethod(addFinalizerMethod, instanceExpression,
                serviceRegistration.RegistrationContext.Finalizer.AsConstant());
        }

        protected abstract Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType);
    }
}
