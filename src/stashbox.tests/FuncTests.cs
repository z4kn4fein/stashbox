using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class FuncTests
    {
        [TestMethod]
        public void FuncTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            var inst = container.Resolve<Func<ITest>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Func<ITest>));
            Assert.IsInstanceOfType(inst(), typeof(Test));
        }

        [TestMethod]
        public void FuncTests_Resolve_ConstructorDependency()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.IsNotNull(inst.Test);
            Assert.IsInstanceOfType(inst.Test, typeof(Func<ITest>));
            Assert.IsInstanceOfType(inst.Test(), typeof(Test));
        }

        public interface ITest
        { }

        public class Test : ITest
        { }

        public class Test2
        {
            public Func<ITest> Test { get; private set; }

            public Test2(Func<ITest> test)
            {
                this.Test = test;
            }
        }
    }
}
