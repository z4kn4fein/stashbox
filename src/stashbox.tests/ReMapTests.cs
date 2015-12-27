using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Infrastructure;

namespace Stashbox.Tests
{
    [TestClass]
    public class ReMapTests
    {
        [TestMethod]
        public void ReMapTests_Resolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();

            var test1 = container.Resolve<ITest1>();

            Assert.IsNotNull(test1);
            Assert.IsInstanceOfType(test1, typeof(Test1));

            container.ReMap<ITest1, Test11>();

            var test11 = container.Resolve<ITest1>();

            Assert.IsNotNull(test11);
            Assert.IsInstanceOfType(test11, typeof(Test11));
        }

        public interface ITest1 { }

        public class Test1 : ITest1
        { }

        public class Test11 : ITest1
        { }
    }
}
