using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Resolution;
using Stashbox.Tests.Utils;
using System;
using Xunit;

namespace Stashbox.Tests;

public class FactoryTests
{
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_DependencyResolve(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, Test>(context => context.WithFactory(() => new Test("test")));
        container.Register<ITest1, Test12>();

        var inst = container.Resolve<ITest1>();

        Assert.IsType<Test>(inst.Test);
        Assert.Equal("test", inst.Test.Name);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_DependencyResolve_ServiceUpdated(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, Test>(context => context.WithFactory(() => new Test("test")));
        container.Register<ITest2, Test2>();
        container.ReMap<ITest, Test>(context => context.WithFactory(() => new Test("test1")));
        var inst = container.Resolve<ITest2>();

        Assert.IsType<Test>(inst.Test);
        Assert.Equal("test1", inst.Test.Name);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, Test>(context => context.WithFactory(() => new Test("test")));
        container.Register<ITest1, Test1>();

        var inst = container.Resolve<ITest1>();

        Assert.IsType<Test>(inst.Test);
        Assert.Equal("test", inst.Test.Name);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_NotSame(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, Test>(context =>
            context.WithInjectionParameter("name", "test"));
        container.Register<ITest1>(context => context.WithFactory(cont =>
        {
            var test1 = cont.Resolve<ITest>();
            return new Test12(test1);
        }));

        var inst1 = container.Resolve<ITest1>();
        var inst2 = container.Resolve<ITest1>();

        Assert.NotSame(inst1.Test, inst2.Test);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_ContainerFactory(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<Test3>();
        container.Register<ITest>(context => context.WithFactory(c => c.Resolve<Test3>()));

        var inst = container.Resolve<ITest>();

        Assert.IsType<Test3>(inst);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_ContainerFactory_Constructor(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<Test3>();
        container.Register<ITest1, Test12>();
        container.Register(typeof(ITest), context => context.WithFactory(() => new Test3()));

        var test1 = container.Resolve<ITest1>();
        Assert.IsType<Test3>(test1.Test);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_ContainerFactory_Initializer(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest4>(context =>
            context.WithFactory(() => new Test4()).WithInitializer((t, r) => t.Init("Test")));

        var test1 = container.Resolve<ITest4>();
        Assert.Equal("Test", test1.Name);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Gets_The_Proper_Scope(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<Test5>(context =>
            context.WithFactory(resolver => new Test5(resolver)));

        using var scope = container.BeginScope();
        var t = scope.Resolve<Test5>();

        Assert.Same(scope, t.DependencyResolver);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_With_Param1(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register<Dummy>(context =>
            context.WithFactory<IT1>(t1 =>
            {
                Assert.IsType<TComp>(t1);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_With_Param2(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register<Dummy>(context =>
            context.WithFactory<IT1, IT2>((t1, t2) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_With_Param3(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register<Dummy>(context =>
            context.WithFactory<IT1, IT2, IT3>((t1, t2, t3) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_With_Param4(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register<Dummy>(context =>
            context.WithFactory<IT1, IT2, IT3, IT4>((t1, t2, t3, t4) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_With_Param5(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register<Dummy>(context =>
            context.WithFactory<IT1, IT2, IT3, IT4, IT5>((t1, t2, t3, t4, t5) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                Assert.IsType<TComp>(t5);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_With_Param_With_Resolver(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<Test5>(context =>
            context.WithFactory<IDependencyResolver>(resolver =>
            {
                return new Test5(resolver);
            }));

        using var scope = container.BeginScope();
        var inst = scope.Resolve<Test5>();

        Assert.Same(scope, inst.DependencyResolver);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_NonGeneric_Resolve_Gets_The_Proper_Scope(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register(typeof(Test5), context =>
            context.WithFactory(resolver => new Test5(resolver)));

        using var scope = container.BeginScope();
        var t = scope.Resolve<Test5>();

        Assert.Same(scope, t.DependencyResolver);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_NonGeneric_Resolve_With_Param1(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register(typeof(Dummy), context =>
            context.WithFactory<IT1>(t1 =>
            {
                Assert.IsType<TComp>(t1);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_NonGeneric_Resolve_With_Param2(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register(typeof(Dummy), context =>
            context.WithFactory<IT1, IT2>((t1, t2) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_NonGeneric_Resolve_With_Param3(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register(typeof(Dummy), context =>
            context.WithFactory<IT1, IT2, IT3>((t1, t2, t3) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_NonGeneric_Resolve_With_Param4(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register(typeof(Dummy), context =>
            context.WithFactory<IT1, IT2, IT3, IT4>((t1, t2, t3, t4) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_NonGeneric_Resolve_With_Param5(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<TComp>(c => c.AsImplementedTypes());
        container.Register(typeof(Dummy), context =>
            context.WithFactory<IT1, IT2, IT3, IT4, IT5>((t1, t2, t3, t4, t5) =>
            {
                Assert.IsType<TComp>(t1);
                Assert.IsType<TComp>(t2);
                Assert.IsType<TComp>(t3);
                Assert.IsType<TComp>(t4);
                Assert.IsType<TComp>(t5);
                return new Dummy();
            }));

        container.Resolve<Dummy>();
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_NonGeneric_Resolve_With_Param_With_Resolver(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register(typeof(Test5), context =>
            context.WithFactory<IDependencyResolver>(resolver =>
            {
                return new Test5(resolver);
            }));

        using var scope = container.BeginScope();
        var inst = scope.Resolve<Test5>();

        Assert.Same(scope, inst.DependencyResolver);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Ensure_Exclude_Works(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType).WithDisposableTransientTracking());
        var disposable = new Disposable();
        Disposable second;
        bool shouldSkip = true;
        container.Register<Disposable>(context => context
            .WithFactory<IRequestContext>(requestContext =>
            {
                if (shouldSkip)
                {
                    shouldSkip = false;
                    return requestContext.ExcludeFromTracking(disposable);
                }

                return new Disposable();
            }));

        {
            using var scope = container.BeginScope();
            var inst = scope.Resolve<Disposable>();

            Assert.Same(disposable, inst);

            second = scope.Resolve<Disposable>();

            Assert.NotSame(disposable, second);
        }

        Assert.True(second.IsDisposed);
        Assert.False(disposable.IsDisposed);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Ensure_Exclude_Works_Scoped(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var disposable = new Disposable();
        Disposable second;
        bool shouldSkip = true;
        container.Register<Disposable>(context => context
            .WithScopedLifetime()
            .WithFactory<IRequestContext>(requestContext =>
            {
                if (shouldSkip)
                {
                    shouldSkip = false;
                    return requestContext.ExcludeFromTracking(disposable);
                }

                return new Disposable();
            }));

        {
            using var scope1 = container.BeginScope();
            var inst = scope1.Resolve<Disposable>();

            Assert.Same(disposable, inst);

            using var scope2 = container.BeginScope();
            second = scope2.Resolve<Disposable>();

            Assert.NotSame(disposable, second);
        }

        Assert.True(second.IsDisposed);
        Assert.False(disposable.IsDisposed);
    }

    private delegate object Factory(Type serviceType);

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Factory_Type(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test3>()
            .Register<Factory>(c => c.WithFactory(r => r.Resolve));

        var factory = container.Resolve<Factory>();
        var inst = factory(typeof(Test3));

        Assert.NotNull(inst);
        Assert.IsType<Test3>(inst);
    }

    private delegate ITest TestFactory(string s);

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Factory_Delegate(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>();

        var factory = container.Resolve<TestFactory>();
        var inst = factory("test");

        Assert.NotNull(inst);
        Assert.IsType<Test>(inst);
        Assert.Equal("test", inst.Name);
    }

    private delegate void TestAction(string s);

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Throws(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>();

        Assert.Throws<ResolutionFailedException>(() => container.Resolve<TestAction>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Multiple(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType).WithDisposableTransientTracking())
            .Register<IT1, TD>(c => c.WithFactory(() => new TD()).AsServiceAlso<IT2>().AsServiceAlso<IT3>().AsServiceAlso<IT4>())
            .Register<TT>();

        var inst = container.Resolve<TT>();
        Assert.IsType<TD>(inst.T1);
        Assert.IsType<TD>(inst.T2);
        Assert.IsType<TD>(inst.T3);
        Assert.IsType<TD>(inst.T4);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Multiple_All(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType).WithDisposableTransientTracking())
            .Register<IT1, TD>(c => c.WithFactory(() => new TD()).AsImplementedTypes())
            .Register<TT>();

        var inst = container.Resolve<TT>();
        Assert.IsType<TD>(inst.T1);
        Assert.IsType<TD>(inst.T2);
        Assert.IsType<TD>(inst.T3);
        Assert.IsType<TD>(inst.T4);
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT1>(c => c.WithFactory(() => new TD()));

        Assert.NotNull(container.Resolve<IT1>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_AsServiceAlso(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT1>(c => c.WithFactory(() => new TD()).AsServiceAlso<IT2>());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.IsType<TD>(container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_DependencyResolver_AsServiceAlso(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT1>(c => c.WithFactory(r => new TD()).AsServiceAlso<IT2>());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.IsType<TD>(container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_AsImplementedTypes(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT1>(c => c.WithFactory(() => new TD()).AsImplementedTypes());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.IsType<TD>(container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_DependencyResolver_AsImplementedTypes(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT1>(c => c.WithFactory(r => new TD()).AsImplementedTypes());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.IsType<TD>(container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_DependencyResolver_AsServiceAlso_Param(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Dummy>()
            .Register<IT1>(c => c.WithFactory<Dummy>(_ => new TD()).AsServiceAlso<IT2>());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.IsType<TD>(container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_DependencyResolver_AsImplementedTypes_Param(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Dummy>()
            .Register<IT1>(c => c.WithFactory<Dummy>(_ => new TD()).AsImplementedTypes());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.Throws<ResolutionFailedException>(() => container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_AsServiceAlso_Interface(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT1>(c => c.WithFactory(() => (IT1)new TD()).AsServiceAlso<IT2>());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.IsType<TD>(container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_AsImplementedTypes_Interface(CompilerType compilerType)
    {
        using var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<IT1>(c => c.WithFactory(() => (IT1)new TD()).AsImplementedTypes());

        Assert.IsType<TD>(container.Resolve<IT1>());
        Assert.Throws<ResolutionFailedException>(() => container.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_Unknown(CompilerType compilerType)
    {
        using var container1 = new StashboxContainer().Register<IT1, TD>();
        
        using var container2 = new StashboxContainer(c => c.WithCompiler(compilerType)
            .WithUnknownTypeResolution(opts =>
            {
                if (opts.ServiceType == typeof(IT1))
                    opts.WithFactory(() => container1.Resolve(opts.ServiceType)).AsServiceAlso<IT2>();
            }));

        Assert.IsType<TD>(container2.Resolve<IT1>());
        Assert.IsType<TD>(container2.Resolve<IT2>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_Unknown_Open_Generic(CompilerType compilerType)
    {
        using var container1 = new StashboxContainer().Register(typeof(ITG<>), typeof(TG<>));
        
        using var container2 = new StashboxContainer(c => c.WithCompiler(compilerType)
            .WithUnknownTypeResolution(opts =>
            {
                opts.WithFactory(() => container1.Resolve(opts.ServiceType));
            }));

        Assert.IsType<TG<int>>(container2.Resolve<ITG<int>>());
        Assert.IsType<TG<string>>(container2.Resolve<ITG<string>>());
    }
    
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void FactoryTests_Resolve_Action_Without_Implementation_Open_Generic(CompilerType compilerType)
    {
        using var container1 = new StashboxContainer().Register(typeof(ITG<>), typeof(TG<>));
        
        using var container2 = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register(typeof(ITG<>), c => c.WithFactory<TypeInformation>(t => container1.Resolve(t.Type)));

        Assert.IsType<TG<int>>(container2.Resolve<ITG<int>>());
        Assert.IsType<TG<string>>(container2.Resolve<ITG<string>>());
    }

    interface ITG<T>;
    
    class TG<T> : ITG<T>;
    
    interface ITest { string Name { get; } }

    interface ITest1 { ITest Test { get; } }

    interface ITest2 { ITest Test { get; } }

    class Test3 : ITest
    {
        public string Name { get; }
    }

    class Test : ITest
    {
        public string Name { get; }

        public Test(string name)
        {
            this.Name = name;
        }
    }

    class Test2 : ITest2
    {
        [Dependency]
        public ITest Test { get; set; }
    }

    class Test1 : ITest1
    {
        public ITest Test { get; set; }

        [InjectionMethod]
        public void Init(ITest test)
        {
            this.Test = test;
        }
    }

    class Test12 : ITest1
    {
        public ITest Test { get; private set; }

        public Test12(ITest test)
        {
            this.Test = test;
        }
    }

    interface ITest4 : ITest
    {
        void Init(string name);
    }

    class Test4 : ITest4
    {
        public string Name { get; private set; }

        public void Init(string name) => Name = name;
    }

    class Test5
    {
        public IDependencyResolver DependencyResolver { get; }

        public Test5(IDependencyResolver dependencyResolver)
        {
            DependencyResolver = dependencyResolver;
        }
    }

    interface IT1;

    interface IT2;

    interface IT3;

    interface IT4;

    interface IT5;

    class TComp : IT1, IT2, IT3, IT4, IT5;

    class Dummy;

    class Disposable : IDisposable
    {
        public bool IsDisposed { get; set; }
        public void Dispose()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(nameof(Disposable));

            this.IsDisposed = true;
        }
    }

    class TD : IT1, IT2, IT3, IT4, IT5
    {
        [InjectionMethod]
        public void Init() {}
    }
    
    class TT
    {
        public IT1 T1 { get; }
        public IT2 T2 { get; }
        public IT3 T3 { get; }
        public IT4 T4 { get; }

        public TT(IT1 t1, IT2 t2, IT3 t3, IT4 t4)
        {
            T1 = t1;
            T2 = t2;
            T3 = t3;
            T4 = t4;
        }
    }
}