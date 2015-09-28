using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.ContainerExtensions.PropertyInjection;
using Stashbox.Entity;

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

        [TestMethod]
        public void InjectionPropertyTests_Resolve_InjectionParameter()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.PrepareType<ITest2, Test2>().WithInjectionParameters(new InjectionParameter { Name = "Name", Value = "test" }).Register();

            var inst = container.Resolve<ITest2>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Test2));
            Assert.AreEqual("test", inst.Name);
        }

        public interface ITest { }

        public interface ITest1 { ITest Test { get; } }

        public class Test : ITest { }

        public class Test1 : ITest1
        {
            [InjectionProperty]
            public ITest Test { get; set; }
        }

        public interface ITest2 { string Name { get; set; } }

        public class Test2 : ITest2
        {
            [InjectionProperty]
            public string Name { get; set; }
        }
    }
}
