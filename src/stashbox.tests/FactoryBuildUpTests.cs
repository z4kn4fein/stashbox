using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;

namespace Stashbox.Tests
{
    [TestClass]
    public class FactoryBuildUpTests
    {
        [TestMethod]
        public void FactoryBuildUpTests_DependencyResolve()
        {
            using (var container = new StashboxContainer())
            {
                container.PrepareType<ITest, Test>().WithFactory(() => new Test("test")).Register();
                container.RegisterType<ITest1, Test12>();

                var inst = container.Resolve<ITest1>();

                Assert.IsInstanceOfType(inst.Test, typeof(Test));
                Assert.AreEqual("test", inst.Test.Name);
            }
        }

        [TestMethod]
        public void FactoryBuildUpTests_DependencyResolve_ServiceUpdated()
        {
            using (var container = new StashboxContainer())
            {
                container.PrepareType<ITest, Test>().WithFactory(() => new Test("test")).Register();
                container.RegisterType<ITest2, Test2>();
                container.PrepareType<ITest, Test>().WithFactory(() => new Test("test1")).ReMap();
                var inst = container.Resolve<ITest2>();

                Assert.IsInstanceOfType(inst.Test, typeof(Test));
                Assert.AreEqual("test1", inst.Test.Name);
            }
        }

        [TestMethod]
        public void FactoryBuildUpTests_Resolve()
        {
            using (var container = new StashboxContainer())
            {
                container.PrepareType<ITest, Test>().WithFactory(() => new Test("test")).Register();
                container.RegisterType<ITest1, Test1>();

                var inst = container.Resolve<ITest1>();

                Assert.IsInstanceOfType(inst.Test, typeof(Test));
                Assert.AreEqual("test", inst.Test.Name);
            }
        }

        [TestMethod]
        public void FactoryBuildUpTests_Resolve_OneParam()
        {
            using (var container = new StashboxContainer())
            {
                container.PrepareType<ITest, Test>().WithFactory((a) => new Test((string)a)).Register();
                container.RegisterType<ITest1, Test1>();

                var inst = container.Resolve<ITest1>(factoryParameters: new[] { "test" });

                Assert.IsInstanceOfType(inst.Test, typeof(Test));
                Assert.AreEqual("test", inst.Test.Name);
            }
        }

        [TestMethod]
        public void FactoryBuildUpTests_Resolve_TwoParam()
        {
            using (var container = new StashboxContainer())
            {
                container.PrepareType<ITest, Test>().WithFactory((a, b) => new Test((string)a + (string)b)).Register();
                container.RegisterType<ITest1, Test1>();

                var inst = container.Resolve<ITest1>(factoryParameters: new[] { "test", "test1" });

                Assert.IsInstanceOfType(inst.Test, typeof(Test));
                Assert.AreEqual("testtest1", inst.Test.Name);
            }
        }

        [TestMethod]
        public void FactoryBuildUpTests_Resolve_ThreeParam()
        {
            using (var container = new StashboxContainer())
            {
                container.PrepareType<ITest, Test>().WithFactory((a, b, c) => new Test((string)a + (string)b + (string)c)).Register();
                container.RegisterType<ITest1, Test1>();

                var inst = container.Resolve<ITest1>(factoryParameters: new[] { "test", "test1", "test2" });

                Assert.IsInstanceOfType(inst.Test, typeof(Test));
                Assert.AreEqual("testtest1test2", inst.Test.Name);
            }
        }

        public interface ITest { string Name { get; } }

        public interface ITest1 { ITest Test { get; } }

        public interface ITest2 { ITest Test { get; } }

        public class Test : ITest
        {
            public string Name { get; }

            public Test(string name)
            {
                this.Name = name;
            }
        }

        public class Test2 : ITest2
        {
            [Dependency]
            public ITest Test { get; set; }
        }

        public class Test1 : ITest1
        {
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init(ITest test)
            {
                this.Test = test;
            }
        }

        public class Test12 : ITest1
        {
            public ITest Test { get; set; }

            public Test12(ITest test)
            {
                this.Test = test;
            }
        }
    }
}
