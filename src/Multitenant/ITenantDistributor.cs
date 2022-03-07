using System;

namespace Stashbox.Multitenant
{
    /// <summary>
    /// Represents a tenant distributor that manages tenants in a multi-tenant environment.
    /// </summary>
    public interface ITenantDistributor :
#if HAS_ASYNC_DISPOSABLE
        IAsyncDisposable,
#endif
        IDisposable

    {
        /// <summary>
        /// The root container.
        /// </summary>
        IStashboxContainer RootContainer { get; }

        /// <summary>
        /// Adds a tenant with a specified service configuration to the distributor.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <param name="tenantConfig">The service configuration of the tenant.</param>
        void ConfigureTenant(object tenantId, Func<IStashboxContainer, IDisposable> tenantConfig);

        /// <summary>
        /// Gets a pre-configured <see cref="IDependencyResolver"/> from the distributor which represents a tenant identified by the given id.
        /// When the requested tenant doesn't exist a null value will be returned.
        /// </summary>
        /// <param name="tenantId">The identifier of the tenant.</param>
        /// <returns>The pre-configured tenant container if it's exist, otherwise null.</returns>
        IDependencyResolver? GetTenant(object tenantId);

        /// <summary>
        /// Validates the root container and the configured tenants.
        /// </summary>
        void Validate();
    }
}
