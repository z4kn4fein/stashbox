using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    internal class EmptyLifetime : LifetimeDescriptor
    {
        private protected override Expression? BuildLifetimeAppliedExpression(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType)
            => null;
    }
}
