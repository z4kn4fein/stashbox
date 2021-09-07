using Stashbox.Utils;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Multitenant
{
    /// <summary>
    /// Represents a tenant distributor that manages tenants in a multitenant environment.
    /// </summary>
    public sealed class TenantDistributor : ITenantDistributor
    {
        private int disposed;
        private ImmutableTree<object, IStashboxContainer> tenantRepository = ImmutableTree<object, IStashboxContainer>.Empty;

        /// <inheritdoc />
        public IStashboxContainer RootContainer { get; }

        /// <summary>
        /// Constructs a <see cref="TenantDistributor"/>.
        /// </summary>
        /// <param name="rootContainer">A pre-configured root container, used to create child tenant containers. If not set, a new will be created.</param>
        public TenantDistributor(IStashboxContainer rootContainer = null)
        {
            this.RootContainer = rootContainer ?? new StashboxContainer();
        }

        /// <inheritdoc />
        public void ConfigureTenant(object tenantId, Func<IStashboxContainer, IDisposable> tenantConfig)
        {
            Shield.EnsureNotNull(tenantId, nameof(tenantId));
            Shield.EnsureNotNull(tenantConfig, nameof(tenantConfig));

            var tenantContainer = this.RootContainer.CreateChildContainer();

            if(Swap.SwapValue(ref this.tenantRepository,
                (id, container, t3, t4, repo) => repo.AddOrUpdate(id, container, false, false),
                tenantId,
                tenantContainer,
                Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder))
            { 
                var disposable = tenantConfig(tenantContainer);
                this.RootContainer.ContainerContext.RootScope.AddDisposableTracking(disposable);
            }
        }

        /// <inheritdoc />
        public IDependencyResolver GetTenant(object tenantId)
        {
            Shield.EnsureNotNull(tenantId, nameof(tenantId));
            return this.tenantRepository.GetOrDefaultByValue(tenantId);
        }

        /// <inheritdoc />
        public void Validate()
        {
            var exceptions = new ExpandableArray<Exception>();

            try
            {
                this.RootContainer.Validate();
            }
            catch (AggregateException ex)
            {
                exceptions.Add(new AggregateException($"Root container validation failed. See the inner exceptions for details.", ex.InnerExceptions));
            }

            foreach (var tenant in this.tenantRepository.Walk())
            {
                try
                {
                    tenant.Value.Validate();
                }
                catch (AggregateException ex)
                {
                    exceptions.Add(new AggregateException($"Tenant validation failed for '{tenant.Key}'. See the inner exceptions for details.", ex.InnerExceptions));
                }
            }

            if (exceptions.Length > 0)
                throw new AggregateException("Tenant distributor validation failed. See the inner exceptions for details.", exceptions);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

            this.RootContainer.Dispose();
        }

#if HAS_ASYNC_DISPOSABLE
        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return new ValueTask(Task.CompletedTask);

            return this.RootContainer.DisposeAsync(); 
        }
#endif
    }
}
