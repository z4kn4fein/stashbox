using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{

    public class StandardResolveTests
    {
        [Fact]
        public void StandardResolveTests_Resolve()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest2, Test2>();
            container.Register<ITest3, Test3>();

            var test3 = container.Resolve<ITest3>();
            var test2 = container.Resolve<ITest2>();
            var test1 = container.Resolve<ITest1>();

            Assert.NotNull(test3);
            Assert.NotNull(test2);
            Assert.NotNull(test1);

            Assert.IsType<Test1>(test1);
            Assert.IsType<Test2>(test2);
            Assert.IsType<Test3>(test3);
        }

        [Fact]
        public void StandardResolveTests_Ensure_DependencyResolver_CanBeResolved()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.RegisterScoped<ResolverTest>();

            using var scope = container.BeginScope();
            var resolver = scope.Resolve<IDependencyResolver>();

            var test = scope.Resolve<ResolverTest>();

            Assert.Same(resolver, test.DependencyResolver);

            using var scope1 = container.BeginScope();
            var scopedResolver = scope1.Resolve<IDependencyResolver>();
            var test1 = scope1.Resolve<ResolverTest>();

            Assert.Same(scope1, scopedResolver);
            Assert.Same(scope1, test1.DependencyResolver);
        }

        [Fact]
        public void StandardResolveTests_Ensure_DependencyResolver_CanBeResolved_FromRoot()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.Register<ResolverTest>();
            var resolver = container.Resolve<IDependencyResolver>();
            var test = container.Resolve<ResolverTest>();

            Assert.Same(resolver, test.DependencyResolver);
            Assert.Same(test.DependencyResolver, container.ContainerContext.RootScope);
        }

        [Fact]
        public void StandardResolveTests_Factory()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            var test1 = container.ResolveFactory(typeof(ITest1)).DynamicInvoke();

            Assert.NotNull(test1);
            Assert.IsType<Test1>(test1);
        }

        [Fact]
        public void StandardResolveTests_Factory_Scoped()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            using var child = container.BeginScope();
            var test1 = child.ResolveFactory(typeof(ITest1)).DynamicInvoke();

            Assert.NotNull(test1);
            Assert.IsType<Test1>(test1);
        }

        [Fact]
        public void StandardResolveTests_Factory_ResolutionFailed()
        {
            using IStashboxContainer container = new StashboxContainer();
            Assert.Throws<ResolutionFailedException>(() => container.ResolveFactory(typeof(ITest1)).DynamicInvoke());
        }

        [Fact]
        public void StandardResolveTests_Factory_ResolutionFailed_Null()
        {
            using IStashboxContainer container = new StashboxContainer();
            var factory = container.ResolveFactory(typeof(ITest1), nullResultAllowed: true);

            Assert.Null(factory);
        }

        [Fact]
        public void StandardResolveTests_DependencyResolve_ResolutionFailed()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.Register<ITest2, Test2>();
            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest2>());
        }

        [Fact]
        public void StandardResolveTests_DependencyResolve_ResolutionFailed_AllowNull()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.Register<ITest2, Test2>();
            var result = container.Resolve<ITest2>(nullResultAllowed: true);

            Assert.Null(result);
        }

        [Fact]
        public void StandardResolveTests_Resolve_ResolutionFailed()
        {
            using IStashboxContainer container = new StashboxContainer();
            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void StandardResolveTests_Resolve_ResolutionFailed_AllowNull()
        {
            using IStashboxContainer container = new StashboxContainer();
            var result = container.Resolve<ITest1>(nullResultAllowed: true);

            Assert.Null(result);
        }

        [Fact]
        public void StandardResolveTests_Resolve_Parallel()
        {
            IStashboxContainer container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest2, Test2>();
            container.Register<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                if (i % 100 == 0)
                {
                    container.Register<ITest1, Test1>(context => context.WithName(i.ToString()));
                    container.Register<ITest3, Test3>(context => context.WithName($"ITest3{i.ToString()}"));
                    var test33 = container.Resolve<ITest3>($"ITest3{i.ToString()}");
                    var test11 = container.Resolve(typeof(ITest1), i.ToString());
                    Assert.NotNull(test33);
                    Assert.NotNull(test11);

                    Assert.IsType<Test1>(test11);
                    Assert.IsType<Test3>(test33);
                }

                var test3 = container.Resolve<ITest3>();
                var test2 = container.Resolve<ITest2>();
                var test1 = container.Resolve<ITest1>();

                Assert.NotNull(test3);
                Assert.NotNull(test2);
                Assert.NotNull(test1);

                Assert.IsType<Test1>(test1);
                Assert.IsType<Test2>(test2);
                Assert.IsType<Test3>(test3);
            });
        }

        [Fact]
        public void StandardResolveTests_Resolve_Parallel_Lazy()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest2, Test2>();
            container.Register<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                if (i % 100 == 0)
                {
                    container.Register<ITest1, Test1>();
                    container.Register<ITest3, Test3>();
                }

                var test3 = container.Resolve<Lazy<ITest3>>();
                var test2 = container.Resolve<Lazy<ITest2>>();
                var test1 = container.Resolve<Lazy<ITest1>>();

                Assert.NotNull(test3.Value);
                Assert.NotNull(test2.Value);
                Assert.NotNull(test1.Value);
            });
        }

        [Fact]
        public void StandardResolveTests_Resolve_Scoped()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.RegisterScoped<ITest1, Test1>();

            using var scope = container.BeginScope();

            var inst = scope.Resolve<ITest1>();
            var inst2 = scope.Resolve<ITest1>();

            Assert.Same(inst, inst2);

            using var child = container.BeginScope();
            var inst3 = child.Resolve<ITest1>();
            var inst4 = child.Resolve<ITest1>();

            Assert.NotSame(inst, inst3);
            Assert.Same(inst3, inst4);
        }

        [Fact]
        public void StandardResolveTests_Resolve_Scoped_Factory()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.RegisterScoped<ITest1, Test1>();

            using var scope = container.BeginScope();
            var factory = scope.ResolveFactory<ITest1>();

            var inst = factory();
            var inst2 = factory();

            Assert.Same(inst, inst2);

            using var child = container.BeginScope();
            var scopeFactory = child.ResolveFactory<ITest1>();
            var inst3 = scopeFactory();
            var inst4 = scopeFactory();

            Assert.NotSame(inst, inst3);
            Assert.Same(inst3, inst4);
        }

        [Fact]
        public void StandardResolveTests_Resolve_Scoped_Injection()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.RegisterScoped(typeof(ITest1), typeof(Test1));
            container.RegisterScoped<ITest4, Test4>();

            using var scope = container.BeginScope();

            var inst = scope.Resolve<ITest4>();
            var inst2 = scope.Resolve<ITest4>();

            Assert.Same(inst.Test, inst2.Test);
            Assert.Same(inst.Test2, inst2.Test2);
            Assert.Same(inst.Test, inst2.Test2);

            using var child = container.BeginScope();
            var inst3 = child.Resolve<ITest4>();
            var inst4 = child.Resolve<ITest4>();

            Assert.NotSame(inst.Test, inst4.Test);
            Assert.NotSame(inst.Test2, inst4.Test2);
            Assert.NotSame(inst.Test, inst4.Test2);

            Assert.Same(inst3.Test, inst4.Test);
            Assert.Same(inst3.Test2, inst4.Test2);
            Assert.Same(inst3.Test, inst4.Test2);
        }

        [Fact]
        public void StandardResolveTests_Resolve_Scoped_Injection_Factory()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.RegisterScoped(typeof(ITest1), typeof(Test1));
            container.RegisterScoped<ITest4, Test4>();

            using var scope = container.BeginScope();
            var factory = scope.ResolveFactory<ITest4>();

            var inst = factory();
            var inst2 = factory();

            Assert.Same(inst.Test, inst2.Test);
            Assert.Same(inst.Test2, inst2.Test2);
            Assert.Same(inst.Test, inst2.Test2);

            using var child = container.BeginScope();
            var scopedFactory = child.ResolveFactory<ITest4>();

            var inst3 = scopedFactory();
            var inst4 = scopedFactory();

            Assert.NotSame(inst.Test, inst4.Test);
            Assert.NotSame(inst.Test2, inst4.Test2);
            Assert.NotSame(inst.Test, inst4.Test2);

            Assert.Same(inst3.Test, inst4.Test);
            Assert.Same(inst3.Test2, inst4.Test2);
            Assert.Same(inst3.Test, inst4.Test2);
        }

        [Fact]
        public void StandardResolveTests_Resolve_LastService()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.Register(typeof(ITest1), typeof(Test1));
            container.Register(typeof(ITest1), typeof(Test11));
            container.Register(typeof(ITest1), typeof(Test12));

            var inst = container.Resolve<ITest1>();

            Assert.IsType<Test12>(inst);
        }

        [Fact]
        public void StandardResolveTests_Resolve_MostParametersConstructor_WithoutDefault()
        {
            using IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters));
            container.Register(typeof(ITest1), typeof(Test1));
            container.Register(typeof(ITest2), typeof(Test22));

            var inst = container.Resolve<ITest2>();
        }

        [Fact]
        public void StandardResolveTests_Resolve_MostParametersConstructor()
        {
            using IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters));
            container.Register(typeof(ITest1), typeof(Test1), context => context.WithName("test1"));
            container.Register(typeof(ITest1), typeof(Test12), context => context.WithName("test12"));
            container.Register(typeof(ITest2), typeof(Test222));

            var inst = container.Resolve<ITest2>();
        }

        [Fact]
        public void StandardResolveTests_Resolve_LeastParametersConstructor()
        {
            using IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters));
            container.Register(typeof(ITest1), typeof(Test1));
            container.Register(typeof(ITest2), typeof(Test2222));

            var inst = container.Resolve<ITest2>();
        }

        [Fact]
        public void StandardResolveTests_Resolve_None_Of_The_Constructors_Selected()
        {
            using IStashboxContainer container = new StashboxContainer(config =>
            config.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters));
            container.Register(typeof(ITest2), typeof(Test222));

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest2>());
        }

        [Fact]
        public void StandardResolveTests_Resolve_Scoped_NullDependency()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.RegisterScoped<Test5>();
            var inst = container.Resolve<Test5>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void StandardResolveTests_Resolve_Singleton_NullDependency()
        {
            using IStashboxContainer container = new StashboxContainer();
            container.RegisterSingleton<Test5>();
            var inst = container.Resolve<Test5>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void StandardResolveTests_Resolve_WithFinalizer()
        {
            var finalized = false;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<Test1>(context => context.WithFinalizer(_ => finalized = true));
                container.Resolve<Test1>();
            }

            Assert.True(finalized);
        }

#if HAS_SERVICEPROVIDER
        [Fact]
        public void StandardResolveTests_ServiceProvider()
        {
            var inst = new StashboxContainer()
                .Register<ITest1, Test1>()
                .Resolve<ITest1>();

            Assert.NotNull(inst);
        }

        [Fact]
        public void StandardResolveTests_ServiceProvider_Scope_Self()
        {
            var scope = new StashboxContainer()
                .Register<ScopeDependent>()
                .BeginScope();

            Assert.Same(scope, scope.Resolve<IServiceProvider>());
            Assert.Same(scope, scope.Resolve<ScopeDependent>().ServiceProvider);
        }

        class ScopeDependent
        {
            public ScopeDependent(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get; }
        }
#endif

        interface ITest1 { string Name { get; set; } }

        interface ITest2 { string Name { get; set; } }

        interface ITest3 { string Name { get; set; } }

        interface ITest4 { ITest1 Test { get; } ITest1 Test2 { get; } }

        class Test1 : ITest1
        {
            public string Name { get; set; }
        }

        class Test11 : ITest1
        {
            public string Name { get; set; }
        }

        class Test12 : ITest1
        {
            public string Name { get; set; }
        }

        class Test2 : ITest2
        {
            public string Name { get; set; }

            public Test2(ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test1>(test1);
            }
        }

        class Test22 : ITest2
        {
            public string Name { get; set; }

            public Test22(ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test1>(test1);
            }

            public Test22(ITest1 test1, int index)
            {
                Assert.True(false, "Wrong constructor selected.");
            }
        }

        class Test222 : ITest2
        {
            public string Name { get; set; }

            public Test222(ITest1 test1)
            {
                Assert.True(false, "Wrong constructor selected.");
            }

            public Test222([Dependency("test1")]ITest1 test1, [Dependency("test12")]ITest1 test2)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureNotNull(test2, nameof(test2));
                Shield.EnsureTypeOf<Test1>(test1);
                Shield.EnsureTypeOf<Test12>(test2);
            }
        }

        class Test2222 : ITest2
        {
            public string Name { get; set; }

            public Test2222(ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test1>(test1);
            }

            public Test2222(ITest1 test1, [Dependency("test12")]ITest1 test2)
            {
                Assert.True(false, "Wrong constructor selected.");
            }
        }

        class Test3 : ITest3
        {
            public string Name { get; set; }

            public Test3(ITest1 test1, ITest2 test2)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureNotNull(test2, nameof(test2));
                Shield.EnsureTypeOf<Test1>(test1);
                Shield.EnsureTypeOf<Test2>(test2);
            }
        }

        class Test4 : ITest4
        {
            public ITest1 Test { get; }

            [Dependency]
            public ITest1 Test2 { get; set; }

            public Test4(ITest1 test)
            {
                this.Test = test;
            }
        }

        class Test5
        {
            public Test5(ITest1 test)
            {
            }
        }

        class ResolverTest
        {
            public IDependencyResolver DependencyResolver { get; }

            public ResolverTest(IDependencyResolver dependencyResolver)
            {
                this.DependencyResolver = dependencyResolver;
            }
        }
    }
}