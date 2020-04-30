using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a scoped lifetime.
    /// </summary>
    public class ScopedLifetime : LifetimeDescriptor
    {
        /// <inheritdoc />
        protected override bool ShouldStoreResultInLocalVariable => true;

        /// <inheritdoc />
        protected override Expression GetLifetimeAppliedExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, resolutionContext, resolveType);
            if (factory == null)
                return null;

            return resolutionContext.CurrentScopeParameter
                .CallMethod(Constants.GetOrAddScopedObjectMethod, serviceRegistration.RegistrationId.AsConstant(),
                    serviceRegistration.RegistrationName.AsConstant(Constants.ObjectType), factory.AsConstant())
                .ConvertTo(resolveType);
        }
    }
}
