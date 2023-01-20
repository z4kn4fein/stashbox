using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Stashbox.Expressions;

internal static partial class ExpressionBuilder
{
    internal static Expression? BuildExpressionForRegistration(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var expression = BuildExpressionByRegistrationType(serviceRegistration, resolutionContext, typeInformation);
        if (expression == null)
            return null;

        if (serviceRegistration.Options != null && !serviceRegistration.IsInstance())
        {
            if (serviceRegistration.Options.TryGetValue(RegistrationOption.AsyncInitializer, out var asyncInitializer))
                expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithAsyncInitializerMethod, expression,
                    asyncInitializer.AsConstant()).ConvertTo(typeInformation.Type);

            if (serviceRegistration.Options.TryGetValue(RegistrationOption.Finalizer, out var finalizer))
                expression = resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddWithFinalizerMethod, expression,
                    finalizer.AsConstant()).ConvertTo(typeInformation.Type);
        }

        if (!ShouldHandleDisposal(resolutionContext.CurrentContainerContext, serviceRegistration) || !expression.Type.IsDisposable())
            return expression;

        return resolutionContext.RequestConfiguration.RequiresRequestContext
            ? resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddRequestContextAwareDisposalMethod,
                expression, resolutionContext.RequestContextParameter).ConvertTo(typeInformation.Type)
            : resolutionContext.CurrentScopeParameter.CallMethod(Constants.AddDisposalMethod, expression).ConvertTo(typeInformation.Type);
    }

    private static Expression? BuildExpressionByRegistrationType(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        resolutionContext = resolutionContext.ShouldFallBackToRequestInitiatorContext
            ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
            : resolutionContext;

        var options = serviceRegistration.Options?.GetOrDefault(RegistrationOption.RegistrationTypeOptions);

        return options switch
        {
            FactoryOptions factoryOptions => GetExpressionForFactory(serviceRegistration, factoryOptions, resolutionContext, typeInformation),
            InstanceOptions instanceRegistration => instanceRegistration.IsWireUp
                ? ExpressionFactory.ConstructBuildUpExpression(serviceRegistration,
                    resolutionContext, instanceRegistration.ExistingInstance.AsConstant(),
                    typeInformation)
                : instanceRegistration.ExistingInstance.AsConstant(),
            Delegate func => GetExpressionForFunc(serviceRegistration, func, resolutionContext),
            _ => GetExpressionForDefault(serviceRegistration, resolutionContext, typeInformation)
        };
    }

    private static bool ShouldHandleDisposal(IContainerContext containerContext, ServiceRegistration serviceRegistration)
    {
        if (serviceRegistration.Options.IsOn(RegistrationOption.IsLifetimeExternallyOwned) ||
            serviceRegistration.IsInstance())
            return false;

        return containerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
               serviceRegistration.Lifetime is not TransientLifetime;
    }
}