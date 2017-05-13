using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests
{
    [TestClass]
    public class FinalizerTests
    {
        [TestMethod]
        public void FinalizerTests_RegisterType()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>(context => context.WithFinalizer(t => t.CleanUp()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_RegisterType_ByInterface()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest>(typeof(Test), context => context.WithFinalizer(t => t.CleanUp()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_RegisterType_ByImplementation()
        {
            Test test;
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>(context => context.WithFinalizer(t => t.CleanUp()));
                test = container.Resolve<Test>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_ReMap()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>(context => context.WithFinalizer(t => t.CleanUp()));
                container.ReMap<ITest, Test>(context => context.WithFinalizer(t => t.CleanUp()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_ReMap_ByInterface()
        {
            ITest test;
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest>(typeof(Test), context => context.WithFinalizer(t => t.CleanUp()));
                container.ReMap<ITest>(typeof(Test), context => context.WithFinalizer(t => t.CleanUp()));
                test = container.Resolve<ITest>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_ReMap_ByImplementation()
        {
            Test test;
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>(context => context.WithFinalizer(t => t.CleanUp()));
                container.ReMap<Test>(context => context.WithFinalizer(t => t.CleanUp()));
                test = container.Resolve<Test>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_Instance_Interface()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstanceAs<ITest>(test, finalizerDelegate: t => t.CleanUp());
                test = (Test)container.Resolve<ITest>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_Instance_Implementation()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstanceAs(test, finalizerDelegate: t => t.CleanUp());
                test = container.Resolve<Test>();
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        [TestMethod]
        public void FinalizerTests_RegisterType_Multiple_Shouldnt_Throw()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>(context => context.WithFinalizer(t => t.CleanUp()));
                for (var i = 0; i < 10; i++)
                {
                    var test = container.Resolve<ITest>();
                    Assert.IsFalse(test.CleanupCalled);
                }
            }
        }

        [TestMethod]
        public void FinalizerTests_RegisterType_Singleton_Multiple_Shouldnt_Throw()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>(context => context.WithFinalizer(t => t.CleanUp()).WithSingletonLifetime());
                for (var i = 0; i < 10; i++)
                {
                    var test = container.Resolve<ITest>();
                    Assert.IsFalse(test.CleanupCalled);
                }
            }
        }

        [TestMethod]
        public void FinalizerTests_RegisterType_Scoped_Multiple_Shouldnt_Throw()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>(context => context.WithFinalizer(t => t.CleanUp()).WithScopedLifetime());
                for (var i = 0; i < 10; i++)
                {
                    ITest test;
                    using (var scope = container.BeginScope())
                    {
                        test = scope.Resolve<ITest>();
                        Assert.IsFalse(test.CleanupCalled);
                    }

                    Assert.IsTrue(test.CleanupCalled);
                }
            }
        }

        [TestMethod]
        public void FinalizerTests_Instance_Implementation_Multiple_Shouldnt_Throw()
        {
            var test = new Test();
            using (var container = new StashboxContainer())
            {
                container.RegisterInstanceAs(test, finalizerDelegate: t => t.CleanUp());
                for (var i = 0; i < 10; i++)
                {
                    var test1 = container.Resolve<Test>();
                    Assert.IsFalse(test1.CleanupCalled);
                    Assert.AreSame(test, test1);
                }
            }

            Assert.IsTrue(test.CleanupCalled);
        }

        public interface ITest
        {
            bool CleanupCalled { get; }

            void CleanUp();
        }

        public class Test : ITest
        {
            public void CleanUp()
            {
                if (this.CleanupCalled)
                    throw new Exception("CleanUp called multiple times!");

                this.CleanupCalled = true;
            }

            public bool CleanupCalled { get; private set; }
        }
    }
}
