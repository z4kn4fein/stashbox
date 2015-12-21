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

        public interface ITest
        { }

        public class Test : ITest
        { }
    }
}
