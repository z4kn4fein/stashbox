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
    protected override Expression ApplyLifetime(Func<IResolutionScope, IRequestContext, object> factory,
        ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var rootScope = resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.ReBuildSingletonsInChildContainerEnabled
            ? resolutionContext.RequestInitiatorContainerContext.RootScope
            : resolutionContext.CurrentContainerContext.RootScope;

        return rootScope.AsConstant().CallMethod(Constants.GetOrAddScopedObjectMethod,
            serviceRegistration.GetDiscriminator(typeInformation, 
                resolutionContext.CurrentContainerContext.ContainerConfiguration).AsConstant(),
            factory.AsConstant(),
            resolutionContext.RequestContextParameter,
            serviceRegistration.ImplementationType.AsConstant());
    }
}