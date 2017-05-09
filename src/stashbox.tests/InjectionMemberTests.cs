using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Entity;

namespace Stashbox.Tests
{
    [TestClass]
    public class InjectionMemberTests
    {
        [TestMethod]
        public void InjectionMemberTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<ITest1, Test1>();

            var inst = container.Resolve<ITest1>();

            Assert.IsNotNull(inst);
            Assert.IsNotNull(inst.Test);
            Assert.IsInstanceOfType(inst, typeof(Test1));
            Assert.IsInstanceOfType(inst.Test, typeof(Test));

            Assert.IsNotNull(((Test1)inst).TestFieldProperty);
            Assert.IsInstanceOfType(((Test1)inst).TestFieldProperty, typeof(Test));
        }

        [TestMethod]
        public void InjectionMemberTests_BuildUp()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest, Test>();

                var test1 = new Test1();
                container.WireUpAs<ITest1>(test1);

                var inst = container.Resolve<ITest1>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test);
                Assert.IsInstanceOfType(inst, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test, typeof(Test));

                Assert.IsNotNull(((Test1)inst).TestFieldProperty);
                Assert.IsInstanceOfType(((Test1)inst).TestFieldProperty, typeof(Test));
            }
        }

        [TestMethod]
        public void InjectionMemberTests_Resolve_InjectionParameter()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest2, Test2>(context => context.WithInjectionParameters(new InjectionParameter { Name = "Name", Value = "test" }));

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
            [Dependency]
            private ITest testField = null;

            public ITest TestFieldProperty => this.testField;

            [Dependency]
            public ITest Test { get; set; }
        }

        public interface ITest2 { string Name { get; set; } }

        public class Test2 : ITest2
        {
            [Dependency]
            public string Name { get; set; }
        }
    }
}
