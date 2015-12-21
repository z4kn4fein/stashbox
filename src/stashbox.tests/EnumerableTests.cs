using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ronin.Common;
using Stashbox.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void EnumerableTests_Resolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>("enumerable");
            container.RegisterType<ITest2, Test22>("array");

            var enumerable = container.Resolve<ITest2>("enumerable");
            var array = container.Resolve<ITest2>("array");

            var all = container.Resolve<IEnumerable<ITest2>>();

            Assert.AreEqual(2, all.Count());
        }

        [TestMethod]
        public void EnumerableTests_Parallel_Resolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>("enumerable");
            container.RegisterType<ITest2, Test22>("array");

            Parallel.For(0, 50000, (i) =>
            {
                var enumerable = container.Resolve<ITest2>("enumerable");
                var array = container.Resolve<ITest2>("array");

                var all = container.Resolve<IEnumerable<ITest2>>();

                Assert.AreEqual(2, all.Count());
            });
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
