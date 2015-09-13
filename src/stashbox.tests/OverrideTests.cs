using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Overrides;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class OverrideTests
    {
        [TestMethod]
        public void OverrideTests_Resolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>(injectionParameters: new[] { new InjectionParameter { Value = new Test1 { Name = "fakeName" }, Name = "test1" } });
            var inst2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(inst2, typeof(Test2));
            Assert.AreEqual("fakeName", inst2.Name);

            container.RegisterType<ITest3, Test3>();

            var inst1 = container.Resolve<ITest1>();
            inst1.Name = "test1";
            var inst3 = container.Resolve<ITest3>(overrides: new[] { new TypeOverride(typeof(ITest1), inst1), new TypeOverride(typeof(ITest2), inst2) });

            Assert.IsInstanceOfType(inst3, typeof(Test3));
            Assert.AreEqual("test1fakeName", inst3.Name);
        }

        [TestMethod]
        public void OverrideTests_Resolve_Parallel()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>(injectionParameters: new[] { new InjectionParameter { Value = new Test1 { Name = "fakeName" }, Name = "test1" } });
            var inst2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(inst2, typeof(Test2));
            Assert.AreEqual("fakeName", inst2.Name);

            container.RegisterType<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                var inst1 = container.Resolve<ITest1>();
                inst1.Name = "test1";
                var inst3 = container.Resolve<ITest3>(overrides: new[] { new TypeOverride(typeof(ITest1), inst1), new TypeOverride(typeof(ITest2), inst2) });

                Assert.IsInstanceOfType(inst3, typeof(Test3));
                Assert.AreEqual("test1fakeName", inst3.Name);
            });
        }

        public interface ITest1 { string Name { get; set; } }

        public interface ITest2 { string Name { get; set; } }

        public interface ITest3 { string Name { get; set; } }

        public class Test1 : ITest1
        {
            public string Name { get; set; }
        }

        public class Test2 : ITest2
        {
            public string Name { get; set; }

            public Test2(ITest1 test1)
            {
                this.Name = test1.Name;
            }
        }

        public class Test3 : ITest3
        {
            public string Name { get; set; }

            public Test3(ITest1 test1, ITest2 test2)
            {
                Name = test1.Name + test2.Name;
            }
        }
    }
}
