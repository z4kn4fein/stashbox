using Stashbox.Utils.Data.Immutable;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration.ServiceRegistrations
{
    /// <summary>
    /// Describes an open-generic service registration.
    /// </summary>
    public class OpenGenericRegistration : ComplexRegistration
    {
        private readonly ServiceRegistration baseRegistration;
        private ImmutableTree<Type, ServiceRegistration> closedGenericRegistrations = ImmutableTree<Type, ServiceRegistration>.Empty;
        internal OpenGenericRegistration(ServiceRegistration baseRegistration)
            : base(baseRegistration)
        {
            this.baseRegistration = baseRegistration;
        }

        internal ServiceRegistration ProduceClosedRegistration(Type requestedType)
        {
            var found = this.closedGenericRegistrations.GetOrDefaultByRef(requestedType);
            if (found != null) return found;
            var genericType = this.ImplementationType.MakeGenericType(requestedType.GetGenericArguments());
            var newRegistration = RegistrationFactory.FromOpenGeneric(genericType, this.baseRegistration);
            return Swap.SwapValue(ref this.closedGenericRegistrations, (t1, t2, _, _, items) =>
                items.AddOrUpdate(t1, t2, true), requestedType, newRegistration, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder)
                    ? newRegistration
                    : this.closedGenericRegistrations.GetOrDefaultByRef(requestedType)!;
        }
    }
}
