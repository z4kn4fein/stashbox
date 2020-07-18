using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal partial class ExpressionBuilder
    {
        private readonly ExpressionFactory expressionFactory;

        internal ExpressionBuilder(ExpressionFactory expressionFactory)
        {
            this.expressionFactory = expressionFactory;
        }

        internal Expression BuildExpressionAndApplyLifetime(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType, LifetimeDescriptor secondaryLifetimeDescriptor = null)
        {
            var lifetimeDescriptor = serviceRegistration.RegistrationContext.Lifetime ?? secondaryLifetimeDescriptor;
            if (!IsOutputLifetimeManageable(serviceRegistration) || lifetimeDescriptor == null)
                return this.BuildExpressionForRegistration(serviceRegistration, resolutionContext, requestedType);

            return lifetimeDescriptor.ApplyLifetime(this, serviceRegistration, resolutionContext, requestedType);
        }

        internal Expression BuildExpressionForRegistration(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType)
        {
            var expression = this.BuildExpressionByRegistrationType(serviceRegistration, resolutionContext, requestedType);
            if (expression == null)
                return null;

            if (serviceRegistration.RegistrationContext.ExistingInstance == null && serviceRegistration.RegistrationContext.Finalizer != null)
                expression = BuildFinalizerExpression(expression, serviceRegistration, resolutionContext.CurrentScopeParameter);

            if (!ShouldHandleDisposal(resolutionContext.CurrentContainerContext, serviceRegistration) || !expression.Type.IsDisposable())
                return CheckRuntimeCircularDependencyExpression(expression, serviceRegistration, resolutionContext, requestedType);

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expression.Type);
            return CheckRuntimeCircularDependencyExpression(resolutionContext.CurrentScopeParameter.CallMethod(method, expression),
                serviceRegistration, resolutionContext, requestedType);
        }

        private Expression BuildExpressionByRegistrationType(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type requestedType)
        {
            resolutionContext = resolutionContext.ShouldFallBackToRequestInitiatorContext
                ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
                : resolutionContext;

            return serviceRegistration.RegistrationType switch
            {
                RegistrationType.Factory => this.GetExpressionForFactory(serviceRegistration, resolutionContext,
                    requestedType),
                RegistrationType.Instance => serviceRegistration.RegistrationContext.ExistingInstance.AsConstant(),
                RegistrationType.WireUp => this.expressionFactory.ConstructBuildUpExpression(serviceRegistration,
                    resolutionContext, serviceRegistration.RegistrationContext.ExistingInstance.AsConstant(),
                    serviceRegistration.ImplementationType),
                RegistrationType.Func => this.GetExpressionForFunc(serviceRegistration, resolutionContext),
                _ => this.GetExpressionForDefault(serviceRegistration, resolutionContext)
            };
        }

        private static Expression CheckRuntimeCircularDependencyExpression(Expression expression,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type requestedType)
        {
            if (!resolutionContext.CurrentContainerContext.ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled)
                return expression;

            var variable = requestedType.AsVariable();
            var expressions = new Expression[]
            {
                resolutionContext.CurrentScopeParameter.CallMethod(
                    Constants.CheckRuntimeCircularDependencyBarrierMethod,
                    serviceRegistration.RegistrationId.AsConstant(), requestedType.AsConstant()),
                variable.AssignTo(expression),
                resolutionContext.CurrentScopeParameter.CallMethod(
                    Constants.ResetRuntimeCircularDependencyBarrierMethod,
                    serviceRegistration.RegistrationId.AsConstant()),
                variable
            };

            return expressions.AsBlock(variable);
        }

        private static Expression BuildFinalizerExpression(Expression instanceExpression, ServiceRegistration serviceRegistration, Expression scopeExpression)
        {
            var addFinalizerMethod = Constants.AddWithFinalizerMethod.MakeGenericMethod(instanceExpression.Type);
            return scopeExpression.CallMethod(addFinalizerMethod, instanceExpression,
                serviceRegistration.RegistrationContext.Finalizer.AsConstant());
        }

        private static bool IsOutputLifetimeManageable(ServiceRegistration serviceRegistration) =>
            serviceRegistration.RegistrationType != RegistrationType.OpenGeneric &&
            serviceRegistration.RegistrationType != RegistrationType.Instance;

        private static bool ShouldHandleDisposal(IContainerContext containerContext, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration.RegistrationContext.IsLifetimeExternallyOwned ||
                serviceRegistration.RegistrationContext.ExistingInstance != null)
                return false;

            return containerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                   !(serviceRegistration.RegistrationContext.Lifetime is TransientLifetime);
        }
    }
}
