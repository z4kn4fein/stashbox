using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;

namespace Stashbox.Tests
{
    [TestClass]
    public class DependencyBindingTests
    {
        [TestMethod]
        public void DependencyBindingTests_Bind_To_The_Same_Type()
        {
            var inst = new StashboxContainer()
                .Register<ITest1, Test1>("test1")
                .Register<ITest1, Test11>("test2")
                .Register<ITest1, Test12>("test3")
                .Register<Test>(ctx => ctx.WithDependencyBinding(typeof(ITest1), "test2"))
                .Resolve<Test>();

            Assert.IsInstanceOfType(inst.Test1, typeof(Test11));
            Assert.IsInstanceOfType(inst.Test2, typeof(Test11));
            Assert.IsInstanceOfType(inst.Test3, typeof(Test11));

        }

        [TestMethod]
        public void DependencyBindingTests_Bind_To_Different_Types()
        {
            var inst = new StashboxContainer()
                .Register<ITest1, Test1>("test1")
                .Register<ITest1, Test11>("test2")
                .Register<ITest1, Test12>("test3")
                .Register<Test>(ctx => ctx
                    .WithDependencyBinding("test1", "test1")
                    .WithDependencyBinding("test2", "test2")
                    .WithDependencyBinding("test3", "test3"))
                .Resolve<Test>();

            Assert.IsInstanceOfType(inst.Test1, typeof(Test1));
            Assert.IsInstanceOfType(inst.Test2, typeof(Test11));
            Assert.IsInstanceOfType(inst.Test3, typeof(Test12));

        }

        [TestMethod]
        public void DependencyBindingTests_Override_Typed_Bindings()
        {
            var inst = new StashboxContainer()
                .Register<ITest1, Test1>("test1")
                .Register<ITest1, Test11>("test2")
                .Register<ITest1, Test12>("test3")
                .Register<Test>(ctx => ctx
                    .WithDependencyBinding("test3", "test3")
                    .WithDependencyBinding<ITest1>("test2"))
                .Resolve<Test>();

            Assert.IsInstanceOfType(inst.Test1, typeof(Test11));
            Assert.IsInstanceOfType(inst.Test2, typeof(Test11));
            Assert.IsInstanceOfType(inst.Test3, typeof(Test12));

        }

        [TestMethod]
        public void DependencyBindingTests_Override_Typed_Bindings_Injection_Method()
        {
            var inst = new StashboxContainer()
                .Register<ITest1, Test1>("test1")
                .Register<ITest1, Test11>("test2")
                .Register<ITest1, Test12>("test3")
                .Register<TestMethodInjection>(ctx => ctx
                    .WithDependencyBinding("test3", "test3")
                    .WithDependencyBinding<ITest1>("test2"))
                .Resolve<TestMethodInjection>();

            Assert.IsInstanceOfType(inst.Test1, typeof(Test11));
            Assert.IsInstanceOfType(inst.Test2, typeof(Test11));
            Assert.IsInstanceOfType(inst.Test3, typeof(Test12));

        }

        interface ITest1 { }

        class Test1 : ITest1
        { }

        class Test11 : ITest1
        { }

        class Test12 : ITest1
        { }

        class Test
        {
            public ITest1 Test1 { get; }
            public ITest1 Test2 { get; }
            public ITest1 Test3 { get; }

            public Test(ITest1 test1, ITest1 test2, ITest1 test3)
            {
                this.Test1 = test1;
                this.Test2 = test2;
                this.Test3 = test3;
            }
        }

        class TestMethodInjection
        {
            public ITest1 Test1 { get; private set; }
            public ITest1 Test2 { get; private set; }
            public ITest1 Test3 { get; private set; }

            [InjectionMethod]
            public void Init(ITest1 test1, ITest1 test2, ITest1 test3)
            {
                this.Test1 = test1;
                this.Test2 = test2;
                this.Test3 = test3;
            }
        }
    }
}
