using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        public Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.IsDecorator || resolutionContext.IsCurrentlyDecorating(resolveType))
                return this.HandleDisposalAndGetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            var decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (!resolveType.IsClosedGenericType())
                    return this.HandleDisposalAndGetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

                decorators = containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                if (decorators == null)
                    return this.HandleDisposalAndGetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
            }

            resolutionContext.AddCurrentlyDecoratingType(resolveType);
            var expression = this.HandleDisposalAndGetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

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

        private Expression CheckRuntimeCircularDependenciesIfNeeded(Expression expression, IContainerContext containerContext,
            IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (!containerContext.ContainerConfigurator.ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled)
                return expression;

            var exprs = new List<Expression>();
            var variable = resolveType.AsVariable();

            exprs.Add(resolutionContext.CurrentScopeParameter.CallMethod(Constants.CheckRuntimeCircularDependencyBarrierMethod,
                serviceRegistration.RegistrationNumber.AsConstant(), resolveType.AsConstant()));
            exprs.Add(variable.AssignTo(expression));
            exprs.Add(resolutionContext.CurrentScopeParameter.CallMethod(Constants.ResetRuntimetCircularDependencyBarrierMethod, serviceRegistration.RegistrationNumber.AsConstant()));
            exprs.Add(variable);

            return exprs.AsBlock(variable);
        }

        private Expression HandleDisposalAndGetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            var expr = this.GetExpressionInternal(containerContext, serviceRegistration, resolutionContext, resolveType);

            if (expr == null)
                return null;

            if (serviceRegistration.RegistrationContext.ExistingInstance == null && serviceRegistration.RegistrationContext.Finalizer != null)
                expr = this.HandleFinalizer(expr, serviceRegistration, resolutionContext.CurrentScopeParameter);

            if (!serviceRegistration.ShouldHandleDisposal || !expr.Type.IsDisposable())
                return this.CheckRuntimeCircularDependenciesIfNeeded(expr, containerContext, serviceRegistration, resolutionContext, resolveType);

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expr.Type);
            return this.CheckRuntimeCircularDependenciesIfNeeded(resolutionContext.CurrentScopeParameter.CallMethod(method, expr),
                containerContext, serviceRegistration, resolutionContext, resolveType);
        }

        private Expression HandleFinalizer(Expression instanceExpression, IServiceRegistration serviceRegistration, Expression scopeExpression)
        {
            var addFinalizerMethod = Constants.AddWithFinalizerMethod.MakeGenericMethod(instanceExpression.Type);
            return scopeExpression.CallMethod(addFinalizerMethod, instanceExpression,
                serviceRegistration.RegistrationContext.Finalizer.AsConstant());
        }

        protected abstract Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType);
    }
}
