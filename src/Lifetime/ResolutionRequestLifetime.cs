using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a per resolution request lifetime.
    /// </summary>
    public class ResolutionRequestLifetime : TransientLifetime
    {
        /// <inheritdoc />
        protected override bool StoreResultInLocalVariable => true;

        /// <inheritdoc />
        protected override Expression GetLifetimeAppliedExpression(IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType) =>
            base.BuildExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
    }
}
