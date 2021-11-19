using Xunit;
using Stashbox.Attributes;

namespace Stashbox.Tests
{

    public class DependencyBindingTests
    {
        [Fact]
        public void DependencyBindingTests_Bind_To_The_Same_Type()
        {
            var inst = new StashboxContainer()
                .Register<ITest1, Test1>("test1")
                .Register<ITest1, Test11>("test2")
                .Register<ITest1, Test12>("test3")
                .Register<Test>(ctx => ctx.WithDependencyBinding(typeof(ITest1), "test2"))
                .Resolve<Test>();

            Assert.IsType<Test11>(inst.Test1);
            Assert.IsType<Test11>(inst.Test2);
            Assert.IsType<Test11>(inst.Test3);

        }

        [Fact]
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

            Assert.IsType<Test1>(inst.Test1);
            Assert.IsType<Test11>(inst.Test2);
            Assert.IsType<Test12>(inst.Test3);

        }

        [Fact]
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

            Assert.IsType<Test11>(inst.Test1);
            Assert.IsType<Test11>(inst.Test2);
            Assert.IsType<Test12>(inst.Test3);

        }

        [Fact]
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

            Assert.IsType<Test11>(inst.Test1);
            Assert.IsType<Test11>(inst.Test2);
            Assert.IsType<Test12>(inst.Test3);

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
