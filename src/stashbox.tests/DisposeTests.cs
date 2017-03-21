using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Infrastructure;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class DisposeTests
    {
        [TestMethod]
        public void DisposeTests_Singleton()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<Test3>();
                container.RegisterSingleton<ITest1, Test1>();
                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Instance()
        {
            ITest1 test = new Test1();
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<Test3>();
                container.RegisterInstance<ITest1>(test);
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_WireUp()
        {
            ITest1 test = new Test1();
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<Test3>();
                container.WireUp<ITest1>(test);
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.RegisterType<ITest2, Test2>();
                container.RegisterType<Test3>();
                container.RegisterType<ITest1, Test1>();
                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.RegisterScoped<ITest2, Test2>();
                container.RegisterScoped<Test3>();
                container.RegisterScoped<ITest1, Test1>();

                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();

                using (var child = container.BeginScope())
                {
                    test4 = child.Resolve<ITest1>();
                    test5 = child.Resolve<ITest2>();
                    test6 = child.Resolve<Test3>();
                }

                Assert.IsTrue(test4.Disposed);
                Assert.IsTrue(test5.Test1.Disposed);
                Assert.IsTrue(test6.Test1.Disposed);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.RegisterType<ITest2, Test2>();
                container.RegisterType<Test3>();
                container.RegisterType<ITest1, Test1>();

                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();

                using (var child = container.BeginScope())
                {
                    test4 = child.Resolve<ITest1>();
                    test5 = child.Resolve<ITest2>();
                    test6 = child.Resolve<Test3>();
                }

                Assert.IsTrue(test4.Disposed);
                Assert.IsTrue(test5.Test1.Disposed);
                Assert.IsTrue(test6.Test1.Disposed);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient_ChildContainer()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking());
            ITest1 test4;
            ITest2 test5;
            Test3 test6;

            container.RegisterType<ITest2, Test2>();
            container.RegisterType<Test3>();
            container.RegisterType<ITest1, Test1>();

            using (var child = container.BeginScope())
            {
                test4 = child.Resolve<ITest1>();
                test5 = child.Resolve<ITest2>();
                test6 = child.Resolve<Test3>();
            }

            Assert.IsTrue(test4.Disposed);
            Assert.IsTrue(test5.Test1.Disposed);
            Assert.IsTrue(test6.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient_Singleton()
        {
            var container = new StashboxContainer(config => config.WithDisposableTransientTracking());


            container.RegisterType<ITest2, Test2>();
            container.RegisterType<Test3>();
            container.RegisterSingleton<ITest1, Test1>();

            ITest1 test4;
            ITest2 test5;
            Test3 test6;

            using (var child = container.BeginScope())
            {
                test4 = child.Resolve<ITest1>();
                test5 = child.Resolve<ITest2>();
                test6 = child.Resolve<Test3>();

                Assert.IsFalse(test4.Disposed);
                Assert.IsFalse(test5.Test1.Disposed);
                Assert.IsFalse(test6.Test1.Disposed);
            }

            Assert.IsFalse(test4.Disposed);
            Assert.IsFalse(test5.Test1.Disposed);
            Assert.IsFalse(test6.Test1.Disposed);

            container.Dispose();

            Assert.IsTrue(test4.Disposed);
            Assert.IsTrue(test5.Test1.Disposed);
            Assert.IsTrue(test6.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Implementation_Has_Disposable()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking());
            ITest11 test1;

            container.RegisterType<ITest11, Test4>();

            using (var child = container.BeginScope())
            {
                test1 = child.Resolve<ITest11>();
            }

            Assert.IsTrue(((Test4)test1).Disposed);
        }

        [TestMethod]
        public void DisposeTests_Instance_TrackTransient()
        {
            ITest1 test = new Test1();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
                container.RegisterInstance(test);

            Assert.IsTrue(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_WireUp_TrackTransient()
        {
            ITest1 test = new Test1();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
                container.WireUp(test);

            Assert.IsTrue(test.Disposed);
        }

        public interface ITest11 { }

        public interface ITest12 { }

        public interface ITest1 : ITest11, ITest12, IDisposable { bool Disposed { get; } }

        public interface ITest2 { ITest1 Test1 { get; } }

        public class Test1 : ITest1
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                this.Disposed = true;
            }
        }

        public class Test2 : ITest2
        {
            public ITest1 Test1 { get; private set; }

            public Test2(ITest1 test1)
            {
                this.Test1 = test1;
            }
        }

        public class Test3
        {
            [Dependency]
            public ITest1 Test1 { get; set; }
        }

        public class Test4 : ITest11, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                this.Disposed = true;
            }
        }
    }
}