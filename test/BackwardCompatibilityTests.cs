using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests
{
    [TestClass]
    public class BackwardCompatibilityTests
    {
#pragma warning disable 0618 // disable obsolate warning

        [TestMethod]
        public void RegisterType_Generic()
        {
            var container = new StashboxContainer();
            var inst = container.RegisterType<ITest, Test>().Resolve<ITest>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Test));
        }

        [TestMethod]
        public void RegisterType_TFrom_Generic()
        {
            var container = new StashboxContainer();
            var inst = container.RegisterType<ITest>(typeof(Test)).Resolve<ITest>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Test));
        }

        [TestMethod]
        public void RegisterType_Simple_Types()
        {
            var container = new StashboxContainer();
            var inst = container.RegisterType(typeof(ITest), typeof(Test)).Resolve<ITest>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Test));
        }

        [TestMethod]
        public void RegisterType_Just_TTo()
        {
            var container = new StashboxContainer();
            var inst = container.RegisterType<Test>().Resolve<Test>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Test));
        }

        [TestMethod]
        public void RegisterType_Just_To_Simple_Type()
        {
            var container = new StashboxContainer();
            var inst = container.RegisterType(typeof(Test)).Resolve<Test>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Test));
        }

#pragma warning restore 0618 // restore obsolate warning

        private interface ITest { }

        private class Test : ITest { }
    }
}
