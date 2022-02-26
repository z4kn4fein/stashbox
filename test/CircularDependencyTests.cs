using Stashbox.Attributes;
using Stashbox.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{

    public class CircularDependencyTests
    {
        [Fact]
        public void CircularDependencyTests_StandardResolve()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            var exception = Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
            Assert.Equal(typeof(Test1), exception.Type);
        }

        [Fact]
        public void CircularDependencyTests_StandardResolve_Parallel_ShouldNotThrow()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test4>();
            Parallel.For(0, 5000, i =>
            {
                container.Resolve<ITest1>();
            });
        }

        [Fact]
        public void CircularDependencyTests_StandardResolve_Parallel_Runtime_ShouldNotThrow()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            using var container = new StashboxContainer(config => config.WithRuntimeCircularDependencyTracking());
#pragma warning restore CS0618 // Type or member is obsolete
            container.Register<ITest1, Test4>();
            Parallel.For(0, 5000, i =>
            {
                container.Resolve<ITest1>();
            });
        }

        [Fact]
        public void CircularDependencyTests_DependencyProperty()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test2>();
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void CircularDependencyTests_InjectionMethod()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test3>();
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void CircularDependencyTests_Generic_StandardResolve()
        {
            using var container = new StashboxContainer();
            container.Register(typeof(ITest1<,>), typeof(Test1<,>));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1<int, int>>());
        }

        [Fact]
        public void CircularDependencyTests_Generic_DependencyProperty()
        {
            using var container = new StashboxContainer();
            container.Register(typeof(ITest1<,>), typeof(Test2<,>));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1<int, int>>());
        }

        [Fact]
        public void CircularDependencyTests_Generic_InjectionMethod()
        {
            using var container = new StashboxContainer();
            container.Register(typeof(ITest1<,>), typeof(Test3<,>));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1<int, int>>());
        }

        [Fact]
        public void CircularDependencyTests_Func()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test5>();
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void CircularDependencyTests_Lazy()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test6>();
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void CircularDependencyTests_Tuple()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test7>(c => c.WithMetadata(new object()));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void CircularDependencyTests_Enumerable()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test8>();
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void CircularDependencyTests_Runtime()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            using var container = new StashboxContainer(config => config.WithRuntimeCircularDependencyTracking());
#pragma warning restore CS0618 // Type or member is obsolete
            container.Register<ITest1, Test1>(config => config.WithFactory(r => (Test1)r.Resolve<ITest1>()));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void CircularDependencyTests_Runtime_Parameterized_Factory()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>(config => config.WithFactory<ITest1>(t => (Test1)t));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

#if HAS_ASYNC_DISPOSABLE
        [Fact]
        public async Task CircularDependencyTests_Runtime_Async()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await using var container = new StashboxContainer(config => config.WithRuntimeCircularDependencyTracking());
#pragma warning restore CS0618 // Type or member is obsolete
            container.Register<ITest1, Test1>(config => config.WithFactory(r => (Test1)r.Resolve<ITest1>()));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public async Task CircularDependencyTests_Runtime_Async_Parameterized_Factory()
        {
            await using var container = new StashboxContainer();
            container.Register<ITest1, Test1>(config => config.WithFactory<ITest1>(t => (Test1)t));
            Assert.Throws<CircularDependencyException>(() => container.Resolve<ITest1>());
        }
#endif

        interface ITest1 { }

        interface ITest2 { }

        interface ITest3 { }

        interface ITest1<I, K> { }

        class Test4 : ITest1 { }

        class Test1<I, K> : ITest1<I, K>
        {
            public Test1(ITest1<I, K> test1)
            {
            }
        }

        class Test2<I, K> : ITest1<I, K>
        {
            [Dependency]
            public ITest1<I, K> Test1 { get; set; }
        }

        class Test3<I, K> : ITest1<I, K>
        {
            [InjectionMethod]
            public void Inject(ITest1<I, K> test1)
            { }
        }

        class Test1 : ITest1
        {
            public Test1(ITest1 test1)
            {
            }
        }

        class Test2 : ITest1
        {
            [Dependency]
            public ITest1 Test1 { get; set; }
        }

        class Test3 : ITest1
        {
            [InjectionMethod]
            public void Inject(ITest1 test1)
            { }
        }

        class Test5 : ITest1
        {
            public Test5(Func<ITest1> test1)
            {
            }
        }

        class Test6 : ITest1
        {
            public Test6(Lazy<ITest1> test1)
            {
            }
        }

        class Test7 : ITest1
        {
            public Test7(Tuple<ITest1, object> test1)
            {
            }
        }

        class Test8 : ITest1
        {
            public Test8(IEnumerable<ITest1> test1)
            {
            }
        }
    }
}
