using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal static partial class ExpressionBuilder
    {
        internal static Expression BuildExpressionForRegistration(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType)
        {
            var expression = BuildExpressionByRegistrationType(serviceRegistration, resolutionContext, requestedType);
            if (expression == null)
                return null;

            if (serviceRegistration.RegistrationContext.ExistingInstance == null && serviceRegistration.RegistrationContext.AsyncInitializer != null)
                expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithAsyncInitializerMethod, expression,
                    serviceRegistration.RegistrationContext.AsyncInitializer.AsConstant()).ConvertTo(requestedType);

            if (serviceRegistration.RegistrationContext.ExistingInstance == null && serviceRegistration.RegistrationContext.Finalizer != null)
                expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithFinalizerMethod, expression,
                    serviceRegistration.RegistrationContext.Finalizer.AsConstant()).ConvertTo(requestedType);

            if (!ShouldHandleDisposal(resolutionContext.CurrentContainerContext, serviceRegistration) || !expression.Type.IsDisposable())
                return CheckRuntimeCircularDependencyExpression(expression, serviceRegistration, resolutionContext, requestedType);

            return CheckRuntimeCircularDependencyExpression(resolutionContext.CurrentScopeParameter
                .CallMethod(Constants.AddDisposalMethod, expression).ConvertTo(requestedType),
                serviceRegistration, resolutionContext, requestedType);
        }

        private static Expression BuildExpressionByRegistrationType(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type requestedType)
        {
            resolutionContext = resolutionContext.ShouldFallBackToRequestInitiatorContext
                ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
                : resolutionContext;

            return serviceRegistration.RegistrationType switch
            {
                RegistrationType.Factory => GetExpressionForFactory(serviceRegistration, resolutionContext, requestedType),
                RegistrationType.Instance => serviceRegistration.RegistrationContext.ExistingInstance.AsConstant(),
                RegistrationType.WireUp => ExpressionFactory.ConstructBuildUpExpression(serviceRegistration,
                    resolutionContext, serviceRegistration.RegistrationContext.ExistingInstance.AsConstant(),
                    serviceRegistration.ImplementationType),
                RegistrationType.Func => GetExpressionForFunc(serviceRegistration, resolutionContext),
                _ => GetExpressionForDefault(serviceRegistration, resolutionContext)
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

        private static bool ShouldHandleDisposal(IContainerContext containerContext, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration.RegistrationContext.IsLifetimeExternallyOwned ||
                serviceRegistration.RegistrationContext.ExistingInstance != null)
                return false;

            return containerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                   serviceRegistration.RegistrationContext.Lifetime is not TransientLifetime;
        }
    }
}
