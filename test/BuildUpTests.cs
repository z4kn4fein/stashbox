using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Utils;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class BuildUpTests
    {
        [TestMethod]
        public void BuildUpTests_BuildUp()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();

                var test1 = new Test1();
                container.WireUpAs<ITest1>(test1);

                var test2 = new Test2();
                var inst = container.BuildUp(test2);

                Assert.AreEqual(test2, inst);
                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test1);
                Assert.IsInstanceOfType(inst, typeof(Test2));
                Assert.IsInstanceOfType(inst.Test1, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test1.Test, typeof(Test));
            }
        }

        [TestMethod]
        public void BuildUpTests_BuildUp_Scoped()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest3, Test3>().Register<ITest, Test>();
                var test3 = new Test3();
                var inst = (Test3)container.BuildUp<ITest3>(test3);

                Assert.IsNotNull(inst.Test);
            }
        }

        [TestMethod]
        public void BuildUpTests_BuildUp_As_InterfaceType()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterScoped<ITest, Test>();

                var test1 = new Test1();
                using (var scope = container.BeginScope())
                {
                    scope.BuildUp(test1);
                    Assert.IsFalse(test1.Test.Disposed);
                }

                Assert.IsTrue(test1.Test.Disposed);

                using (var scope = container.BeginScope())
                {
                    scope.BuildUp(test1);
                    Assert.IsFalse(test1.Test.Disposed);
                }

                Assert.IsTrue(test1.Test.Disposed);
            }
        }

        public interface ITest : IDisposable { bool Disposed { get; } }

        public interface ITest1 { ITest Test { get; } }

        public interface ITest3 { }

        public class Test : ITest
        {
            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException("disposed");

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
        }

        public class Test1 : ITest1
        {
            [Dependency]
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init()
            {
                Shield.EnsureNotNull(this.Test, nameof(this.Test));
            }
        }

        public class Test2
        {
            [Dependency]
            public ITest1 Test1 { get; set; }
        }
        
        public class Test3 : ITest3
        {
            [Dependency]
            public ITest Test { get; set; }
        }
    }
}
