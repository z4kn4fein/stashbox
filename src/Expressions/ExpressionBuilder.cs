using Stashbox.Lifetime;
using Stashbox.Registration.ServiceRegistrations;
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

            if (serviceRegistration is not InstanceRegistration && serviceRegistration is ComplexRegistration complex)
            {
                if (complex.AsyncInitializer != null)
                    expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithAsyncInitializerMethod, expression,
                        complex.AsyncInitializer.AsConstant()).ConvertTo(requestedType);

                if (complex.Finalizer != null)
                    expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithFinalizerMethod, expression,
                        complex.Finalizer.AsConstant()).ConvertTo(requestedType);
            }
                
            if (!ShouldHandleDisposal(resolutionContext.CurrentContainerContext, serviceRegistration) || !expression.Type.IsDisposable())
                return expression;

            return resolutionContext.RequestConfiguration.RequiresRequestContext
                ? resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddRequestContextAwareDisposalMethod,
                    expression, resolutionContext.RequestContextParameter).ConvertTo(requestedType)
                : resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddDisposalMethod, expression).ConvertTo(requestedType);
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

        private static bool ShouldHandleDisposal(IContainerContext containerContext, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration is ComplexRegistration complex && complex.IsLifetimeExternallyOwned ||
                serviceRegistration is InstanceRegistration)
                return false;

            return containerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                   serviceRegistration.Lifetime is not TransientLifetime;
        }
    }
}
