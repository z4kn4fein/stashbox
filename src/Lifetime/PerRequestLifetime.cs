using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime;

/// <summary>
/// Represents a per-request lifetime.
/// </summary>
public class PerRequestLifetime : FactoryLifetimeDescriptor
{
    /// <inheritdoc />
    internal override bool StoreResultInLocalVariable => true;

    /// <inheritdoc />
    protected override Expression ApplyLifetime(Func<IResolutionScope, IRequestContext, object> factory,
        ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        resolutionContext.RequestConfiguration.RequiresRequestContext = true;

        return resolutionContext.RequestContextParameter
            .ConvertTo(TypeCache<IInternalRequestContext>.Type)
            .CallMethod(Constants.GetOrAddInstanceMethod,
                serviceRegistration.GetDiscriminator(typeInformation, 
                    resolutionContext.CurrentContainerContext.ContainerConfiguration).AsConstant(),
                factory.AsConstant(),
                resolutionContext.CurrentScopeParameter);
    }
}