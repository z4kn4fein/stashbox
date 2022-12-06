using Stashbox.Registration;
using Stashbox.Resolution;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    internal class EmptyLifetime : LifetimeDescriptor
    {
        private protected override Expression? BuildLifetimeAppliedExpression(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, TypeInformation typeInformation)
            => null;
    }
}
