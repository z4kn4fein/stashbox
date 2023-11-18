using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime;

/// <summary>
/// Represents a singleton lifetime.
/// </summary>
public class SingletonLifetime : FactoryLifetimeDescriptor
{
    /// <inheritdoc />
    protected internal override int LifeSpan => 20;

    /// <inheritdoc />
    internal override bool StoreResultInLocalVariable => true;

    /// <inheritdoc />
    protected override Expression ApplyLifetime(Func<IResolutionScope, IRequestContext, object> factory,
        ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var rootScope = resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.ReBuildSingletonsInChildContainerEnabled
            ? resolutionContext.RequestInitiatorContainerContext.RootScope
            : resolutionContext.CurrentContainerContext.RootScope;

        // do not build singletons during validation, we just have to ensure the expression tree is valid
        if (resolutionContext.IsValidationContext)
            return rootScope.AsConstant().CallMethod(Constants.GetOrAddScopedObjectMethod,
                    serviceRegistration.GetDiscriminator(typeInformation, 
                        resolutionContext.CurrentContainerContext.ContainerConfiguration).AsConstant(),
                    factory.AsConstant(),
                    resolutionContext.RequestContextParameter,
                    serviceRegistration.ImplementationType.AsConstant());

        return rootScope.GetOrAddScopedObject(serviceRegistration.GetDiscriminator(typeInformation, 
                resolutionContext.CurrentContainerContext.ContainerConfiguration), factory, 
            resolutionContext.RequestContext, serviceRegistration.ImplementationType).AsConstant();
    }
}