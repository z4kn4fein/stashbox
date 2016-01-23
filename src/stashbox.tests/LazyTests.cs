using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class LazyTests
    {
        [TestMethod]
        public void LazyTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            var inst = container.Resolve<Lazy<ITest>>();

            Assert.IsNotNull(inst);
            Assert.IsFalse(inst.IsValueCreated);
            Assert.IsInstanceOfType(inst, typeof(Lazy<ITest>));
            Assert.IsInstanceOfType(inst.Value, typeof(Test));
        }

        [TestMethod]
        public void LazyTests_Resolve_ConstructorDependency()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.IsNotNull(inst.Test);
            Assert.IsFalse(inst.Test.IsValueCreated);
            Assert.IsInstanceOfType(inst.Test, typeof(Lazy<ITest>));
            Assert.IsInstanceOfType(inst.Test.Value, typeof(Test));
        }

        public interface ITest
        { }

        public class Test : ITest
        { }

        public class Test2
        {
            public Lazy<ITest> Test { get; private set; }

            public Test2(Lazy<ITest> test)
            {
                this.Test = test;
            }
        }
    }
}
