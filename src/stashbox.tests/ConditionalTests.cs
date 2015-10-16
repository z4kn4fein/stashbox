using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.ContainerExtensions.PropertyInjection;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class ConditionalTests
    {
        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_First()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.PrepareType<ITest1, Test1>().WhenDependantIs<Test2>().Register();
            container.RegisterType<ITest1, Test11>();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test1));
            Assert.IsInstanceOfType(test2.test12, typeof(Test1));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_Second()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.RegisterType<ITest1, Test1>();
            container.PrepareType<ITest1, Test11>().WhenDependantIs<Test2>().Register();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test11));
            Assert.IsInstanceOfType(test2.test12, typeof(Test11));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_Third()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest1, Test11>();
            container.PrepareType<ITest1, Test12>().WhenDependantIs<Test2>().Register();
            container.RegisterType<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test12));
            Assert.IsInstanceOfType(test2.test12, typeof(Test12));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_First()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.PrepareType<ITest1, Test1>().WhenHas<TestConditionAttribute>().Register();
            container.RegisterType<ITest1, Test11>();
            container.PrepareType<ITest1, Test12>().WhenHas<TestCondition2Attribute>().Register();
            container.RegisterType<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test1));
            Assert.IsInstanceOfType(test3.test12, typeof(Test12));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_Second()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.RegisterType<ITest1, Test1>();
            container.PrepareType<ITest1, Test11>().WhenHas<TestCondition2Attribute>().Register();
            container.PrepareType<ITest1, Test12>().WhenHas<TestConditionAttribute>().Register();
            container.RegisterType<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test12));
            Assert.IsInstanceOfType(test3.test12, typeof(Test11));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_Third()
        {
            var container = new StashboxContainer();
            container.RegisterExtension(new PropertyInjectionExtension());
            container.PrepareType<ITest1, Test1>().WhenHas<TestCondition2Attribute>().Register();
            container.PrepareType<ITest1, Test11>().WhenHas<TestConditionAttribute>().Register();
            container.RegisterType<ITest1, Test12>();
            container.RegisterType<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test11));
            Assert.IsInstanceOfType(test3.test12, typeof(Test1));
        }

        public interface ITest1 { }

        public interface ITest2 { ITest1 test1 { get; set; } ITest1 test12 { get; set; } }

        public class Test1 : ITest1
        { }

        public class Test11 : ITest1
        { }

        public class Test12 : ITest1
        { }

        public class Test2 : ITest2
        {
            [InjectionProperty]
            public ITest1 test1 { get; set; }
            public ITest1 test12 { get; set; }

            public Test2(ITest1 test12)
            {
                this.test12 = test12;
            }
        }

        public class Test3 : ITest2
        {
            [InjectionProperty, TestCondition]
            public ITest1 test1 { get; set; }

            public ITest1 test12 { get; set; }

            public Test3([TestCondition2]ITest1 test12)
            {
                this.test12 = test12;
            }
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
        public class TestConditionAttribute : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
        public class TestCondition2Attribute : Attribute
        {
        }
    }
}
