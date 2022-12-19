using Stashbox.Multitenant;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{
    public class MultitenantTests
    {
        [Fact]
        public void MultitenantTests_Same_Root()
        {
            var container = new StashboxContainer();
            var md = new TenantDistributor(container);

            Assert.Same(container, md.RootContainer);
        }

        [Fact]
        public void MultitenantTests_Null_Container()
        {
            var md = new TenantDistributor(null);

            Assert.NotNull(md.RootContainer);
        }

        [Fact]
        public void MultitenantTests_Get_NonExisting_Null()
        {
            var container = new StashboxContainer();
            var md = new TenantDistributor(container);

            Assert.Null(md.GetTenant("A"));
        }

        [Fact]
        public void MultitenantTests_Configure()
        {
            var container = new StashboxContainer();
            container.Register<IA, A>();

            var md = new TenantDistributor(container);

            md.ConfigureTenant("A", c => c.Register<IA, B>());

            Assert.IsType<A>(container.Resolve<IA>());

            var tenant = md.GetTenant("A");

            Assert.NotNull(tenant);
            Assert.IsType<B>(md.GetTenant("A").Resolve<IA>());
        }

        [Fact]
        public void MultitenantTests_Configure_Dep()
        {
            var container = new StashboxContainer();
            container.Register<D>();
            container.Register<IA, A>();

            var md = new TenantDistributor(container);

            md.ConfigureTenant("A", c => c.Register<IA, B>());

            var tenant = md.GetTenant("A");

            Assert.IsType<A>(container.Resolve<D>().Ia);
            Assert.IsType<B>(md.GetTenant("A").Resolve<D>().Ia);
        }

        [Fact]
        public void MultitenantTests_Configure_Validate_Root_Throws()
        {
            var container = new StashboxContainer();
            container.Register<D>();

            var md = new TenantDistributor(container);

            md.ConfigureTenant("A", c => c.Register<IA, B>());

            var exception = Assert.Throws<AggregateException>(() => md.Validate());
            Assert.Single(exception.InnerExceptions);
        }

        [Fact]
        public void MultitenantTests_Configure_Validate_Root_And_Tenants_Throws()
        {
            var container = new StashboxContainer();
            container.Register<D>();

            var md = new TenantDistributor(container);

            md.ConfigureTenant("A", c => c.Register<D>());

            var exception = Assert.Throws<AggregateException>(() => md.Validate());
            Assert.Equal(2, exception.InnerExceptions.Count);
        }

        [Fact]
        public void MultitenantTests_Configure_Validate_Valid()
        {
            var container = new StashboxContainer();
            container.Register<IA, A>();

            var md = new TenantDistributor(container);

            md.ConfigureTenant("A", c => c.Register<D>());

            md.Validate();
        }

        [Fact]
        public void MultitenantTests_Dispose()
        {
            var container = new StashboxContainer(c => c.WithDisposableTransientTracking());
            container.Register<IA, C>();

            var md = new TenantDistributor(container);

            md.ConfigureTenant("C", c => { });
            var tenant = md.GetTenant("C");

            var inst = (C)tenant.Resolve<IA>();

            md.Dispose();

            Assert.True(inst.Disposed);
            Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
            Assert.Throws<ObjectDisposedException>(() => tenant.Resolve<IA>());
        }

        [Fact]
        public void MultitenantTests_Dispose_Tenant()
        {
            var container = new StashboxContainer(c => c.WithDisposableTransientTracking());

            var md = new TenantDistributor(container);

            md.ConfigureTenant("C", c => c.Register<IA, C>());
            var tenant = md.GetTenant("C");

            var inst = (C)tenant.Resolve<IA>();

            md.Dispose();

            Assert.True(inst.Disposed);

            Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
            Assert.Throws<ObjectDisposedException>(() => tenant.Resolve<IA>());
        }

        [Fact]
        public void MultitenantTests_Dispose_Multiple()
        {
            var container = new StashboxContainer();
            container.Register<IA, C>();

            var md = new TenantDistributor(container);

            md.Dispose();
            md.Dispose();
        }

#if HAS_ASYNC_DISPOSABLE
        [Fact]
        public async Task MultitenantTests_Dispose_Async()
        {
            var container = new StashboxContainer(c => c.WithDisposableTransientTracking());
            container.Register<IA, C>();

            var md = new TenantDistributor(container);

            md.ConfigureTenant("C", c => { });
            var tenant = md.GetTenant("C");

            var inst = (C)tenant.Resolve<IA>();

            await md.DisposeAsync();

            Assert.True(inst.Disposed);
            Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
            Assert.Throws<ObjectDisposedException>(() => tenant.Resolve<IA>());
        }

        [Fact]
        public async Task MultitenantTests_Dispose_Async_Tenant()
        {
            var container = new StashboxContainer(c => c.WithDisposableTransientTracking());

            var md = new TenantDistributor(container);

            md.ConfigureTenant("C", c => c.Register<IA, C>());
            var tenant = md.GetTenant("C");

            var inst = (C)tenant.Resolve<IA>();

            await md.DisposeAsync();

            Assert.True(inst.Disposed);
            Assert.Throws<ObjectDisposedException>(() => container.Resolve<IA>());
            Assert.Throws<ObjectDisposedException>(() => tenant.Resolve<IA>());
        }

        [Fact]
        public async Task MultitenantTests_Dispose_Async_Multiple()
        {
            var container = new StashboxContainer();
            container.Register<IA, C>();

            var md = new TenantDistributor(container);

            await md.DisposeAsync();
            await md.DisposeAsync();
        }
#endif

        interface IA { }

        class A : IA { }

        class B : IA { }

        class C : IA, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(C));

                this.Disposed = true;
            }
        }

        class D
        {
            public D(IA ia)
            {
                Ia = ia;
            }

            public IA Ia { get; }
        }
    }
}
