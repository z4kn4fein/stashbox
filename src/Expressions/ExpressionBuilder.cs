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
        internal static Expression? BuildExpressionForRegistration(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType)
        {
            var expression = BuildExpressionByRegistrationType(serviceRegistration, resolutionContext, requestedType);
            if (expression == null)
                return null;

            if (serviceRegistration is not InstanceRegistration && serviceRegistration.AsyncInitializer != null)
                expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithAsyncInitializerMethod, expression,
                    serviceRegistration.AsyncInitializer.AsConstant()).ConvertTo(requestedType);

            if (serviceRegistration is not InstanceRegistration && serviceRegistration.Finalizer != null)
                expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithFinalizerMethod, expression,
                    serviceRegistration.Finalizer.AsConstant()).ConvertTo(requestedType);

            if (!ShouldHandleDisposal(resolutionContext.CurrentContainerContext, serviceRegistration) || !expression.Type.IsDisposable())
                return CheckRuntimeCircularDependencyExpression(expression, serviceRegistration, resolutionContext, requestedType);

            var disposeTrackingExpression = resolutionContext.RequestConfiguration.RequiresRequestContext 
                ? resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddRequestContextAwareDisposalMethod, 
                    expression, resolutionContext.RequestContextParameter).ConvertTo(requestedType)
                : resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddDisposalMethod, expression).ConvertTo(requestedType);

            return CheckRuntimeCircularDependencyExpression(disposeTrackingExpression, serviceRegistration, resolutionContext, requestedType);
        }

        private static Expression? BuildExpressionByRegistrationType(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type requestedType)
        {
            resolutionContext = resolutionContext.ShouldFallBackToRequestInitiatorContext
                ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
                : resolutionContext;

            return serviceRegistration switch
            {
                FactoryRegistration factoryRegistration => GetExpressionForFactory(factoryRegistration, resolutionContext, requestedType),
                InstanceRegistration instanceRegistration => instanceRegistration.IsWireUp 
                    ? ExpressionFactory.ConstructBuildUpExpression(serviceRegistration,
                        resolutionContext, instanceRegistration.ExistingInstance.AsConstant(),
                        serviceRegistration.ImplementationType)
                    : instanceRegistration.ExistingInstance.AsConstant(),
                FuncRegistration funcRegistration => GetExpressionForFunc(funcRegistration, resolutionContext),
                _ => GetExpressionForDefault(serviceRegistration, resolutionContext)
            };
        }

        private static Expression CheckRuntimeCircularDependencyExpression(Expression expression,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type requestedType)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (!resolutionContext.CurrentContainerContext.ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled)
#pragma warning restore CS0618 // Type or member is obsolete
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
            if (serviceRegistration.IsLifetimeExternallyOwned ||
                serviceRegistration is InstanceRegistration)
                return false;

            return containerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                   serviceRegistration.Lifetime is not TransientLifetime;
        }
    }
}
