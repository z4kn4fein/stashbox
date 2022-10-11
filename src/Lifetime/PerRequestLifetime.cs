using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a per-request lifetime.
    /// </summary>
    public class PerRequestLifetime : FactoryLifetimeDescriptor
    {
        /// <inheritdoc />
        private protected override bool StoreResultInLocalVariable => true;

        /// <inheritdoc />
        protected override Expression ApplyLifetime(Func<IResolutionScope, IRequestContext, object> factory,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            resolutionContext.RequestConfiguration.RequiresRequestContext = true;

            return resolutionContext.RequestContextParameter
                .ConvertTo(Constants.InternalRequestContextType)
                .CallMethod(Constants.GetOrAddInstanceMethod,
                    serviceRegistration.RegistrationId.AsConstant(),
                    factory.AsConstant(),
                    resolutionContext.CurrentScopeParameter)
                .ConvertTo(resolveType);
        }
    }
}
