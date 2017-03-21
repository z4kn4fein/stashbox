using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using Stashbox.Utils;
using System;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class LifetimeTests
    {
        [TestMethod]
        public void LifetimeTests_Resolve()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterSingleton<ITest1, Test1>();
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<ITest3, Test3>();

                var test1 = container.Resolve<ITest1>();
                test1.Name = "test1";
                var test2 = container.Resolve<ITest2>();
                var test3 = container.Resolve<ITest3>();

                Assert.IsNotNull(test1);
                Assert.IsNotNull(test2);
                Assert.IsNotNull(test3);
            }
        }

        [TestMethod]
        public void LifetimeTests_Resolve_Parallel()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterSingleton(typeof(ITest1), typeof(Test1));
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<ITest3, Test3>();

                Parallel.For(0, 50000, (i) =>
                {
                    var test1 = container.Resolve<ITest1>();
                    test1.Name = "test1";
                    var test2 = container.Resolve<ITest2>();
                    var test3 = container.Resolve<ITest3>();

                    Assert.IsNotNull(test1);
                    Assert.IsNotNull(test2);
                    Assert.IsNotNull(test3);
                });
            }
        }

        [TestMethod]
        public void LifetimeTests_Resolve_Parallel_Lazy()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.PrepareType<ITest1, Test1>().WithLifetime(new SingletonLifetime()).Register();
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<ITest3, Test3>();

                Parallel.For(0, 50000, (i) =>
                {
                    var test1 = container.Resolve<Lazy<ITest1>>();
                    test1.Value.Name = "test1";
                    var test2 = container.Resolve<Lazy<ITest2>>();
                    var test3 = container.Resolve<Lazy<ITest3>>();

                    Assert.IsNotNull(test1.Value);
                    Assert.IsNotNull(test2.Value);
                    Assert.IsNotNull(test3.Value);
                });
            }
        }

        [TestMethod]
        public void LifetimeTests_StateCheck()
        {
            var scoped = new ScopedLifetime();
            Assert.IsTrue(scoped.HandlesObjectDisposal);
            Assert.IsInstanceOfType(scoped.Create(), typeof(ScopedLifetime));

            var singleton = new SingletonLifetime();
            Assert.IsTrue(singleton.HandlesObjectDisposal);
            Assert.IsInstanceOfType(singleton.Create(), typeof(SingletonLifetime));
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
                Shield.EnsureNotNullOrEmpty(test1.Name, nameof(test1.Name));
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
                Shield.EnsureNotNullOrEmpty(test1.Name, nameof(test1.Name));

                Shield.EnsureTypeOf<Test1>(test1);
                Shield.EnsureTypeOf<Test2>(test2);
            }
        }
    }
}
