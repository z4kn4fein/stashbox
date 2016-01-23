using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests
{
    [TestClass]
    public class InstanceBuilderTests
    {
        [TestMethod]
        public void InstanceBuilderTests_Resolve()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                container.RegisterInstance<ITest>(dep);
                var inst = container.Resolve<ITest>();

                Assert.AreSame(inst, dep);
            }
        }

        [TestMethod]
        public void InstanceBuilderTests_DependencyResolve()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                container.RegisterInstance<ITest>(dep);
                container.RegisterType<ITest1, Test1>();

                var inst = container.Resolve<ITest1>();

                Assert.AreSame(inst.Test, dep);
            }
        }

        public interface ITest { }

        public interface ITest1 { ITest Test { get; } }

        public class Test : ITest { }

        public class Test1 : ITest1
        {
            public ITest Test { get; }
            public Test1(ITest test)
            {
                this.Test = test;
            }
        }
    }
}
