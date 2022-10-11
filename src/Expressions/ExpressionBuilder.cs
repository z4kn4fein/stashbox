using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

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

            if (serviceRegistration.Options != null && !serviceRegistration.IsInstance())
            {
                if (serviceRegistration.Options.TryGetValue(OptionIds.AsyncInitializer, out var asyncInitializer))
                    expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithAsyncInitializerMethod, expression,
                        asyncInitializer.AsConstant()).ConvertTo(requestedType);

                if (serviceRegistration.Options.TryGetValue(OptionIds.Finalizer, out var finalizer))
                    expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithFinalizerMethod, expression,
                        finalizer.AsConstant()).ConvertTo(requestedType);
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

            var options = serviceRegistration.Options?.GetOrDefault(OptionIds.RegistrationTypeOptions);

            return options switch
            {
                FactoryOptions factoryOptions => GetExpressionForFactory(serviceRegistration, factoryOptions, resolutionContext, requestedType),
                InstanceOptions instanceRegistration => instanceRegistration.IsWireUp
                    ? ExpressionFactory.ConstructBuildUpExpression(serviceRegistration,
                        resolutionContext, instanceRegistration.ExistingInstance.AsConstant(),
                        serviceRegistration.ImplementationType)
                    : instanceRegistration.ExistingInstance.AsConstant(),
                Delegate func => GetExpressionForFunc(serviceRegistration, func, resolutionContext),
                _ => GetExpressionForDefault(serviceRegistration, resolutionContext)
            };
        }

        private static bool ShouldHandleDisposal(IContainerContext containerContext, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration.Options.IsOn(OptionIds.IsLifetimeExternallyOwned) ||
                serviceRegistration.IsInstance())
                return false;

            return containerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                   serviceRegistration.Lifetime is not TransientLifetime;
        }
    }
}
