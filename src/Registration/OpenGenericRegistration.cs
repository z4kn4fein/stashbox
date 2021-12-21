using Stashbox.Configuration;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;

namespace Stashbox.Registration
{
    internal class OpenGenericRegistration : ServiceRegistration
    {
        private ImmutableTree<Type, ServiceRegistration> closedGenericRegistrations = ImmutableTree<Type, ServiceRegistration>.Empty;

        internal OpenGenericRegistration(Type implementationType,
            ContainerConfiguration containerConfiguration,
            RegistrationContext registrationContext,
            bool isDecorator)
            : base(implementationType,
                  RegistrationType.OpenGeneric,
                  containerConfiguration,
                  registrationContext,
                  isDecorator)
        { }

        public ServiceRegistration ProduceClosedRegistration(Type requestedType)
        {
            var found = this.closedGenericRegistrations.GetOrDefaultByRef(requestedType);
            if (found != null) return found;

            var genericType = this.ImplementationType.MakeGenericType(requestedType.GetGenericArguments());
            var newRegistration = new ServiceRegistration(genericType, RegistrationType.Default,
                    base.Configuration, base.RegistrationContext, base.IsDecorator);
            newRegistration.RegistrationContext.Name = null;

            return Swap.SwapValue(ref this.closedGenericRegistrations, (t1, t2, _, _, items) =>
                items.AddOrUpdate(t1, t2, true), requestedType, newRegistration, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder) 
                    ? newRegistration 
                    : this.closedGenericRegistrations.GetOrDefaultByRef(requestedType);
        }
    }
}
