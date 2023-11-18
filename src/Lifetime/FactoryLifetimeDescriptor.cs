using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime;

/// <summary>
/// Represents a lifetime descriptor which applies to factory delegates.
/// </summary>
public abstract class FactoryLifetimeDescriptor : LifetimeDescriptor
{
    private protected override Expression? BuildLifetimeAppliedExpression(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var factory = GetFactoryDelegateForRegistration(serviceRegistration, resolutionContext, typeInformation);
        return factory == null ? null : this.ApplyLifetime(factory, serviceRegistration, resolutionContext, typeInformation);
    }
    
    internal override Expression? ApplyLifetimeToExpression(Expression? expression,
        ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation) => expression == null
        ? null
        : this.ApplyLifetime(expression.CompileDelegate(resolutionContext, resolutionContext.CurrentContainerContext.ContainerConfiguration), 
            serviceRegistration, resolutionContext, typeInformation);

    private static Func<IResolutionScope, IRequestContext, object>? GetFactoryDelegateForRegistration(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        if (!IsRegistrationOutputCacheable(serviceRegistration, resolutionContext))
            return GetNewFactoryDelegate(serviceRegistration, resolutionContext.BeginSubGraph(), typeInformation);

        var factory = resolutionContext.FactoryCache.GetOrDefault(serviceRegistration.RegistrationId);
        if (factory != null)
            return factory;

        factory = GetNewFactoryDelegate(serviceRegistration, resolutionContext.BeginSubGraph(), typeInformation);
        if (factory == null)
            return null;

        resolutionContext.FactoryCache.Add(serviceRegistration.RegistrationId, factory);
        return factory;
    }

    private static Func<IResolutionScope, IRequestContext, object>? GetNewFactoryDelegate(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation) =>
        GetExpressionForRegistration(serviceRegistration, resolutionContext, typeInformation)
            ?.CompileDelegate(resolutionContext, resolutionContext.CurrentContainerContext.ContainerConfiguration);

    /// <summary>
    /// Derived types are using this method to apply their lifetime to the instance creation.
    /// </summary>
    /// <param name="factory">The factory which can be used to instantiate the service.</param>
    /// <param name="serviceRegistration">The service registration.</param>
    /// <param name="resolutionContext">The info about the actual resolution.</param>
    /// <param name="typeInformation">The type information of the resolved service.</param>
    /// <returns>The lifetime managed expression.</returns>
    protected abstract Expression ApplyLifetime(Func<IResolutionScope, IRequestContext, object> factory, ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation);
}