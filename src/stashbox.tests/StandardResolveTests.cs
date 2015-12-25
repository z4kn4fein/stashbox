using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ronin.Common;
using Stashbox.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class StandardResolveTests
    {
        [TestMethod]
        public void StandardResolveTests_Resolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();
            container.RegisterType<ITest3, Test3>();

            var test3 = container.Resolve<ITest3>();
            var test2 = container.Resolve<ITest2>();
            var test1 = container.Resolve<ITest1>();

            Assert.IsNotNull(test3);
            Assert.IsNotNull(test2);
            Assert.IsNotNull(test1);

            Assert.IsInstanceOfType(test1, typeof(Test1));
            Assert.IsInstanceOfType(test2, typeof(Test2));
            Assert.IsInstanceOfType(test3, typeof(Test3));
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Parallel()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();
            container.RegisterType<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                if (i % 100 == 0)
                {
                    container.RegisterType<ITest1, Test1>(i.ToString());
                    container.RegisterType<ITest3, Test3>($"ITest3{i.ToString()}");
                    var test33 = container.Resolve<ITest3>($"ITest3{i.ToString()}");
                    var test11 = container.Resolve<ITest1>(i.ToString());
                    Assert.IsNotNull(test33);
                    Assert.IsNotNull(test11);

                    Assert.IsInstanceOfType(test11, typeof(Test1));
                    Assert.IsInstanceOfType(test33, typeof(Test3));
                }

                var test3 = container.Resolve<ITest3>();
                var test2 = container.Resolve<ITest2>();
                var test1 = container.Resolve<ITest1>();

                Assert.IsNotNull(test3);
                Assert.IsNotNull(test2);
                Assert.IsNotNull(test1);

                Assert.IsInstanceOfType(test1, typeof(Test1));
                Assert.IsInstanceOfType(test2, typeof(Test2));
                Assert.IsInstanceOfType(test3, typeof(Test3));
            });
        }

        [TestMethod]
        public void StandardResolveTests_Resolve_Parallel_Lazy()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();
            container.RegisterType<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                if (i % 100 == 0)
                {
                    container.RegisterType<ITest1, Test1>();
                    container.RegisterType<ITest3, Test3>();
                }

                var test3 = container.Resolve<Lazy<ITest3>>();
                var test2 = container.Resolve<Lazy<ITest2>>();
                var test1 = container.Resolve<Lazy<ITest1>>();

                Assert.IsNotNull(test3.Value);
                Assert.IsNotNull(test2.Value);
                Assert.IsNotNull(test1.Value);
            });
        }

        [TestMethod]
        public void StandardResolveTests_ChildContainer()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            var child = container.CreateChildContainer();
            child.RegisterType<ITest3, Test3>();

            var test3 = child.Resolve<ITest3>();

            Assert.IsNotNull(test3);
            Assert.IsInstanceOfType(test3, typeof(Test3));
        }

        public interface ITest1 { string Name { get; set; } }

        public interface ITest2 { string Name { get; set; } }

        public interface ITest3 { string Name { get; set; } }

        public class Test1 : ITest1
        {
            public string Name { get; set; }
        }

        public class Test2 : ITest2
        {
            public string Name { get; set; }

            public Test2(ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test1>(test1);
            }
        }

        public class Test3 : ITest3
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
    }
}