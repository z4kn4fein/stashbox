using Stashbox.Utils.Data.Immutable;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Describes an open-generic service registration.
    /// </summary>
    public class OpenGenericRegistration : ServiceRegistration
    {
        private ImmutableTree<Type, ServiceRegistration> closedGenericRegistrations = ImmutableTree<Type, ServiceRegistration>.Empty;
        internal OpenGenericRegistration(ServiceRegistration serviceRegistration)
            : base(serviceRegistration.ImplementationType, serviceRegistration.Name, serviceRegistration.Lifetime, serviceRegistration.IsDecorator,
                  serviceRegistration.Options, serviceRegistration.RegistrationId, serviceRegistration.RegistrationOrder)
        { }

        internal ServiceRegistration ProduceClosedRegistration(Type requestedType)
        {
            var found = closedGenericRegistrations.GetOrDefaultByRef(requestedType);
            if (found != null) return found;
            var genericType = ImplementationType.MakeGenericType(requestedType.GetGenericArguments());
            var newRegistration = new ServiceRegistration(genericType, null, Lifetime, IsDecorator);
            return Swap.SwapValue(ref closedGenericRegistrations, (t1, t2, _, _, items) =>
                items.AddOrUpdate(t1, t2, true), requestedType, newRegistration, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder)
                    ? newRegistration
                    : closedGenericRegistrations.GetOrDefaultByRef(requestedType)!;
        }
    }
}
