using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class ActivateTests
    {
        [TestMethod]
        public void ActivateTests_Full()
        {
            var inst = new StashboxContainer().Register<Test>().Activate<Test1>();

            Assert.IsNotNull(inst.Test);
            Assert.IsNotNull(inst.Test2);

            Assert.IsTrue(inst.InjectionMethodCalled);
        }

        [TestMethod]
        public void ActivateTests_Full_DepOverride()
        {
            var test = new Test();
            var inst = new StashboxContainer().Activate<Test1>(test);

            Assert.AreSame(test, inst.Test);
            Assert.AreSame(test, inst.Test2);

            Assert.IsTrue(inst.InjectionMethodCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ActivateTests_Fail()
        {
            new StashboxContainer().Activate<ITest>();
        }

        interface ITest { }

        class Test { }

        class Test1
        {
            public bool InjectionMethodCalled { get; set; }

            public Test Test { get; set; }

            [Dependency]
            public Test Test2 { get; set; }

            public Test1(Test test)
            {
                this.Test = test;
            }

            [InjectionMethod]
            public void InjectMethod()
            {
                this.InjectionMethodCalled = true;
            }
        }
    }
}
