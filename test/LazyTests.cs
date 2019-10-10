using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Tests
{
    
    public class LazyTests
    {
        [Fact]
        public void LazyTests_Resolve()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            var inst = container.Resolve<Lazy<ITest>>();

            Assert.NotNull(inst);
            Assert.False(inst.IsValueCreated);
            Assert.IsType<Lazy<ITest>>(inst);
            Assert.IsType<Test>(inst.Value);
        }
        
        [Fact]
        public void LazyTests_Resolve_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Lazy<ITest>>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void LazyTests_Resolve_Func()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            var inst = container.Resolve<Lazy<Func<ITest>>>();

            Assert.NotNull(inst);
            Assert.False(inst.IsValueCreated);
            Assert.IsType<Lazy<Func<ITest>>>(inst);
            Assert.IsType<Test>(inst.Value());
        }

        [Fact]
        public void LazyTests_Resolve_Func_Param()
        {
            var container = new StashboxContainer(config => config.WithCircularDependencyWithLazy());
            container.Register<ITest2, Test3>();
            var inst = container.Resolve<Lazy<Func<int, Lazy<ITest2>>>>();

            Assert.NotNull(inst);
            Assert.False(inst.IsValueCreated);
            Assert.IsType<Lazy<Func<int, Lazy<ITest2>>>>(inst);
            Assert.IsType<Test3>(inst.Value(5).Value);
            Assert.Equal(5, inst.Value(5).Value.T);
        }

        [Fact]
        public void LazyTests_Resolve_Func_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Lazy<Func<ITest>>>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void LazyTests_Resolve_Enumerable()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            var inst = container.Resolve<Lazy<IEnumerable<ITest>>>();

            Assert.NotNull(inst);
            Assert.False(inst.IsValueCreated);
            Assert.IsType<Lazy<IEnumerable<ITest>>>(inst);
            Assert.IsType<Test>(inst.Value.First());
        }

        [Fact]
        public void LazyTests_Resolve_Enumerable_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Lazy<IEnumerable<ITest>>>();

            Assert.Empty(inst.Value);
        }

        [Fact]
        public void LazyTests_Resolve_ConstructorDependency()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            container.Register<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.NotNull(inst.Test);
            Assert.False(inst.Test.IsValueCreated);
            Assert.IsType<Lazy<ITest>>(inst.Test);
            Assert.IsType<Test>(inst.Test.Value);
        }

        [Fact]
        public void LazyTests_Resolve_ConstructorDependency_Null()
        {
            var container = new StashboxContainer();
            container.Register<Test2>();
            var inst = container.Resolve<Test2>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void LazyTests_Resolve_Circular()
        {
            var container = new StashboxContainer(config =>
            config.WithCircularDependencyTracking()
            .WithCircularDependencyWithLazy());

            container.RegisterSingleton<Circular1>();
            container.RegisterSingleton<Circular2>();

            var inst1 = container.Resolve<Circular1>();
            var inst2 = container.Resolve<Circular2>();

            Assert.Same(inst2, inst1.Dep.Value);
            Assert.Same(inst1, inst2.Dep.Value);
        }

        [Fact]
        public void LazyTests_Resolve_Circular_Evaluate()
        {
            var container = new StashboxContainer(config =>
            config.WithCircularDependencyTracking()
            .WithCircularDependencyWithLazy());

            container.Register<Circular1>();
            container.Register<Circular2>();

            var inst1 = container.Resolve<Circular1>();

            Assert.NotNull(inst1.Dep.Value.Dep.Value.Dep.Value.Dep);
        }

        [Fact]
        public void LazyTests_Resolve_Circular_Evaluate_Singleton()
        {
            var container = new StashboxContainer(config =>
            config.WithCircularDependencyTracking()
            .WithCircularDependencyWithLazy());

            container.RegisterSingleton<Circular1>();
            container.RegisterSingleton<Circular2>();

            var inst1 = container.Resolve<Circular1>();

            Assert.NotNull(inst1.Dep.Value.Dep.Value.Dep.Value.Dep);
        }

        [Fact]
        public void LazyTests_Resolve_IEnumerable_Circular()
        {
            var container = new StashboxContainer(config =>
            config.WithCircularDependencyTracking()
            .WithCircularDependencyWithLazy());

            container.RegisterSingleton<Circular3>();
            container.RegisterSingleton<Circular4>();

            var inst1 = container.Resolve<Circular3>();
            var inst2 = container.Resolve<Circular4>();

            Assert.Same(inst2, inst1.Dep.First().Value);
            Assert.Same(inst1, inst2.Dep.First().Value);
        }

        interface ITest
        { }

        class Test : ITest
        { }

        interface ITest2
        {
            int T { get; }
        }

        class Test3 : ITest2
        {
            public Test3(int t)
            {
                this.T = t;
            }

            public int T { get; }
        }

        class Test2
        {
            public Lazy<ITest> Test { get; }

            public Test2(Lazy<ITest> test)
            {
                this.Test = test;
            }
        }

        class Circular1
        {
            public Lazy<Circular2> Dep { get; set; }

            public Circular1(Lazy<Circular2> dep)
            {
                Dep = dep;
            }
        }

        class Circular2
        {
            public Lazy<Circular1> Dep { get; set; }

            public Circular2(Lazy<Circular1> dep)
            {
                Dep = dep;
            }
        }

        class Circular3
        {
            public IEnumerable<Lazy<Circular4>> Dep { get; set; }

            public Circular3(IEnumerable<Lazy<Circular4>> dep)
            {
                Dep = dep;
            }
        }

        class Circular4
        {
            public IEnumerable<Lazy<Circular3>> Dep { get; set; }

            public Circular4(IEnumerable<Lazy<Circular3>> dep)
            {
                Dep = dep;
            }
        }
    }
}
