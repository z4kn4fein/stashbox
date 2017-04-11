using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests
{
    [TestClass]
    public class ResolveFactoryTests
    {
        [TestMethod]
        public void ResolveFactoryTests_ParameterLess()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>();
                var factory = container.ResolveFactory<Test>();

                Assert.IsNotNull(factory());
            }
        }

        [TestMethod]
        public void ResolveFactoryTests_OneParam()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test1>();
                var factory = container.ResolveFactory<Test, Test1>();

                var test = new Test();
                var inst = factory(test);

                Assert.AreSame(test, inst.Test);
            }
        }

        [TestMethod]
        public void ResolveFactoryTests_TwoParams()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test2>();
                var factory = container.ResolveFactory<Test, Test1, Test2>();

                var test = new Test();
                var test1 = new Test1(test);
                var inst = factory(test, test1);

                Assert.AreSame(test, inst.Test);
                Assert.AreSame(test1, inst.Test1);
            }
        }

        [TestMethod]
        public void ResolveFactoryTests_ThreeParams()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test3>();
                var factory = container.ResolveFactory<Test, Test1, Test2, Test3>();

                var test = new Test();
                var test1 = new Test1(test);
                var test2 = new Test2(test1, test);
                var inst = factory(test, test1, test2);

                Assert.AreSame(test, inst.Test);
                Assert.AreSame(test1, inst.Test1);
                Assert.AreSame(test2, inst.Test2);
            }
        }

        [TestMethod]
        public void ResolveFactoryTests_FourParams()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test4>();
                var factory = container.ResolveFactory<Test, Test1, Test2, Test3, Test4>();

                var test = new Test();
                var test1 = new Test1(test);
                var test2 = new Test2(test1, test);
                var test3 = new Test3(test1, test, test2);
                var inst = factory(test, test1, test2, test3);

                Assert.AreSame(test, inst.Test);
                Assert.AreSame(test1, inst.Test1);
                Assert.AreSame(test2, inst.Test2);
                Assert.AreSame(test3, inst.Test3);
            }
        }

        public class Test
        { }

        public class Test1
        {
            public Test1(Test test)
            {
                this.Test = test;
            }

            public Test Test { get; }
        }

        public class Test2
        {
            public Test2(Test1 test1, Test test)
            {
                this.Test1 = test1;
                this.Test = test;
            }

            public Test1 Test1 { get; }
            public Test Test { get; }
        }

        public class Test3
        {
            public Test3(Test1 test1, Test test, Test2 test2)
            {
                this.Test1 = test1;
                this.Test = test;
                this.Test2 = test2;
            }

            public Test1 Test1 { get; }
            public Test Test { get; }
            public Test2 Test2 { get; }
        }

        public class Test4
        {
            public Test4(Test1 test1, Test test, Test2 test2, Test3 test3)
            {
                this.Test1 = test1;
                this.Test = test;
                this.Test2 = test2;
                this.Test3 = test3;
            }

            public Test1 Test1 { get; }
            public Test Test { get; }
            public Test2 Test2 { get; }
            public Test3 Test3 { get; }
        }
    }
}
