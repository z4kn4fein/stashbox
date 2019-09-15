using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class CircularDependencyTests
    {
        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_StandardResolve()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test1>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        public void CircularDependencyTests_StandardResolve_Parallel_ShouldntThrow()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test4>();
                Parallel.For(0, 5000, i =>
                {
                    container.Resolve<ITest1>();
                });
            }
        }

        [TestMethod]
        public void CircularDependencyTests_StandardResolve_Parallel_Runtime_ShouldntThrow()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking(true)))
            {
                container.Register<ITest1, Test4>();
                Parallel.For(0, 5000, i =>
                {
                    container.Resolve<ITest1>();
                });
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_DependencyProperty()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test2>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_InjectionMethod()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test3>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Generic_StandardResolve()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                container.Resolve<ITest1<int, int>>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Generic_DependencyProperty()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register(typeof(ITest1<,>), typeof(Test2<,>));
                container.Resolve<ITest1<int, int>>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Generic_InjectionMethod()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register(typeof(ITest1<,>), typeof(Test3<,>));
                container.Resolve<ITest1<int, int>>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Func()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test5>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Lazy()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test6>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Tuple()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test7>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Enumerable()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.Register<ITest1, Test8>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Runtime()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking(true)))
            {
                container.Register<ITest1, Test1>(config => config.WithFactory(() => container.Resolve<ITest1>()));
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Runtime_Resolver()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking(true)))
            {
                container.Register<ITest1, Test1>(config => config.WithFactory(r => r.Resolve<ITest1>()));
                container.Resolve<ITest1>();
            }
        }

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
            public Test7(Tuple<ITest1> test1)
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
