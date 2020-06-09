using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a singleton lifetime manager.
    /// </summary>
    public class SingletonLifetime : FactoryLifetimeDescriptor
    {
        /// <inheritdoc />
        protected override int LifeSpan { get; } = 20;

        /// <inheritdoc />
        protected override Expression ApplyLifetime(Func<IResolutionScope, object> factory,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            resolutionContext.CurrentContainerContext.RootScope
                    .GetOrAddScopedObject(serviceRegistration.RegistrationId, serviceRegistration.SynchronizationObject, factory)
                    .AsConstant();
    }
}
