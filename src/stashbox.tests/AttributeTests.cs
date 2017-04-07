using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Utils;
using System;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class AttributeTest
    {
        [TestMethod]
        public void AttributeTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>("test1");
            container.RegisterType<ITest1, Test11>("test11");
            container.RegisterType<ITest1, Test12>("test12");
            container.RegisterType<ITest2, Test2>("test2");
            container.RegisterType<ITest2, Test22>(context => context.WithName("test22"));
            container.RegisterType<ITest3, Test3>();
            container.RegisterType<ITest4, Test4>();

            var test1 = container.Resolve<ITest1>();
            var test2 = container.Resolve<ITest2>("test2");
            var test3 = container.Resolve<ITest3>();
            var test4 = container.Resolve<Lazy<ITest4>>();

            Assert.IsNotNull(test1);
            Assert.IsNotNull(test2);
            Assert.IsNotNull(test3);
            Assert.IsNotNull(test4.Value);

            Assert.IsTrue(test3.MethodInvoked);
            Assert.IsTrue(test3.MethodInvoked2);

            Assert.IsInstanceOfType(test3.test1, typeof(Test11));
            Assert.IsInstanceOfType(test3.test2, typeof(Test22));
        }
        
        [TestMethod]
        public void AttributeTests_Named_Resolution()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>("test1");
            container.RegisterType<ITest1, Test11>("test11");
            container.RegisterType<ITest1, Test12>("test12");
            container.RegisterType<ITest2, Test2>("test2");
            container.RegisterType<ITest2, Test22>(context => context.WithName("test22"));

            var test1 = container.Resolve<ITest1>("test1");
            var test11 = container.Resolve<ITest1>("test11");
            var test12 = container.Resolve<ITest1>("test12");
            var test2 = container.Resolve<ITest2>("test2");
            var test22 = container.Resolve<ITest2>("test22");

            Assert.IsInstanceOfType(test1, typeof(Test1));
            Assert.IsInstanceOfType(test11, typeof(Test11));
            Assert.IsInstanceOfType(test12, typeof(Test12));
            Assert.IsInstanceOfType(test2, typeof(Test2));
            Assert.IsInstanceOfType(test22, typeof(Test22));
        }

        [TestMethod]
        public void AttributeTests_Parallel_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>("test1");
            container.RegisterType<ITest1, Test11>("test11");
            container.RegisterType<ITest1, Test12>("test12");
            container.RegisterType<ITest2, Test2>("test2");
            container.RegisterType<ITest2, Test22>("test22");
            container.RegisterType<ITest3, Test3>();

            Parallel.For(0, 50000, (i) =>
            {
                var test1 = container.Resolve<ITest1>();
                var test2 = container.Resolve<ITest2>("test2");
                var test3 = container.Resolve<ITest3>();

                Assert.IsNotNull(test1);
                Assert.IsNotNull(test2);
                Assert.IsNotNull(test3);

                Assert.IsTrue(test3.MethodInvoked);

                Assert.IsInstanceOfType(test3.test1, typeof(Test11));
                Assert.IsInstanceOfType(test3.test2, typeof(Test22));
            });
        }

        [TestMethod]
        public void AttributeTests_Parallel_Lazy_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>("test1");
            container.RegisterType<ITest1, Test11>("test11");
            container.RegisterType<ITest1, Test12>("test12");
            container.RegisterType<ITest2, Test2>("test2");
            container.RegisterType<ITest2, Test22>("test22");
            container.RegisterType<ITest3, Test3>();
            container.RegisterType<ITest4, Test4>();

            Parallel.For(0, 50000, (i) =>
            {
                var test1 = container.Resolve<Lazy<ITest1>>();
                var test2 = container.Resolve<Lazy<ITest2>>("test2");
                var test3 = container.Resolve<Lazy<ITest3>>();
                var test4 = container.Resolve<Lazy<ITest4>>();

                Assert.IsNotNull(test1.Value);
                Assert.IsNotNull(test2.Value);
                Assert.IsNotNull(test3.Value);
                Assert.IsNotNull(test4.Value);

                Assert.IsTrue(test3.Value.MethodInvoked);
                Assert.IsTrue(test4.Value.MethodInvoked);

                Assert.IsInstanceOfType(test3.Value.test1, typeof(Test11));
                Assert.IsInstanceOfType(test3.Value.test2, typeof(Test22));

                Assert.IsInstanceOfType(test4.Value.test1.Value, typeof(Test11));
                Assert.IsInstanceOfType(test4.Value.test2.Value, typeof(Test22));
            });
        }

        public interface ITest1 { }

        public interface ITest2 { ITest1 test1 { get; set; } }

        public interface ITest3 { ITest2 test2 { get; set; } ITest1 test1 { get; set; } bool MethodInvoked { get; set; } bool MethodInvoked2 { get; set; } }

        public interface ITest4 { Lazy<ITest2> test2 { get; set; } Lazy<ITest1> test1 { get; set; } bool MethodInvoked { get; set; } }

        public class Test1 : ITest1
        { }

        public class Test11 : ITest1
        { }

        public class Test12 : ITest1
        { }

        public class Test22 : ITest2 { public ITest1 test1 { get; set; } }

        public class Test2 : ITest2
        {
            public ITest1 test1 { get; set; }

            public Test2([Dependency("test11")]ITest1 test1)
            {
                Shield.EnsureNotNull(test1, nameof(test1));
                Shield.EnsureTypeOf<Test11>(test1);
            }
        }

        public class Test3 : ITest3
        {
            [Dependency("test11")]
            public ITest1 test1 { get; set; }

            [Dependency("test22")]
            public ITest2 test2 { get; set; }

            [InjectionMethod]
            public void MethodTest([Dependency("test22")]ITest2 test2)
            {
                Shield.EnsureNotNull(test2, nameof(test2));
                this.MethodInvoked = true;
            }

            [InjectionMethod]
            public void MethodTest2([Dependency("test22")]ITest2 test2)
            {
                Shield.EnsureNotNull(test2, nameof(test2));
                this.MethodInvoked2 = true;
            }

            public Test3([Dependency("test12")]ITest1 test12, [Dependency("test2")]ITest2 test2)
            {
                Shield.EnsureNotNull(test12, nameof(test12));
                Shield.EnsureNotNull(test2, nameof(test2));

                Shield.EnsureTypeOf<Test12>(test12);
                Shield.EnsureTypeOf<Test2>(test2);
            }

            public bool MethodInvoked { get; set; }
            public bool MethodInvoked2 { get; set; }
        }

        public class Test4 : ITest4
        {
            [Dependency("test11")]
            public Lazy<ITest1> test1 { get; set; }

            [Dependency("test22")]
            public Lazy<ITest2> test2 { get; set; }

            [InjectionMethod]
            public void MethodTest([Dependency("test22")]Lazy<ITest2> test2)
            {
                Shield.EnsureNotNull(test2.Value, nameof(test2.Value));
                this.MethodInvoked = true;
            }

            public Test4([Dependency("test12")]Lazy<ITest1> test1, [Dependency("test2")]Lazy<ITest2> test2)
            {
                Shield.EnsureNotNull(test1.Value, nameof(test1.Value));
                Shield.EnsureNotNull(test2.Value, nameof(test2.Value));

                Shield.EnsureTypeOf<Test12>(test1.Value);
                Shield.EnsureTypeOf<Test2>(test2.Value);
            }

            public bool MethodInvoked { get; set; }
        }
    }
}
