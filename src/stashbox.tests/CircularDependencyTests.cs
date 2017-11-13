using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Exceptions;
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
                container.RegisterType<ITest1, Test1>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        public void CircularDependencyTests_StandardResolve_Parallel_ShouldntThrow()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.RegisterType<ITest1, Test4>();
                Parallel.For(0, 50000, (i) =>
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
                container.RegisterType<ITest1, Test2>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_InjectionMethod()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.RegisterType<ITest1, Test3>();
                container.Resolve<ITest1>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Generic_StandardResolve()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                container.Resolve<ITest1<int, int>>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Generic_DependencyProperty()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test2<,>));
                container.Resolve<ITest1<int, int>>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void CircularDependencyTests_Generic_InjectionMethod()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test3<,>));
                container.Resolve<ITest1<int, int>>();
            }
        }
    }

    public interface ITest1 { }

    public interface ITest2 { }

    public interface ITest3 { }

    public interface ITest1<I, K> { }

    public class Test4 : ITest1 { }

    public class Test1<I, K> : ITest1<I, K>
    {
        public Test1(ITest1<I, K> test1)
        {
        }
    }

    public class Test2<I, K> : ITest1<I, K>
    {
        [Dependency]
        public ITest1<I, K> Test1 { get; set; }
    }

    public class Test3<I, K> : ITest1<I, K>
    {
        [InjectionMethod]
        public void Inject(ITest1<I, K> test1)
        { }
    }

    public class Test1 : ITest1
    {
        public Test1(ITest1 test1)
        {
        }
    }

    public class Test2 : ITest1
    {
        [Dependency]
        public ITest1 Test1 { get; set; }
    }

    public class Test3 : ITest1
    {
        [InjectionMethod]
        public void Inject(ITest1 test1)
        { }
    }
}
