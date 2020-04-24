using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a transient lifetime.
    /// </summary>
    public class TransientLifetime : LifetimeDescriptor
    {
        /// <inheritdoc />
        protected override Expression GetLifetimeAppliedExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType) =>
            base.BuildExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
    }
}
