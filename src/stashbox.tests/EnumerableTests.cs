using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Tests
{
    [TestClass]
    public class EnumerableTests
    {
        [TestMethod]
        public void EnumerableTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test>();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            var inst = container.Resolve<ITest2>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Test2));
            Assert.IsNotNull(inst.Tests);
            Assert.IsNotNull(inst.Tests1);

            var instances = container.ResolveAll<ITest1>();
            Assert.IsNotNull(instances);
            Assert.AreEqual(2, instances.Count());

        }

        public interface ITest1 { }
        public interface ITest2
        {
            IEnumerable<ITest1> Tests { get; }
            ITest1[] Tests1 { get; }
        }

        public class Test : ITest1 { }
        public class Test1 : ITest1 { }

        public class Test2 : ITest2
        {
            public Test2(IEnumerable<ITest1> tests, ITest1[] tests1)
            {
                this.Tests = tests;
                this.Tests1 = tests1;
            }

            public IEnumerable<ITest1> Tests { get; }
            public ITest1[] Tests1 { get; }
        }
    }
}
