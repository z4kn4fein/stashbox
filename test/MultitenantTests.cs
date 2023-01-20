using Stashbox.Multitenant;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Stashbox.Configuration;
using Stashbox.Registration.Fluent;
using Stashbox.Resolution;
using Xunit;

namespace Stashbox.Tests;

public class MultitenantTests
{
    [Fact]
    public void MultitenantTests_Same_Root()
    {
        var container = new StashboxContainer();
        ITenantDistributor md = new TenantDistributor(container);

        Assert.Same(container, md.GetType().GetField("rootContainer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(md));
    }

    [Fact]
    public void MultitenantTests_Null_Container()
    {
        ITenantDistributor md = new TenantDistributor(null);

        Assert.NotNull(md.GetType().GetField("rootContainer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(md));
    }

    [Fact]
    public void MultitenantTests_Get_NonExisting_Null()
    {
        var container = new StashboxContainer();
        ITenantDistributor md = new TenantDistributor(container);

        Assert.Null(md.GetTenant("A"));
    }

    [Fact]
    public void MultitenantTests_Configure()
    {
        var container = new StashboxContainer();
        container.Register<IA, A>();

        ITenantDistributor md = new TenantDistributor(container);

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

        ITenantDistributor md = new TenantDistributor(container);

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

        ITenantDistributor md = new TenantDistributor(container);

        md.ConfigureTenant("A", c => c.Register<IA, B>());

        var exception = Assert.Throws<AggregateException>(() => md.Validate());
        Assert.Single(exception.InnerExceptions);
    }

    [Fact]
    public void MultitenantTests_Configure_Validate_Root_And_Tenants_Throws()
    {
        var container = new StashboxContainer();
        container.Register<D>();

        ITenantDistributor md = new TenantDistributor(container);

        md.ConfigureTenant("A", c => c.Register<D>());

        var exception = Assert.Throws<AggregateException>(() => md.Validate());
        Assert.Equal(2, exception.InnerExceptions.Count);
    }

    [Fact]
    public void MultitenantTests_Configure_Validate_Valid()
    {
        var container = new StashboxContainer();
        container.Register<IA, A>();

        ITenantDistributor md = new TenantDistributor(container);

        md.ConfigureTenant("A", c => c.Register<D>());

        md.Validate();
    }

    [Fact]
    public void MultitenantTests_Dispose()
    {
        var container = new StashboxContainer(c => c.WithDisposableTransientTracking());
        container.Register<IA, C>();

        ITenantDistributor md = new TenantDistributor(container);

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

        ITenantDistributor md = new TenantDistributor(container);

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

        ITenantDistributor md = new TenantDistributor(container);

        md.Dispose();
        md.Dispose();
    }

    [Fact]
    public void MultitenantTests_Ensure_Container_Method_Calls_Delegate_To_Root()
    {
        var container = new Mock<IStashboxContainer>();
        ITenantDistributor d = new TenantDistributor(container.Object);

        d.RegisterResolver(new Mock<IResolver>().Object);
        d.CreateChildContainer();
        d.IsRegistered<IA>();
        d.IsRegistered(typeof(IA));
        d.Configure(c => { });
        d.Validate();
        d.GetRegistrationMappings();
        d.GetRegistrationDiagnostics();
        d.Register<IA, C>(c => { });
        d.Register<IA, C>();
        d.Register<IA, C>("a");
        d.Register<IA>(typeof(C));
        d.Register(typeof(IA), typeof(C));
        d.Register<C>(c => { });
        d.Register<C>("a");
        d.Register(typeof(C));
        d.RegisterSingleton<IA, C>();
        d.RegisterSingleton(typeof(IA), typeof(C));
        d.RegisterSingleton<C>("a");
        d.RegisterScoped<IA, C>();
        d.RegisterScoped(typeof(IA), typeof(C));
        d.RegisterScoped<C>("a");
        d.RegisterInstance<IA>(new C());
        d.RegisterInstance((object)new C(), typeof(IA));
        d.WireUp<IA>(new C());
        d.WireUp((object)new C(), typeof(IA));
        d.GetService(typeof(IA));
        d.Resolve(typeof(IA));
        d.Resolve(typeof(IA), new object[] { "a" });
        d.Resolve(typeof(IA), "a", new object[] { "a" });
        d.Resolve(typeof(IA), "a");
        d.ResolveOrDefault(typeof(IA));
        d.ResolveOrDefault(typeof(IA), new object[] { "a" });
        d.ResolveOrDefault(typeof(IA), "a", new object[] { "a" });
        d.ResolveOrDefault(typeof(IA), "a");
        d.ResolveAll<IA>();
        d.ResolveAll<IA>("a");
        d.ResolveAll<IA>(new object[] { "a" });
        d.ResolveAll<IA>("a", new object[] { "a" });
        d.ResolveAll(typeof(IA));
        d.ResolveAll(typeof(IA),"a");
        d.ResolveAll(typeof(IA),new object[] { "a" });
        d.ResolveAll(typeof(IA),"a", new object[] { "a" });
        d.ResolveFactory(typeof(IA));
        d.ResolveFactoryOrDefault(typeof(IA));
        d.BeginScope();
        d.PutInstanceInScope(typeof(IA), new C());
        d.BuildUp<IA>(new C());
        d.Activate(typeof(C));
        d.InvokeAsyncInitializers();
        d.CanResolve<IA>();
        d.CanResolve(typeof(IA));
        d.GetDelegateCacheEntries();
        d.ReMap<IA, C>();
        d.ReMap<IA>(typeof(C));
        d.ReMap(typeof(IA), typeof(C));
        d.ReMap<C>();
        d.ReMapDecorator<IA, C>();
        d.ReMapDecorator<IA>(typeof(C));
        d.ReMapDecorator(typeof(IA), typeof(C));
        d.RegisterTypesAs(typeof(IA), new []{typeof(C)});
        d.RegisterTypes(new []{typeof(C)});
        d.ComposeBy(new Mock<ICompositionRoot>().Object.GetType());
        d.ComposeBy(new Mock<ICompositionRoot>().Object);
        d.RegisterDecorator<IA, C>();
        d.RegisterDecorator(typeof(IA), typeof(C));
        d.RegisterDecorator<C>();
        d.RegisterDecorator<IA>(typeof(C));
        d.RegisterDecorator(typeof(C));
        d.RegisterFunc<IA>((r) => new C());
        d.RegisterFunc<int, IA>((i, resolver) => new C());
        d.RegisterFunc<int, int, IA>((i, i1, arg3) => new C());
        d.RegisterFunc<int, int, int, IA>((i, i1, arg3, arg4) => new C());
        d.RegisterFunc<int, int, int, int, IA>((i, i1, arg3, arg4, arg5) => new C());
            
        container.Verify(c => c.RegisterResolver(It.IsAny<IResolver>()), Times.Once);
        container.Verify(c => c.CreateChildContainer(null), Times.Once);
        container.Verify(c => c.IsRegistered<IA>(null), Times.Once);
        container.Verify(c => c.IsRegistered(typeof(IA), null), Times.Once);
        container.Verify(c => c.Configure(It.IsAny<Action<ContainerConfigurator>>()), Times.Once);
        container.Verify(c => c.Validate(), Times.Once);
        container.Verify(c => c.GetRegistrationMappings(), Times.Once);
        container.Verify(c => c.GetRegistrationDiagnostics(), Times.Once);
        container.Verify(c => c.Register<IA, C>(It.IsAny<RegistrationConfigurator<IA, C>>()), Times.Once);
        container.Verify(c => c.Register<IA, C>("a"), Times.Once);
        container.Verify(c => c.Register<IA>(typeof(C), null), Times.Once);
        container.Verify(c => c.Register(typeof(IA), typeof(C), null), Times.Once);
        container.Verify(c => c.Register<C>(It.IsAny<Action<RegistrationConfigurator<C, C>>>()), Times.Once);
        container.Verify(c => c.Register<C>("a"), Times.Once);
        container.Verify(c => c.Register(typeof(C), null), Times.Once);
        container.Verify(c => c.RegisterSingleton<IA, C>(null), Times.Once);
        container.Verify(c => c.RegisterSingleton(typeof(IA), typeof(C), null), Times.Once);
        container.Verify(c => c.RegisterSingleton<C>("a"), Times.Once);
        container.Verify(c => c.RegisterScoped<IA, C>(null), Times.Once);
        container.Verify(c => c.RegisterScoped(typeof(IA), typeof(C), null), Times.Once);
        container.Verify(c => c.RegisterScoped<C>("a"), Times.Once);
        container.Verify(c => c.RegisterInstance<IA>(It.IsAny<C>(), null, false, null), Times.Once);
        container.Verify(c => c.RegisterInstance(It.IsAny<object>(), It.IsAny<Type>(), null, false), Times.Once);
        container.Verify(c => c.WireUp<IA>(It.IsAny<C>(), null, false, null), Times.Once);
        container.Verify(c => c.WireUp(It.IsAny<object>(), It.IsAny<Type>(), null, false), Times.Once);
        container.Verify(c => c.GetService(typeof(IA)), Times.Once);
        container.Verify(c => c.Resolve(typeof(IA)), Times.Once);
        container.Verify(c => c.Resolve(typeof(IA), new object[] { "a" }), Times.Once);
        container.Verify(c => c.Resolve(typeof(IA), "a", new object[] { "a" }), Times.Once);
        container.Verify(c => c.Resolve(typeof(IA), "a"), Times.Once);
        container.Verify(c => c.ResolveOrDefault(typeof(IA)), Times.Once);
        container.Verify(c => c.ResolveOrDefault(typeof(IA), new object[] { "a" }), Times.Once);
        container.Verify(c => c.ResolveOrDefault(typeof(IA), "a", new object[] { "a" }), Times.Once);
        container.Verify(c => c.ResolveOrDefault(typeof(IA), "a"), Times.Once);
        container.Verify(c => c.ResolveAll<IA>(), Times.Once);
        container.Verify(c => c.ResolveAll<IA>("a"), Times.Once);
        container.Verify(c => c.ResolveAll<IA>(new object[] { "a" }), Times.Once);
        container.Verify(c => c.ResolveAll<IA>("a", new object[] { "a" }), Times.Once);
        container.Verify(c => c.ResolveAll(typeof(IA)), Times.Once);
        container.Verify(c => c.ResolveAll(typeof(IA),"a"), Times.Once);
        container.Verify(c => c.ResolveAll(typeof(IA),new object[] { "a" }), Times.Once);
        container.Verify(c => c.ResolveAll(typeof(IA),"a", new object[] { "a" }), Times.Once);
        container.Verify(c => c.ResolveFactory(typeof(IA), null, It.IsAny<Type[]>()), Times.Once);
        container.Verify(c => c.ResolveFactoryOrDefault(typeof(IA), null, It.IsAny<Type[]>()), Times.Once);
        container.Verify(c => c.BeginScope(null, false), Times.Once);
        container.Verify(c => c.PutInstanceInScope(typeof(IA), It.IsAny<C>(), false, null), Times.Once);
        container.Verify(c => c.BuildUp<IA>(It.IsAny<C>()), Times.Once);
        container.Verify(c => c.Activate(typeof(C)), Times.Once);
        container.Verify(c => c.InvokeAsyncInitializers(default), Times.Once);
        container.Verify(c => c.CanResolve<IA>(null), Times.Once);
        container.Verify(c => c.CanResolve(typeof(IA), null), Times.Once);
        container.Verify(c => c.GetDelegateCacheEntries(), Times.Once);
        container.Verify(c => c.ReMap<IA, C>(null), Times.Once);
        container.Verify(c => c.ReMap<IA>(typeof(C), null), Times.Once);
        container.Verify(c => c.ReMap(typeof(IA), typeof(C), null), Times.Once);
        container.Verify(c => c.ReMap<C>(null), Times.Once);
        container.Verify(c => c.ReMapDecorator<IA, C>(null), Times.Once);
        container.Verify(c => c.ReMapDecorator<IA>(typeof(C), null), Times.Once);
        container.Verify(c => c.ReMapDecorator(typeof(IA), typeof(C), null), Times.Once);
        container.Verify(c => c.RegisterTypesAs(typeof(IA), It.IsAny<Type[]>(), null, null), Times.Once);
        container.Verify(c => c.RegisterTypes(It.IsAny<Type[]>(), null, null, true, null), Times.Once);
        container.Verify(c => c.ComposeBy(It.IsAny<Type>(), It.IsAny<object[]>()), Times.Once);
        container.Verify(c => c.ComposeBy(It.IsAny<Type>(), It.IsAny<object[]>()), Times.Once);
        container.Verify(c => c.RegisterDecorator<IA, C>(null), Times.Once);
        container.Verify(c => c.RegisterDecorator(typeof(IA), typeof(C), null), Times.Once);
        container.Verify(c => c.RegisterDecorator<C>(null), Times.Once);
        container.Verify(c => c.RegisterDecorator<IA>(typeof(C), null), Times.Once);
        container.Verify(c => c.RegisterDecorator(typeof(C), null), Times.Once);
        container.Verify(c => c.RegisterFunc(It.IsAny<Func<IDependencyResolver, IA>>(), null), Times.Once);
        container.Verify(c => c.RegisterFunc(It.IsAny<Func<int, IDependencyResolver, IA>>(), null), Times.Once);
        container.Verify(c => c.RegisterFunc(It.IsAny<Func<int, int, IDependencyResolver, IA>>(), null), Times.Once);
        container.Verify(c => c.RegisterFunc(It.IsAny<Func<int, int, int, IDependencyResolver, IA>>(), null), Times.Once);
        container.Verify(c => c.RegisterFunc(It.IsAny<Func<int, int, int, int, IDependencyResolver, IA>>(), null), Times.Once);
    }

#if HAS_ASYNC_DISPOSABLE
        [Fact]
        public async Task MultitenantTests_Dispose_Async()
        {
            var container = new StashboxContainer(c => c.WithDisposableTransientTracking());
            container.Register<IA, C>();

            ITenantDistributor md = new TenantDistributor(container);

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

            ITenantDistributor md = new TenantDistributor(container);

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

            ITenantDistributor md = new TenantDistributor(container);

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