using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{

    public class InitializerFinalizerTests
    {
        [Fact]
        public void InitializerTests_Interface_Method()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithInitializer((t, resolver) => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void InitializerTests_ImplOnly_Method()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<Test1>();
                container.Register<ITest, Test>(context => context.WithInitializer((t, resolver) => t.ImplMethod(resolver.Resolve<Test1>())));
                test = container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_Register()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_Register_ByInterface()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest>(typeof(Test), context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_Register_ByImplementation()
        {
            Test test;
            using (var container = new StashboxContainer())
            {
                container.Register<Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<Test>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_ReMap()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
                container.ReMap<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_ReMap_ByInterface()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest>(typeof(Test), context => context.WithFinalizer(t => t.Method()));
                container.ReMap<ITest>(typeof(Test), context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_ReMap_ByImplementation()
        {
            Test test;
            using (var container = new StashboxContainer())
            {
                container.Register<Test>(context => context.WithFinalizer(t => t.Method()));
                container.ReMap<Test>(context => context.WithFinalizer(t => t.Method()));
                test = container.Resolve<Test>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_Instance_Interface()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstance<ITest>(test, finalizerDelegate: t => t.Method());
                test = (Test)container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_Instance_Implementation()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstance(test, finalizerDelegate: t => t.Method());
                test = container.Resolve<Test>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_WireUp_Interface()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.WireUp<ITest>(test, finalizerDelegate: t => t.Method());
                test = (Test)container.Resolve<ITest>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_WireUp_Implementation()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.WireUp(test, finalizerDelegate: t => t.Method());
                test = container.Resolve<Test>();
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public void FinalizerTests_Register_Multiple_Shouldnt_Throw()
        {
            using var container = new StashboxContainer();
            container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()));
            for (var i = 0; i < 10; i++)
            {
                var test = container.Resolve<ITest>();
                Assert.False(test.MethodCalled);
            }
        }

        [Fact]
        public void FinalizerTests_Register_Singleton_Multiple_Shouldnt_Throw()
        {
            using var container = new StashboxContainer();
            container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()).WithSingletonLifetime());
            for (var i = 0; i < 10; i++)
            {
                var test = container.Resolve<ITest>();
                Assert.False(test.MethodCalled);
            }
        }

        [Fact]
        public void FinalizerTests_Register_Scoped_Multiple_Shouldnt_Throw()
        {
            using var container = new StashboxContainer();
            container.Register<ITest, Test>(context => context.WithFinalizer(t => t.Method()).WithScopedLifetime());
            for (var i = 0; i < 10; i++)
            {
                ITest test;
                using (var scope = container.BeginScope())
                {
                    test = scope.Resolve<ITest>();
                    Assert.False(test.MethodCalled);
                }

                Assert.True(test.MethodCalled);
            }
        }

        [Fact]
        public void FinalizerTests_Instance_Implementation_Multiple_Shouldnt_Throw()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstance(test, finalizerDelegate: t => t.Method());
                for (var i = 0; i < 10; i++)
                {
                    var test1 = container.Resolve<Test>();
                    Assert.False(test1.MethodCalled);
                    Assert.Same(test, test1);
                }
            }

            Assert.True(test.MethodCalled);
        }

        [Fact]
        public async Task AsyncInitializer_Ensure_Order_Singleton()
        {
            using var container = new StashboxContainer()
                .Register<T1>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()).WithSingletonLifetime())
                .Register<T2>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()))
                .Register<T3>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()));

            var initializables = new List<IT>();

            container.Resolve<T3>(dependencyOverrides: new[] { initializables });
            await container.InvokeAsyncInitializers();

            Assert.Equal(3, initializables.Count);

            Assert.IsType<T3>(initializables[0]);
            Assert.IsType<T2>(initializables[1]);
            Assert.IsType<T1>(initializables[2]);
        }

        [Fact]
        public async Task AsyncInitializer_Ensure_Order()
        {
            using var container = new StashboxContainer()
                .Register<T1>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()))
                .Register<T2>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()))
                .Register<T3>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()));

            var initializables = new List<IT>();

            container.Resolve<T3>(dependencyOverrides: new[] { initializables });
            await container.InvokeAsyncInitializers();

            Assert.Equal(4, initializables.Count);

            Assert.IsType<T3>(initializables[0]);
            Assert.IsType<T2>(initializables[1]);
            Assert.IsType<T1>(initializables[2]);
            Assert.IsType<T1>(initializables[3]);
        }

        [Fact]
        public async Task AsyncInitializer_Scoped_Multiple()
        {
            using var container = new StashboxContainer()
                .Register<T1>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()).WithScopedLifetime())
                .Register<T2>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()).WithScopedLifetime())
                .Register<T3>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()).WithScopedLifetime());

            var initializables = new List<IT>();

            using var scope = container.BeginScope();

            scope.Resolve<T3>(dependencyOverrides: new[] { initializables });
            await scope.InvokeAsyncInitializers();

            Assert.Equal(3, initializables.Count);

            Assert.IsType<T3>(initializables[0]);
            Assert.IsType<T2>(initializables[1]);
            Assert.IsType<T1>(initializables[2]);

            scope.Resolve<T3>(dependencyOverrides: new[] { initializables });
            await scope.InvokeAsyncInitializers();
            Assert.Equal(3, initializables.Count);
        }

        [Fact]
        public async Task AsyncInitializer_Scoped_Singleton_Multiple()
        {
            using var container = new StashboxContainer()
                .Register<T1>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()).WithSingletonLifetime())
                .Register<T2>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()).WithScopedLifetime())
                .Register<T3>(c => c.WithAsyncInitializer((t, r, c) => t.InitAsync()).WithScopedLifetime());

            var initializables = new List<IT>();

            using var scope1 = container.BeginScope();

            scope1.Resolve<T3>(dependencyOverrides: new[] { initializables });
            await scope1.InvokeAsyncInitializers();

            Assert.Equal(3, initializables.Count);

            Assert.IsType<T1>(initializables[0]);
            Assert.IsType<T3>(initializables[1]);
            Assert.IsType<T2>(initializables[2]);

            using var scope2 = container.BeginScope();

            scope2.Resolve<T3>(dependencyOverrides: new[] { initializables });
            await scope2.InvokeAsyncInitializers();

            Assert.Equal(5, initializables.Count);

            Assert.IsType<T1>(initializables[0]);
            Assert.IsType<T3>(initializables[1]);
            Assert.IsType<T2>(initializables[2]);
            Assert.IsType<T3>(initializables[3]);
            Assert.IsType<T2>(initializables[4]);
        }

        [Fact]
        public void Finalizers_Ensure_Order_Singleton()
        {
            var finalizables = new List<IT>();
            {
                using var container = new StashboxContainer()
                    .Register<F1>(c => c.WithFinalizer(t => t.Fin()).WithSingletonLifetime())
                    .Register<F2>(c => c.WithFinalizer(t => t.Fin()))
                    .Register<F3>(c => c.WithFinalizer(t => t.Fin()));


                container.Resolve<F3>(dependencyOverrides: new[] { finalizables });
            }

            Assert.Equal(3, finalizables.Count);

            Assert.IsType<F3>(finalizables[0]);
            Assert.IsType<F2>(finalizables[1]);
            Assert.IsType<F1>(finalizables[2]);
        }

        [Fact]
        public void Finalizers_Ensure_Order()
        {
            var finalizables = new List<IT>();
            {
                using var container = new StashboxContainer()
                    .Register<F1>(c => c.WithFinalizer(t => t.Fin()))
                    .Register<F2>(c => c.WithFinalizer(t => t.Fin()))
                    .Register<F3>(c => c.WithFinalizer(t => t.Fin()));


                container.Resolve<F3>(dependencyOverrides: new[] { finalizables });
            }

            Assert.Equal(4, finalizables.Count);

            Assert.IsType<F3>(finalizables[0]);
            Assert.IsType<F1>(finalizables[1]);
            Assert.IsType<F2>(finalizables[2]);
            Assert.IsType<F1>(finalizables[3]);
        }

        interface ITest
        {
            bool MethodCalled { get; }

            void Method();
        }

        class Test1
        { }

        class Test : ITest
        {
            public void Method()
            {
                if (this.MethodCalled)
                    throw new Exception("Method called multiple times!");

                this.MethodCalled = true;
            }

            public void ImplMethod(Test1 t)
            {
                if (t == null)
                    throw new NullReferenceException();

                if (this.MethodCalled)
                    throw new Exception("Method called multiple times!");

                this.MethodCalled = true;
            }

            public bool MethodCalled { get; private set; }
        }

        interface IT { }

        class T1 : IT
        {
            private readonly List<IT> initializables;

            public T1(List<IT> initializables)
            {
                this.initializables = initializables;
            }

            public Task InitAsync()
            {
                this.initializables.Add(this);
                return Task.FromResult(false);
            }
        }

        class T2 : IT
        {
            private readonly List<IT> initializables;
            private readonly T1 t1;

            public T2(List<IT> initializables, T1 t1)
            {
                this.initializables = initializables;
                this.t1 = t1;
            }

            public Task InitAsync()
            {
                this.initializables.Add(this);
                return Task.FromResult(false);
            }
        }

        class T3 : IT
        {
            private readonly List<IT> initializables;
            private readonly T1 t1;
            private readonly T2 t2;

            public T3(List<IT> initializables, T1 t1, T2 t2)
            {
                this.initializables = initializables;
                this.t1 = t1;
                this.t2 = t2;
            }

            public Task InitAsync()
            {
                this.initializables.Add(this);
                return Task.FromResult(false);
            }
        }

        class F1 : IT
        {
            private readonly List<IT> finalizables;

            public F1(List<IT> finalizables)
            {
                this.finalizables = finalizables;
            }

            public void Fin()
            {
                this.finalizables.Add(this);
            }
        }

        class F2 : IT
        {
            private readonly List<IT> finalizables;
            private readonly F1 f1;

            public F2(List<IT> finalizables, F1 f1)
            {
                this.finalizables = finalizables;
                this.f1 = f1;
            }

            public void Fin()
            {
                this.finalizables.Add(this);
            }
        }

        class F3 : IT
        {
            private readonly List<IT> finalizables;
            private readonly F1 f1;
            private readonly F2 f2;

            public F3(List<IT> finalizables, F2 f2, F1 f1)
            {
                this.finalizables = finalizables;
                this.f1 = f1;
                this.f2 = f2;
            }

            public void Fin()
            {
                this.finalizables.Add(this);
            }
        }
    }
}
