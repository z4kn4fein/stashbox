using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.ContainerExtensions.PropertyInjection;

namespace Stashbox.Tests
{
    [TestClass]
    public class InjectionPropertyTests
    {
        [TestMethod]
        public void InjectionPropertyTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.RegisterType<ITest, Test>();
            container.RegisterType<ITest1, Test1>();

            var inst = container.Resolve<ITest1>();

            Assert.IsNotNull(inst);
            Assert.IsNotNull(inst.Test);
            Assert.IsInstanceOfType(inst, typeof(Test1));
            Assert.IsInstanceOfType(inst.Test, typeof(Test));
        }

        public interface ITest { }

        public interface ITest1 { ITest Test { get; } }

        public class Test : ITest { }

        public class Test1 : ITest1
        {
            [InjectionProperty]
            public ITest Test { get; set; }
        }
    }
}
