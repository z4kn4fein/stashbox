using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
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
            container.Register<ITest1, Test1>(context => context.WhenDependantIs<Test2>());
            container.Register<ITest1, Test11>();
            container.Register<ITest1, Test12>();
            container.Register<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test1));
            Assert.IsInstanceOfType(test2.test12, typeof(Test1));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_When_First()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>(context => context.When(type => type.ParentType == typeof(Test2)));
            container.Register<ITest1, Test11>();
            container.Register<ITest1, Test12>();
            container.Register<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test1));
            Assert.IsInstanceOfType(test2.test12, typeof(Test1));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_First_NonGeneric()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>(context => context.WhenDependantIs(typeof(Test2)));
            container.Register<ITest1, Test11>();
            container.Register<ITest1, Test12>();
            container.Register<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test1));
            Assert.IsInstanceOfType(test2.test12, typeof(Test1));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_Second()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>(context => context.WhenDependantIs<Test2>());
            container.Register<ITest1, Test12>();
            container.Register<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test11));
            Assert.IsInstanceOfType(test2.test12, typeof(Test11));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_Second_NonGeneric()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>(context => context.WhenDependantIs(typeof(Test2)));
            container.Register<ITest1, Test12>();
            container.Register<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test11));
            Assert.IsInstanceOfType(test2.test12, typeof(Test11));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_Third()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>();
            container.Register<ITest1, Test12>(context => context.WhenDependantIs<Test2>());
            container.Register<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test12));
            Assert.IsInstanceOfType(test2.test12, typeof(Test12));
        }

        [TestMethod]
        public void ConditionalTests_ParentTypeCondition_Third_NonGeneric()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>();
            container.Register<ITest1, Test12>(context => context.WhenDependantIs(typeof(Test2)));
            container.Register<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test2.test1, typeof(Test12));
            Assert.IsInstanceOfType(test2.test12, typeof(Test12));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_First()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>(context => context.WhenHas<TestConditionAttribute>());
            container.Register<ITest1, Test11>();
            container.Register<ITest1, Test12>(context => context.WhenHas<TestCondition2Attribute>());
            container.Register<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test1));
            Assert.IsInstanceOfType(test3.test12, typeof(Test12));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_First_NonGeneric()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>(context => context.WhenHas(typeof(TestConditionAttribute)));
            container.Register<ITest1, Test11>();
            container.Register<ITest1, Test12>(context => context.WhenHas(typeof(TestCondition2Attribute)));
            container.Register<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test1));
            Assert.IsInstanceOfType(test3.test12, typeof(Test12));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_Second()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>(context => context.WhenHas<TestCondition2Attribute>());
            container.Register<ITest1, Test12>(context => context.WhenHas<TestConditionAttribute>());
            container.Register<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test12));
            Assert.IsInstanceOfType(test3.test12, typeof(Test11));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_Second_NonGeneric()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>(context => context.WhenHas(typeof(TestCondition2Attribute)));
            container.Register<ITest1, Test12>(context => context.WhenHas(typeof(TestConditionAttribute)));
            container.Register<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test12));
            Assert.IsInstanceOfType(test3.test12, typeof(Test11));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_Third()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>(context => context.WhenHas<TestCondition2Attribute>());
            container.Register<ITest1, Test11>(context => context.WhenHas<TestConditionAttribute>());
            container.Register<ITest1, Test12>();
            container.Register<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test11));
            Assert.IsInstanceOfType(test3.test12, typeof(Test1));
        }

        [TestMethod]
        public void ConditionalTests_AttributeCondition_Third_NonGeneric()
        {
            var container = new StashboxContainer();
            container.Register<ITest1, Test1>(context => context.WhenHas(typeof(TestCondition2Attribute)));
            container.Register<ITest1, Test11>(context => context.WhenHas(typeof(TestConditionAttribute)));
            container.Register<ITest1, Test12>();
            container.Register<ITest2, Test3>();

            var test3 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(test3.test1, typeof(Test11));
            Assert.IsInstanceOfType(test3.test12, typeof(Test1));
        }

        interface ITest1 { }

        interface ITest2 { ITest1 test1 { get; set; } ITest1 test12 { get; set; } }

        class Test1 : ITest1
        { }

        class Test11 : ITest1
        { }

        class Test12 : ITest1
        { }

        class Test2 : ITest2
        {
            [Dependency]
            public ITest1 test1 { get; set; }
            public ITest1 test12 { get; set; }

            public Test2(ITest1 test12)
            {
                this.test12 = test12;
            }
        }

        class Test3 : ITest2
        {
            [Dependency, TestCondition]
            public ITest1 test1 { get; set; }

            public ITest1 test12 { get; set; }

            public Test3([TestCondition2]ITest1 test12)
            {
                this.test12 = test12;
            }
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
        class TestConditionAttribute : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
        class TestCondition2Attribute : Attribute
        {
        }
    }
}
