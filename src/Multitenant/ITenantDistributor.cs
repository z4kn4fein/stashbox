﻿using System;

namespace Stashbox.Multitenant;

/// <summary>
/// Represents a tenant distributor that manages tenants in a multi-tenant environment.
/// </summary>
public interface ITenantDistributor : IStashboxContainer,
#if HAS_ASYNC_DISPOSABLE
        IAsyncDisposable,
#endif
    IDisposable

{
    /// <summary>
    /// Adds a tenant with a specified service configuration to the distributor.
    /// </summary>
    /// <param name="tenantId">The identifier of the tenant.</param>
    /// <param name="tenantConfig">The service configuration of the tenant.</param>
    /// <param name="attachTenantToParent">If true, the new tenant will be attached to the lifecycle of the root container. When the root is being disposed, the tenant will be disposed with it.</param>
    void ConfigureTenant(object tenantId, Action<IStashboxContainer> tenantConfig, bool attachTenantToParent = true);

    /// <summary>
    /// Gets a pre-configured <see cref="IDependencyResolver"/> from the distributor which represents a tenant identified by the given id.
    /// When the requested tenant doesn't exist a null value will be returned.
    /// </summary>
    /// <param name="tenantId">The identifier of the tenant.</param>
    /// <returns>The pre-configured tenant container if it's exist, otherwise null.</returns>
    IDependencyResolver? GetTenant(object tenantId);
}