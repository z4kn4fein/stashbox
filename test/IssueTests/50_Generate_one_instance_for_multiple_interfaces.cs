using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class GenerateOneInstanceForMultipleInterfaces
    {
        [TestMethod]
        public void Generate_one_instance_for_multiple_interfaces()
        {
            var container = new StashboxContainer();
            container.Register<Test2>(context => context.AsImplementedTypes());
            var inst1 = container.Resolve<ITest>();
            var inst2 = container.Resolve<ITest1>();
            var inst3 = container.Resolve<ITest2>();
            var inst4 = container.Resolve<Test>();
            var inst5 = container.Resolve<Test2>();

            Assert.AreNotSame(inst1, inst2);
            Assert.AreNotSame(inst2, inst3);
            Assert.AreNotSame(inst3, inst4);
            Assert.AreNotSame(inst4, inst5);
        }

        [TestMethod]
        public void Generate_one_instance_for_multiple_interfaces_singleton()
        {
            var container = new StashboxContainer();
            container.Register<Test2>(context => context.AsImplementedTypes().WithSingletonLifetime());
            var inst1 = container.Resolve<ITest>();
            var inst2 = container.Resolve<ITest1>();
            var inst3 = container.Resolve<ITest2>();
            var inst4 = container.Resolve<Test>();
            var inst5 = container.Resolve<Test2>();

            Assert.AreSame(inst1, inst2);
            Assert.AreSame(inst2, inst3);
            Assert.AreSame(inst3, inst4);
            Assert.AreSame(inst4, inst5);
        }

        [TestMethod]
        public void Generate_one_instance_for_multiple_interfaces_scoped()
        {
            var container = new StashboxContainer();
            container.Register<Test2>(context => context.AsImplementedTypes().WithScopedLifetime());

            var scope = container.BeginScope();
            var inst1 = scope.Resolve<ITest>();
            var inst2 = scope.Resolve<ITest1>();
            var inst3 = scope.Resolve<ITest2>();
            var inst4 = scope.Resolve<Test>();
            var inst5 = scope.Resolve<Test2>();

            Assert.AreSame(inst1, inst2);
            Assert.AreSame(inst2, inst3);
            Assert.AreSame(inst3, inst4);
            Assert.AreSame(inst4, inst5);
        }

        [TestMethod]
        public void Generate_one_instance_for_multiple_interfaces_named_scope()
        {
            var container = new StashboxContainer();
            container.Register<Test2>(context => context.AsImplementedTypes().InNamedScope("A"));

            var scope = container.BeginScope("A");
            var inst1 = scope.Resolve<ITest>();
            var inst2 = scope.Resolve<ITest1>();
            var inst3 = scope.Resolve<ITest2>();
            var inst4 = scope.Resolve<Test>();
            var inst5 = scope.Resolve<Test2>();

            Assert.AreSame(inst1, inst2);
            Assert.AreSame(inst2, inst3);
            Assert.AreSame(inst3, inst4);
            Assert.AreSame(inst4, inst5);
        }

        interface ITest { }

        interface ITest1 { }

        interface ITest2 { }

        class Test { }

        class Test2 : Test, ITest, ITest1, ITest2 { }
    }
}
