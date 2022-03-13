using System;
using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;

namespace Stashbox.Registration
{
    /// <summary>
    /// Describes an open-generic service registration.
    /// </summary>
    public class OpenGenericRegistration : ServiceRegistration
    {
        private ImmutableTree<Type, ServiceRegistration> closedGenericRegistrations = ImmutableTree<Type, ServiceRegistration>.Empty;

        internal OpenGenericRegistration(Type implementationType,
            RegistrationContext registrationContext,
            ContainerConfiguration containerConfiguration,
            bool isDecorator)
            : base(implementationType,
                  registrationContext,
                  containerConfiguration,
                  isDecorator)
        { }

        internal OpenGenericRegistration(Type implementationType,
            ContainerConfiguration containerConfiguration,
            object? name,
            LifetimeDescriptor? lifetime,
            bool isDecorator)
            : base(implementationType,
                  containerConfiguration,
                  isDecorator,
                  name, 
                  lifetime)
        { }

        internal ServiceRegistration ProduceClosedRegistration(Type requestedType, ResolutionContext resolutionContext)
        {
            var found = this.closedGenericRegistrations.GetOrDefaultByRef(requestedType);
            if (found != null) return found;

            var genericType = this.ImplementationType.MakeGenericType(requestedType.GetGenericArguments());
            var newRegistration = new ServiceRegistration(genericType, null,
                    resolutionContext.CurrentContainerContext.ContainerConfiguration, this);

            return Swap.SwapValue(ref this.closedGenericRegistrations, (t1, t2, _, _, items) =>
                items.AddOrUpdate(t1, t2, true), requestedType, newRegistration, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder)
                    ? newRegistration
                    : this.closedGenericRegistrations.GetOrDefaultByRef(requestedType)!;
        }
    }
}
