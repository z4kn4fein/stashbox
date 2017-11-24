using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Entity;
using System;

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
        public void InjectionMemberTests_Resolve_WithoutRegistered()
        {
            var container = new StashboxContainer();
            var test1 = new Test1();
            container.WireUpAs<ITest1>(test1);

            var inst = container.Resolve<ITest1>();

            Assert.IsNotNull(inst);
            Assert.IsNull(inst.Test);
            Assert.IsNull(((Test1)inst).TestFieldProperty);
        }

        [TestMethod]
        public void InjectionMemberTests_WireUp()
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

        [TestMethod]
        public void InjectionMemberTests_Inject_With_Config()
        {
            var container = new StashboxContainer();
            container.RegisterType<Test3>(context => context.InjectMember("Test1", "test1").InjectMember("test2", "test2"))
                .RegisterType<ITest, TestM1>(context => context.WithName("test1"))
                .RegisterType<ITest, TestM2>(context => context.WithName("test2"));

            var inst = container.Resolve<Test3>();

            Assert.IsNotNull(inst.Test1);
            Assert.IsNotNull(inst.Test2);
            Assert.IsInstanceOfType(inst.Test1, typeof(TestM1));
            Assert.IsInstanceOfType(inst.Test2, typeof(TestM2));
        }

        [TestMethod]
        public void InjectionMemberTests_Inject_With_Invalid_Config()
        {
            var container = new StashboxContainer();
            container.RegisterType<Test3>(context => context.InjectMember("Test3"));

            var inst = container.Resolve<Test3>();

            Assert.IsNull(inst.Test1);
            Assert.IsNull(inst.Test2);
        }

        [TestMethod]
        public void InjectionMemberTests_Inject_With_Config_Generic()
        {
            var container = new StashboxContainer();
            container.RegisterType<Test3>(context => context.InjectMember(x => x.Test1, "test1").InjectMember(x => x.Test2, "test2"))
                .RegisterType<ITest, TestM1>(context => context.WithName("test1"))
                .RegisterType<ITest, TestM2>(context => context.WithName("test2"));

            var inst = container.Resolve<Test3>();

            Assert.IsNotNull(inst.Test1);
            Assert.IsNotNull(inst.Test2);
            Assert.IsInstanceOfType(inst.Test1, typeof(TestM1));
            Assert.IsInstanceOfType(inst.Test2, typeof(TestM2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InjectionMemberTests_Inject_With_Config_Generic_Throws()
        {
            var container = new StashboxContainer();
            container.RegisterType<Test3>(context => context.InjectMember(x => 50));
        }

        public interface ITest { }

        public interface ITest1 { ITest Test { get; } }

        public class Test : ITest { }

        public class TestM1 : ITest { }

        public class TestM2 : ITest { }

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

        public class Test3
        {
            public ITest Test1 { get; set; }

            private ITest test2 = null;

            public ITest Test2 => this.test2;

            public void Test() { }
        }
    }
}
