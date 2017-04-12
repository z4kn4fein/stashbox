using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Configuration;
using Stashbox.Infrastructure;
using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void EnumerableTests_Resolve_Array_PreserveOrder()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var all = container.Resolve<ITest1[]>();

            Assert.AreEqual(3, all.Length);
        }

        [TestMethod]
        public void EnumerableTests_Resolve_IList()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var all = container.Resolve<IList<ITest1>>();

            Assert.AreEqual(3, all.Count);
        }
        
        [TestMethod]
        public void EnumerableTests_Resolve_ICollection()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var all = container.Resolve<ICollection<ITest1>>();

            Assert.AreEqual(3, all.Count);
        }

        [TestMethod]
        public void EnumerableTests_Resolve_IReadonlyCollection()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var all = container.Resolve<IReadOnlyCollection<ITest1>>();

            Assert.AreEqual(3, all.Count);
        }

        [TestMethod]
        public void EnumerableTests_Resolve_IReadOnlyList()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var all = container.Resolve<IReadOnlyList<ITest1>>();

            Assert.AreEqual(3, all.Count);
        }

        [TestMethod]
        public void EnumerableTests_Resolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>(context => context.WithName("enumerable"));
            container.RegisterType<ITest2, Test22>(context => context.WithName("array"));

            container.Resolve<ITest2>("enumerable");
            container.Resolve<ITest2>("array");

            var all = container.Resolve<IEnumerable<ITest2>>();
            var all2 = container.ResolveAll<ITest2>();

            Assert.AreEqual(2, all.Count());
            Assert.AreEqual(2, all2.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var all = container.Resolve<IEnumerable<ITest2>>();
            var all2 = container.ResolveAll<ITest2>();

            Assert.AreEqual(0, all.Count());
            Assert.AreEqual(0, all2.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Scoped_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var scope = container.BeginScope();

            var all = scope.Resolve<IEnumerable<ITest2>>();
            var all2 = scope.ResolveAll<ITest2>();

            Assert.AreEqual(0, all.Count());
            Assert.AreEqual(0, all2.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Scoped()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.BeginScope();

            var all = child.Resolve<IEnumerable<ITest1>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Parent()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.CreateChildContainer();

            var all = child.Resolve<IEnumerable<ITest1>>();

            Assert.AreEqual(3, all.Count());
        }
        
        [TestMethod]
        public void EnumerableTests_Resolve_Parent_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var child = container.CreateChildContainer();

            var all = child.Resolve<IEnumerable<ITest1>>();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Scoped_Lazy()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.BeginScope();

            var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Parent_Lazy()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.CreateChildContainer();

            var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Scoped_Lazy_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var child = container.BeginScope();

            var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Parent_Lazy_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var child = container.CreateChildContainer();

            var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Scoped_Func()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.BeginScope();

            var all = child.Resolve<IEnumerable<Func<ITest1>>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Parent_Func()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.CreateChildContainer();

            var all = child.Resolve<IEnumerable<Func<ITest1>>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Scoped_Func_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var child = container.BeginScope();

            var all = child.Resolve<IEnumerable<Func<ITest1>>>();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Parent_Func_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var child = container.CreateChildContainer();

            var all = child.Resolve<IEnumerable<Func<ITest1>>>();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Lazy()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var all = container.Resolve<IEnumerable<Lazy<ITest1>>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Lazy_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var all = container.Resolve<IEnumerable<Lazy<ITest1>>>();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Func()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var all = container.Resolve<IEnumerable<Func<ITest1>>>();

            Assert.AreEqual(3, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Resolve_Func_Null()
        {
            IStashboxContainer container = new StashboxContainer();

            var all = container.Resolve<IEnumerable<Func<ITest1>>>();

            Assert.AreEqual(0, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_ResolveNonGeneric()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>(context => context.WithName("enumerable"));
            container.RegisterType<ITest2, Test22>(context => context.WithName("array"));

            container.Resolve<ITest2>("enumerable");
            container.Resolve<ITest2>("array");

            var all = container.Resolve<IEnumerable<ITest2>>();
            var all2 = (IEnumerable<ITest2>)container.ResolveAll(typeof(ITest2));
            var all3 = container.ResolveAll(typeof(ITest2));

            Assert.AreEqual(2, all.Count());
            Assert.AreEqual(2, all2.Count());
            Assert.AreEqual(2, all3.Count());
        }

        [TestMethod]
        public void EnumerableTests_ResolveNonGeneric_Scoped()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>(context => context.WithName("enumerable"));
            container.RegisterType<ITest2, Test22>(context => context.WithName("array"));

            var scope = container.BeginScope();

            scope.Resolve<ITest2>("enumerable");
            scope.Resolve<ITest2>("array");

            var all = scope.Resolve<IEnumerable<ITest2>>();
            var all2 = (IEnumerable<ITest2>)scope.ResolveAll(typeof(ITest2));
            var all3 = scope.ResolveAll(typeof(ITest2));

            Assert.AreEqual(2, all.Count());
            Assert.AreEqual(2, all2.Count());
            Assert.AreEqual(2, all3.Count());
        }

        [TestMethod]
        public void EnumerableTests_Parallel_Resolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>(context => context.WithName("enumerable"));
            container.RegisterType<ITest2, Test22>(context => context.WithName("array"));

            Parallel.For(0, 10000, (i) =>
            {
                container.Resolve<ITest2>("enumerable");
                container.Resolve<ITest2>("array");
                var all = container.Resolve<IEnumerable<ITest2>>();

                Assert.AreEqual(2, all.Count());
            });
        }

        [TestMethod]
        public void EnumerableTests_Parallel_Resolve_NonGeneric()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>(context => context.WithName("enumerable"));
            container.RegisterType<ITest2, Test22>(context => context.WithName("array"));

            Parallel.For(0, 10000, (i) =>
            {
                container.Resolve<ITest2>("enumerable");
                container.Resolve<ITest2>("array");

                var all = container.Resolve<IEnumerable<ITest2>>();
                var all2 = (IEnumerable<ITest2>)container.ResolveAll(typeof(ITest2));

                Assert.AreEqual(2, all2.Count());
                Assert.AreEqual(2, all.Count());
            });
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var services = container.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.IsInstanceOfType(services[0], typeof(Test1));
            Assert.IsInstanceOfType(services[1], typeof(Test11));
            Assert.IsInstanceOfType(services[2], typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_ResolveAll_PreserveOrder()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var services = container.ResolveAll<ITest1>().ToArray();

            Assert.IsInstanceOfType(services[0], typeof(Test1));
            Assert.IsInstanceOfType(services[1], typeof(Test11));
            Assert.IsInstanceOfType(services[2], typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Scoped()
        {
            IStashboxContainer container = new StashboxContainer(config =>
                config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));

            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.BeginScope();

            var services = child.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.IsInstanceOfType(services[0], typeof(Test1));
            Assert.IsInstanceOfType(services[1], typeof(Test11));
            Assert.IsInstanceOfType(services[2], typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Parent()
        {
            IStashboxContainer container = new StashboxContainer(config =>
                config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));

            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.CreateChildContainer();

            var services = child.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.IsInstanceOfType(services[0], typeof(Test1));
            Assert.IsInstanceOfType(services[1], typeof(Test11));
            Assert.IsInstanceOfType(services[2], typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Scoped_Lazy()
        {
            IStashboxContainer container = new StashboxContainer(config => 
                config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));

            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.BeginScope();

            var services = child.Resolve<IEnumerable<Lazy<ITest1>>>().ToArray();

            Assert.IsInstanceOfType(services[0].Value, typeof(Test1));
            Assert.IsInstanceOfType(services[1].Value, typeof(Test11));
            Assert.IsInstanceOfType(services[2].Value, typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Parent_Lazy()
        {
            IStashboxContainer container = new StashboxContainer(config =>
                config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));

            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.CreateChildContainer();

            var services = child.Resolve<IEnumerable<Lazy<ITest1>>>().ToArray();

            Assert.IsInstanceOfType(services[0].Value, typeof(Test1));
            Assert.IsInstanceOfType(services[1].Value, typeof(Test11));
            Assert.IsInstanceOfType(services[2].Value, typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Scoped_Func()
        {
            IStashboxContainer container = new StashboxContainer(config =>
                config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));

            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.BeginScope();

            var services = child.Resolve<IEnumerable<Func<ITest1>>>().ToArray();

            Assert.IsInstanceOfType(services[0](), typeof(Test1));
            Assert.IsInstanceOfType(services[1](), typeof(Test11));
            Assert.IsInstanceOfType(services[2](), typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Parent_Func()
        {
            IStashboxContainer container = new StashboxContainer(config =>
                config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));

            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var child = container.CreateChildContainer();

            var services = child.Resolve<IEnumerable<Func<ITest1>>>().ToArray();

            Assert.IsInstanceOfType(services[0](), typeof(Test1));
            Assert.IsInstanceOfType(services[1](), typeof(Test11));
            Assert.IsInstanceOfType(services[2](), typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Lazy()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var services = container.Resolve<IEnumerable<Lazy<ITest1>>>().ToArray();

            Assert.IsInstanceOfType(services[0].Value, typeof(Test1));
            Assert.IsInstanceOfType(services[1].Value, typeof(Test11));
            Assert.IsInstanceOfType(services[2].Value, typeof(Test12));
        }

        [TestMethod]
        public void EnumerableTests_Resolve_PreserveOrder_Func()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();

            var services = container.Resolve<IEnumerable<Func<ITest1>>>().ToArray();

            Assert.IsInstanceOfType(services[0](), typeof(Test1));
            Assert.IsInstanceOfType(services[1](), typeof(Test11));
            Assert.IsInstanceOfType(services[2](), typeof(Test12));
        }

        public interface ITest1 { }

        public interface ITest2 { }

        public class Test1 : ITest1
        { }

        public class Test11 : ITest1
        { }

        public class Test12 : ITest1
        { }

        public class Test2 : ITest2
        {
            public Test2(IEnumerable<ITest1> tests)
            {
                Shield.EnsureNotNull(tests, nameof(tests));
                Assert.AreEqual(3, tests.Count());
            }
        }

        public class Test22 : ITest2
        {
            public Test22(ITest1[] tests)
            {
                Shield.EnsureNotNull(tests, nameof(tests));
                Assert.AreEqual(3, tests.Count());
            }
        }
    }
}
