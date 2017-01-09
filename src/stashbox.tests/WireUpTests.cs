using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ronin.Common;
using Stashbox.Attributes;

namespace Stashbox.Tests
{
    [TestClass]
    public class WireUpTests
    {
        [TestMethod]
        public void InjectionMemberTests_WireUp()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>();

                var test1 = new Test1();
                container.WireUp<ITest1>(test1);

                container.RegisterType<Test2>();

                var inst = container.Resolve<Test2>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test1);
                Assert.IsInstanceOfType(inst, typeof(Test2));
                Assert.IsInstanceOfType(inst.Test1, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test1.Test, typeof(Test));
            }
        }

        [TestMethod]
        public void InjectionMemberTests_WireUp_ServiceUpdated()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>();

                var test1 = new Test1();
                container.WireUp<ITest1>(test1);

                container.ReMap<ITest, Test>();

                container.RegisterType<Test2>();

                var inst = container.Resolve<Test2>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test1);
                Assert.IsInstanceOfType(inst, typeof(Test2));
                Assert.IsInstanceOfType(inst.Test1, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test1.Test, typeof(Test));
            }
        }

        [TestMethod]
        public void InjectionMemberTests_WireUp_WithoutService()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>();
                var test1 = new Test1();
                container.WireUp(test1);
                var inst = container.Resolve<Test1>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test);
                Assert.IsInstanceOfType(inst, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test, typeof(Test));
            }
        }

        public interface ITest { }

        public interface ITest1 { ITest Test { get; } }

        public class Test : ITest { }

        public class Test1 : ITest1
        {
            [Dependency]
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init()
            {
                Shield.EnsureNotNull(Test, nameof(Test));
            }
        }

        public class Test2
        {
            public ITest1 Test1 { get; set; }

            public Test2(ITest1 test1)
            {
                this.Test1 = test1;
            }
        }
    }
}
