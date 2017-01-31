using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Overrides;
using Stashbox.Utils;
using System;
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
            container.PrepareType<ITest2, Test2>().WithInjectionParameters(new InjectionParameter { Value = new Test1 { Name = "fakeName" }, Name = "test1" }).Register();
            var inst2 = container.Resolve<ITest2>();

            Assert.IsInstanceOfType(inst2, typeof(Test2));
            Assert.AreEqual("fakeName", inst2.Name);

            container.RegisterType<ITest3, Test3>();

            var inst1 = container.Resolve<ITest1>();
            inst1.Name = "test1";
            container.Resolve<ITest3>();
            var inst3 = container.Resolve<ITest3>(overrides: new[] { new TypeOverride(typeof(ITest1), inst1), new TypeOverride(typeof(ITest2), inst2) });

            Assert.IsInstanceOfType(inst3, typeof(Test3));
            Assert.AreEqual("test1fakeNametest1", inst3.Name);
        }

        [TestMethod]
        public void OverrideTests_Resolve_Named()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest2, Test22>();
            var inst2 = container.Resolve<ITest2>(overrides: new[] { new NamedOverride("test1", new Test1 { Name = "test1" }) });

            Assert.IsInstanceOfType(inst2, typeof(Test22));
            Assert.AreEqual("test1", inst2.Name);
        }

        [TestMethod]
        public void OverrideTests_Resolve_Parallel()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.PrepareType<ITest2, Test2>().WithInjectionParameters(new InjectionParameter { Value = new Test1 { Name = "fakeName" }, Name = "test1" }).Register();
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
                Assert.AreEqual("test1fakeNametest1", inst3.Name);
            });
        }

        [TestMethod]
        public void OverrideTests_Resolve_Parallel_Lazy()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.PrepareType<ITest2, Test2>().WithInjectionParameters(new InjectionParameter { Value = new Test1 { Name = "fakeName" }, Name = "test1" }).Register();
            var inst2 = container.Resolve<Lazy<ITest2>>();

            Assert.IsInstanceOfType(inst2.Value, typeof(Test2));
            Assert.AreEqual("fakeName", inst2.Value.Name);

            container.RegisterType<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                var inst1 = container.Resolve<Lazy<ITest1>>();
                inst1.Value.Name = "test1";
                var inst3 = container.Resolve<Lazy<ITest3>>(overrides: new[] { new TypeOverride(typeof(ITest1), inst1.Value), new TypeOverride(typeof(ITest2), inst2.Value) });

                Assert.IsInstanceOfType(inst3.Value, typeof(Test3));
                Assert.AreEqual("test1fakeNametest1", inst3.Value.Name);
                Assert.IsTrue(inst3.Value.MethodInvoked);
            });
        }

        public interface ITest1 { string Name { get; set; } }

        public interface ITest2 { string Name { get; set; } }

        public interface ITest3 { string Name { get; set; } bool MethodInvoked { get; } }

        public class Test1 : ITest1
        {
            public string Name { get; set; }
        }

        public class Test12 : ITest1
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

        public class Test22 : ITest2
        {
            public string Name { get; set; }

            public Test22([Dependency("test1")]ITest1 test1)
            {
                this.Name = test1.Name;
            }
        }

        public class Test3 : ITest3
        {
            public string Name { get; set; }

            public bool MethodInvoked { get; set; }

            public Test3(ITest1 test1, ITest2 test2)
            {
                Name = test1.Name + test2.Name;
            }

            [InjectionMethod]
            public void Inject(ITest1 test)
            {
                Shield.EnsureNotNull(test, nameof(test));
                this.MethodInvoked = true;
                Name += test.Name;
            }
        }
    }
}
