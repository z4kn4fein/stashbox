using Moq;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests;

public class ContainerTests
{
    [Fact]
    public void ContainerTests_ChildContainer()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>();

        Assert.Empty(container.ChildContainers);

        var child = container.CreateChildContainer();
        child.Register<ITest3, Test3>();

        Assert.Contains(child, container.ChildContainers);
        Assert.Single(container.ChildContainers);

        var test3 = child.Resolve<ITest3>();

        Assert.NotNull(test3);
        Assert.IsType<Test3>(test3);
        Assert.Equal(container.ContainerContext, child.ContainerContext.ParentContext);
    }

    [Fact]
    public void ContainerTests_ChildContainer_RemovedFromParentOnChildDispose()
    {
        var container = new StashboxContainer();
        Assert.Empty(container.ChildContainers);

        var child = container.CreateChildContainer();
        Assert.Single(container.ChildContainers);

        child.Dispose();
        Assert.Empty(container.ChildContainers);
    }

#if HAS_ASYNC_DISPOSABLE
    [Fact]
    public async Task ContainerTests_ChildContainer_RemovedFromParentOnChildDisposeAsync()
    {
        var container = new StashboxContainer();
        Assert.Empty(container.ChildContainers);

        var child = container.CreateChildContainer();
        Assert.Single(container.ChildContainers);

        await child.DisposeAsync();
        Assert.Empty(container.ChildContainers);
    }
#endif

    [Fact]
    public void ContainerTests_ChildContainer_ResolveFromParent()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();

        var child = container.CreateChildContainer();

        var test1 = child.Resolve<ITest1>();

        Assert.NotNull(test1);
        Assert.IsType<Test1>(test1);
    }

    [Fact]
    public void ContainerTests_Validate_MissingDependency()
    {
        using var container = new StashboxContainer();
        container.Register<ITest2, Test2>();
        var agr = Assert.Throws<AggregateException>(() => container.Validate());
        Assert.IsType<ResolutionFailedException>(agr.InnerExceptions[0]);
    }

    [Fact]
    public void ContainerTests_Validate_CircularDependency()
    {
        using var container = new StashboxContainer();
        container.Register<ITest1, Test4>();
        container.Register<ITest3, Test3>();
        var agr = Assert.Throws<AggregateException>(() => container.Validate());
        Assert.IsType<CircularDependencyException>(agr.InnerExceptions[0]);
    }

    [Fact]
    public void ContainerTests_Validate_Ok()
    {
        using var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>();
        container.Validate();
    }

    [Fact]
    public void ContainerTests_Validate_Skips_Open_Generic()
    {
        using var container = new StashboxContainer();
        container.Register(typeof(TestOpenGeneric<>));
        container.Validate();
    }

    [Fact]
    public void ContainerTests_Validate_Throws_When_No_Public_Constructor_Found()
    {
        using var container = new StashboxContainer();
        container.Register<NoPublicConstructor>();
        var agr = Assert.Throws<AggregateException>(() => container.Validate());
        Assert.IsType<ResolutionFailedException>(agr.InnerExceptions[0]);
    }

    [Fact]
    public void ContainerTests_Validate_Throws_Multiple()
    {
        using var container = new StashboxContainer();
        container.Register<NoPublicConstructor>();
        container.Register<ITest1, Test4>();
        container.Register<ITest3, Test3>();
        var agr = Assert.Throws<AggregateException>(() => container.Validate());

        Assert.Equal(3, agr.InnerExceptions.Count);
        Assert.Equal(2, agr.InnerExceptions.Count(e => e is CircularDependencyException));
        Assert.Equal(1, agr.InnerExceptions.Count(e => e is ResolutionFailedException));
    }

    [Fact]
    public void ContainerTests_Ensure_Validate_Does_Not_Build_Singletons()
    {
        using var container = new StashboxContainer();
        container.RegisterSingleton<Test1>();

        container.Validate();

        var reg = container.GetRegistrationMappings().First(r => r.Key == typeof(Test1));
        var t = new Test1();
        var res = container.ContainerContext.RootScope.GetOrAddScopedObject(reg.Value.RegistrationId, (s, r) => t, null, typeof(Test1));

        Assert.Same(t, res);
    }

    [Fact]
    public void ContainerTests_CheckRegistration()
    {
        using var container = new StashboxContainer();
        container.Register<ITest1, Test1>();

        var reg = container.GetRegistrationMappings().FirstOrDefault(r => r.Key == typeof(ITest1));

        Assert.Equal(typeof(Test1), reg.Value.ImplementationType);

        reg = container.GetRegistrationMappings().FirstOrDefault(r => r.Value.ImplementationType == typeof(Test1));

        Assert.Equal(typeof(Test1), reg.Value.ImplementationType);
    }

    [Fact]
    public void ContainerTests_CanResolve()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>();

        var child = container.CreateChildContainer();

        Assert.True(container.CanResolve<ITest1>());
        Assert.True(container.CanResolve(typeof(ITest2)));
        Assert.True(container.CanResolve<IEnumerable<ITest2>>());
        Assert.True(container.CanResolve<Lazy<ITest2>>());
        Assert.True(container.CanResolve<Func<ITest2>>());
        Assert.True(container.CanResolve<Tuple<ITest2, object>>());

        Assert.True(child.CanResolve<ITest1>());
        Assert.True(child.CanResolve(typeof(ITest2)));
        Assert.True(child.CanResolve<IEnumerable<ITest2>>());
        Assert.True(child.CanResolve<Lazy<ITest2>>());
        Assert.True(child.CanResolve<Func<ITest2>>());
        Assert.True(child.CanResolve<Tuple<ITest2, object>>());

        Assert.False(container.CanResolve<ITest3>());
        Assert.False(container.CanResolve<ITest1>("test"));
        Assert.False(container.CanResolve(typeof(ITest1), "test"));
    }

    [Fact]
    public void ContainerTests_IsRegistered()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>(context => context.WithName("test"));

        var child = container.CreateChildContainer();

        Assert.True(container.IsRegistered<ITest1>());
        Assert.True(container.IsRegistered<ITest2>("test"));
        Assert.True(container.IsRegistered(typeof(ITest1)));
        Assert.False(container.IsRegistered<IEnumerable<ITest1>>());
        Assert.False(container.IsRegistered<Func<ITest1>>());
        Assert.False(container.IsRegistered<Lazy<ITest1>>());

        Assert.False(child.IsRegistered<ITest1>());
        Assert.False(child.IsRegistered(typeof(ITest1)));
        Assert.False(child.IsRegistered<IEnumerable<ITest1>>());
    }

    [Fact]
    public void ContainerTests_IsRegistered_VariableName()
    {
        var name = "name".ToLower();
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(c => c.WithName("name"));

        Assert.True(container.IsRegistered<ITest1>(name));
    }

    [Fact]
    public void ContainerTests_IsRegistered_OpenGeneric()
    {
        var container = new StashboxContainer()
            .Register<ITest1, Test1>()
            .Register(typeof(TestOpenGeneric<>));

        Assert.True(container.IsRegistered(typeof(TestOpenGeneric<>)));
        Assert.False(container.IsRegistered<TestOpenGeneric<ITest1>>());
    }

    [Fact]
    public void ContainerTests_ResolverTest()
    {
        var container = new StashboxContainer();
        container.RegisterResolver(new TestResolver());
        var inst = container.Resolve<ITest1>();

        Assert.IsType<Test1>(inst);
    }

    [Fact]
    public void ContainerTests_ResolverTest_SupportsMany()
    {
        var container = new StashboxContainer();
        container.RegisterResolver(new TestResolver2());
        var inst = container.Resolve<IEnumerable<ITest1>>();

        Assert.IsType<Test1>(inst.First());
    }

    [Fact]
    public void ContainerTests_ResolverTest_Null_Context_DoesNot_Break()
    {
        var container = new StashboxContainer();
        var mockResolver = new Mock<IServiceResolver>();
        mockResolver.Setup(r => r.CanUseForResolution(It.IsAny<TypeInformation>(), It.IsAny<ResolutionContext>())).Returns(true);
        mockResolver.Setup(r => r.GetExpression(It.IsAny<IResolutionStrategy>(), It.IsAny<TypeInformation>(), It.IsAny<ResolutionContext>())).Returns<ServiceContext>(null);
        container.RegisterResolver(mockResolver.Object);

        Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest1>());
        mockResolver.Verify(r => r.GetExpression(It.IsAny<IResolutionStrategy>(), It.IsAny<TypeInformation>(), It.IsAny<ResolutionContext>()), Times.Once);
    }

    [Fact]
    public void ContainerTests_UnknownType_Config()
    {
        var container = new StashboxContainer(config => config
            .WithUnknownTypeResolution(context => context.WithSingletonLifetime()));

        container.Resolve<Test1>();

        var regs = container.ContainerContext.RegistrationRepository.GetRegistrationMappings().ToArray();

        Assert.Single(regs);
        Assert.Equal(typeof(Test1), regs[0].Key);
        Assert.True(regs[0].Value.Lifetime is SingletonLifetime);
    }

    [Fact]
    public void ContainerTests_ChildContainer_Singleton()
    {
        var container = new StashboxContainer();
        container.RegisterSingleton<ITest1, Test1>();

        var child = container.CreateChildContainer();
        child.RegisterSingleton<ITest2, Test2>();

        var test = child.Resolve<ITest2>();

        Assert.NotNull(test);
        Assert.IsType<Test2>(test);
    }

    [Fact]
    public void ContainerTests_ChildContainer_Scoped()
    {
        var container = new StashboxContainer();
        container.RegisterScoped<ITest1, Test1>();

        var child = container.CreateChildContainer();
        child.Register<ITest2, Test2>();

        var test = child.BeginScope().Resolve<ITest2>();

        Assert.NotNull(test);
        Assert.IsType<Test2>(test);
    }

    [Fact]
    public void ContainerTests_ConfigurationChanged()
    {
        ContainerConfiguration newConfig = null;
        var container = new StashboxContainer(config => config.OnContainerConfigurationChanged(nc => newConfig = nc));
        container.Configure(config => config.WithUnknownTypeResolution());

        Assert.True(newConfig.UnknownTypeResolutionEnabled);
    }

    [Fact]
    public void ContainerTests_ConfigurationChange_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new StashboxContainer().Configure(null));
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Throws()
    {
        var exception = Assert.Throws<ServiceAlreadyRegisteredException>(() => new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException))
            .Register<S>().Register<S>());
        Assert.Equal(typeof(S), exception.Type);
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Does_Not_Throw_On_Replace()
    {
        using var container = new StashboxContainer(c =>
            c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));

        container.Register<S>().Register<S>(c => c.ReplaceExisting());

        Assert.Single(container.GetRegistrationDiagnostics().Where(r => r.ServiceType == typeof(S)));
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Does_Not_Throw_On_Replace_Only_If_Exists()
    {
        using var container = new StashboxContainer(c =>
            c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));

        container.Register<S>().Register<S>(c => c.ReplaceOnlyIfExists());

        Assert.Single(container.GetRegistrationDiagnostics().Where(r => r.ServiceType == typeof(S)));
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Does_Not_Throw_On_Different_Implementation()
    {
        using var container = new StashboxContainer(c =>
            c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));

        container.Register<ITest1, Test1>().Register<ITest1, Test11>();

        Assert.Equal(2, container.GetRegistrationDiagnostics().Where(r => r.ServiceType == typeof(ITest1)).Count());
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Skip()
    {
        using var container = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.SkipDuplications))
            .Register<S>(c => c.WithInitializer((s, r) => s.Id = 0)).Register<S>(c => c.WithInitializer((s, r) => s.Id = 1));
        var regs = container.GetRegistrationMappings();

        Assert.Single(regs);
        Assert.Equal(0, container.Resolve<S>().Id);
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Preserve()
    {
        var regs = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications))
            .Register<Test1>().Register<Test1>().GetRegistrationDiagnostics();

        Assert.Equal(2, regs.Count());
    }

    [Fact]
    public void ContainerTests_Diagnostics_Print()
    {
        var regs = new StashboxContainer()
            .Register<ITest1, Test1>("t1").Register<Test1>().GetRegistrationDiagnostics().Select(d => d.ToString()).OrderBy(s => s).ToArray();

        Assert.Equal("ITest1 => Test1, name: t1", regs[0]);
        Assert.Equal("Test1 => Test1, name: null", regs[1]);
    }

    [Fact]
    public void ContainerTests_Diagnostics_Generic_Print()
    {
        var regs = new StashboxContainer()
            .Register(typeof(TestOpenGeneric<>), c => c.WithName("t1"))
            .Register(typeof(TestOpenGeneric<>)).GetRegistrationDiagnostics()
            .Select(d => d.ToString()).OrderBy(s => s)
            .ToArray();

        Assert.Equal("TestOpenGeneric<> => TestOpenGeneric<>, name: null", regs[0]);
        Assert.Equal("TestOpenGeneric<> => TestOpenGeneric<>, name: t1", regs[1]);
    }

    [Fact]
    public void ContainerTests_Cache_Diag()
    {
        using var container = new StashboxContainer()
            .Register<ITest1, Test1>("t1").Register<ITest1, Test1>().Register<Test1>();

        container.Resolve<ITest1>();
        container.Resolve<ITest1>("t1");
        container.Resolve<Test1>();

        var cache = container.GetDelegateCacheEntries().ToArray();

        Assert.Equal(2, cache.Length);
        Assert.Equal(typeof(ITest1), cache[0].ServiceType);
        Assert.Equal(typeof(Test1), cache[1].ServiceType);
        Assert.IsType<Test1>(cache[0].CachedDelegate.Invoke(container.ContainerContext.RootScope, null));
        Assert.IsType<Test1>(cache[1].CachedDelegate.Invoke(container.ContainerContext.RootScope, null));
        Assert.Single(cache[0].NamedCacheEntries);
        Assert.Null(cache[1].NamedCacheEntries);
        Assert.Equal("t1", cache[0].NamedCacheEntries.First().Name);
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Preserve_Cache_Invalidates()
    {
        using var container = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications))
            .Register<ITest1, Test1>();

        var a = container.Resolve<ITest1>();

        container.Register<ITest1, Test11>();

        a = container.Resolve<ITest1>();

        Assert.IsType<Test11>(a);
    }

    [Fact]
    public void ContainerTests_Configuration_DuplicatedBehavior_Replace()
    {
        using var container = new StashboxContainer(c =>
                c.WithRegistrationBehavior(Rules.RegistrationBehavior.ReplaceExisting))
            .Register<S>(c => c.WithInitializer((s, r) => s.Id = 0)).Register<S>(c => c.WithInitializer((s, r) => s.Id = 1));
        var regs = container.GetRegistrationMappings();

        Assert.Single(regs);
        Assert.Equal(1, container.Resolve<S>().Id);
    }

    [Fact]
    public void ContainerTests_Throws_When_TypeMap_Invalid()
    {
        using var container = new StashboxContainer();
        Assert.Equal(typeof(Test2), Assert.Throws<InvalidRegistrationException>(() => container.Register(typeof(ITest1), typeof(Test2))).Type);
        Assert.Equal(typeof(Test2), Assert.Throws<InvalidRegistrationException>(() => container.RegisterDecorator(typeof(ITest1), typeof(Test2))).Type);
        Assert.Equal(typeof(Test2), Assert.Throws<InvalidRegistrationException>(() => container.RegisterDecorator<ITest1>(typeof(Test2))).Type);
        Assert.Equal(typeof(Test2), Assert.Throws<InvalidRegistrationException>(() => container.ReMapDecorator<ITest1>(typeof(Test2))).Type);
    }

    [Fact]
    public void ContainerTests_ChildContainer_Rebuild_Singletons_In_Child()
    {
        using var container = new StashboxContainer();

        container.RegisterSingleton<ITest1, Test1>();

        var a = container.Resolve<ITest1>();
        var b = container.Resolve<ITest1>();

        using var child = container.CreateChildContainer();

        var c = child.Resolve<ITest1>();

        Assert.Same(a, b);
        Assert.Same(a, c);
        Assert.Same(b, c);

        child.Configure(c => c.WithReBuildSingletonsInChildContainer());
        c = child.Resolve<ITest1>();

        Assert.Same(a, b);
        Assert.NotSame(a, c);
        Assert.NotSame(b, c);
    }

    [Fact]
    public void ContainerTests_ChildContainer_Rebuild_Singletons_In_Child_Deps()
    {
        using var container = new StashboxContainer(c => c.WithReBuildSingletonsInChildContainer());

        container.Register<ITest1, Test1>();
        container.RegisterSingleton<Test2>();

        var a = container.Resolve<Test2>();

        Assert.IsType<Test1>(a.Test1);

        using var child = container.CreateChildContainer();
        child.Register<ITest1, Test11>();

        var b = child.Resolve<Test2>();

        Assert.IsType<Test11>(b.Test1);
    }

    [Fact]
    public void ContainerTests_ChildContainer_Rebuild_Singletons_In_Child_Deps_Config_On_Child()
    {
        using var container = new StashboxContainer();

        container.Register<ITest1, Test1>();
        container.RegisterSingleton<Test2>();

        var a = container.Resolve<Test2>();

        Assert.IsType<Test1>(a.Test1);

        using var child = container.CreateChildContainer(c => c.WithReBuildSingletonsInChildContainer());
        child.Register<ITest1, Test11>();

        var b = child.Resolve<Test2>();

        Assert.IsType<Test11>(b.Test1);
    }

    [Fact]
    public void ContainerTests_Throws_Disposed_Exceptions()
    {
        var container = new StashboxContainer();

        var scope = container.BeginScope();
        scope.Dispose();

        Assert.Throws<ObjectDisposedException>(() => scope.Activate(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => scope.BeginScope());
        Assert.Throws<ObjectDisposedException>(() => scope.BuildUp(new object()));
        Assert.Throws<ObjectDisposedException>(() => scope.CanResolve(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => scope.GetService(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => scope.PutInstanceInScope(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => scope.Resolve(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => scope.ResolveAll(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => scope.ResolveFactory(this.GetType()));
        Assert.ThrowsAsync<ObjectDisposedException>(async () => await scope.InvokeAsyncInitializers());

        container.Dispose();

        Assert.Throws<ObjectDisposedException>(() => container.Activate(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.BeginScope());
        Assert.Throws<ObjectDisposedException>(() => container.BuildUp(new object()));
        Assert.Throws<ObjectDisposedException>(() => container.CanResolve(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.ComposeAssemblies(new[] { this.GetType().Assembly }));
        Assert.Throws<ObjectDisposedException>(() => container.ComposeAssembly(this.GetType().Assembly));
        Assert.Throws<ObjectDisposedException>(() => container.ComposeBy(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.Configure(c => { }));
        Assert.Throws<ObjectDisposedException>(() => container.CreateChildContainer());
        Assert.Throws<ObjectDisposedException>(() => container.GetRegistrationMappings());
        Assert.Throws<ObjectDisposedException>(() => container.GetService(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.IsRegistered(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.PutInstanceInScope(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.Register(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterAssemblies(new[] { this.GetType().Assembly }));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterAssembly(this.GetType().Assembly));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterAssemblyContaining<ITest1>());
        Assert.Throws<ObjectDisposedException>(() => container.RegisterDecorator(this.GetType(), this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterFunc<ITest1>(r => new Test1()));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterInstance(new object()));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterInstances(new object()));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterResolver(null));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterScoped<ITest1, Test1>());
        Assert.Throws<ObjectDisposedException>(() => container.RegisterSingleton<ITest1, Test1>());
        Assert.Throws<ObjectDisposedException>(() => container.RegisterTypes(new[] { this.GetType() }));
        Assert.Throws<ObjectDisposedException>(() => container.RegisterTypesAs<ITest1>(this.GetType().Assembly));
        Assert.Throws<ObjectDisposedException>(() => container.ReMap<ITest1, Test1>());
        Assert.Throws<ObjectDisposedException>(() => container.ReMapDecorator(this.GetType(), this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.Resolve(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.ResolveOrDefault(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.ResolveAll(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.ResolveFactory(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.ResolveFactoryOrDefault(this.GetType()));
        Assert.Throws<ObjectDisposedException>(() => container.Validate());
        Assert.Throws<ObjectDisposedException>(() => container.WireUp(new object()));
        Assert.ThrowsAsync<ObjectDisposedException>(async () => await container.InvokeAsyncInitializers());
    }

    interface ITest1 { }

    interface ITest2 { }

    interface ITest3 { }

    interface ITest5
    {
        Func<ITest2> Func { get; }
        Lazy<ITest2> Lazy { get; }
        IEnumerable<ITest3> Enumerable { get; }
        Tuple<ITest2, ITest3> Tuple { get; }
    }

    class Test1 : ITest1
    {
    }

    class Test11 : ITest1
    {
    }

    class Test2 : ITest2
    {
        public Test2(ITest1 test1)
        {
            Test1 = test1;
        }

        public ITest1 Test1 { get; }
    }

    class Test3 : ITest3
    {
        public Test3(ITest1 test1, ITest2 test2)
        {
        }
    }

    class Test4 : ITest1
    {
        public Test4(ITest3 test3)
        {

        }
    }

    class Test5 : ITest5
    {
        public Test5(Func<ITest2> func, Lazy<ITest2> lazy, IEnumerable<ITest3> enumerable, Tuple<ITest2, ITest3> tuple)
        {
            Func = func;
            Lazy = lazy;
            Enumerable = enumerable;
            Tuple = tuple;
        }

        public Func<ITest2> Func { get; }
        public Lazy<ITest2> Lazy { get; }
        public IEnumerable<ITest3> Enumerable { get; }
        public Tuple<ITest2, ITest3> Tuple { get; }
    }

    class TestResolver : IServiceResolver
    {
        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionInfo)
        {
            return typeInfo.Type == typeof(ITest1);
        }

        public ServiceContext GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionInfo)
        {
            return new ServiceContext(Expression.Constant(new Test1()), null);
        }
    }

    class TestResolver2 : IEnumerableSupportedResolver
    {
        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionInfo)
        {
            return typeInfo.Type == typeof(ITest1);
        }

        public ServiceContext GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionInfo)
        {
            return new ServiceContext(Expression.Constant(new Test1()), null);
        }

        public IEnumerable<ServiceContext> GetExpressionsForEnumerableRequest(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionInfo)
        {
            return new ServiceContext[] { new ServiceContext(Expression.Constant(new Test1()), null) };
        }
    }

    class S { public int Id { get; set; } }

    class TestOpenGeneric<T> { }

    class NoPublicConstructor
    {
        protected NoPublicConstructor() { }
    }
}