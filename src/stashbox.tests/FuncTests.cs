using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class FuncTests
    {
        [TestMethod]
        public void FuncTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            var inst = container.Resolve<Func<ITest>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Func<ITest>));
            Assert.IsInstanceOfType(inst(), typeof(Test));
        }

        [TestMethod]
        public void FuncTests_Resolve_ConstructorDependency()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.IsNotNull(inst.Test);
            Assert.IsInstanceOfType(inst.Test, typeof(Func<ITest>));
            Assert.IsInstanceOfType(inst.Test(), typeof(Test));
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection()
        {
            var container = new StashboxContainer();
            container.RegisterType<IFTest1, FTest1>();
            var inst = container.Resolve<Func<ITest, IFTest1>>();

            var t = new Test();
            var r = inst(t);

            Assert.AreSame(t, r.Test);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_2Params()
        {
            var container = new StashboxContainer();
            container.RegisterType<IFTest2, FTest2>();
            var inst = container.Resolve<Func<ITest, IFTest1, IFTest2>>();

            var t = new Test();
            var t1 = new FTest1();
            var r = inst(t, t1);

            Assert.AreSame(t, r.Test);
            Assert.AreSame(t1, r.Test1);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_3Params()
        {
            var container = new StashboxContainer();
            container.RegisterType<IFTest3, FTest3>();
            var inst = container.Resolve<Func<ITest, IFTest1, IFTest2, IFTest3>>();

            var t = new Test();
            var t1 = new FTest1();
            var t2 = new FTest2(null);
            var r = inst(t, t1, t2);

            Assert.AreSame(t, r.Test);
            Assert.AreSame(t1, r.Test1);
            Assert.AreSame(t2, r.Test2);
        }

        [TestMethod]
        public void FuncTests_Resolve_ParameterInjection_SubDependency()
        {
            var container = new StashboxContainer();
            container.RegisterType<FunctTest2>();
            container.RegisterType<FuncTest>();
            var inst = container.Resolve<Func<ITest, FuncTest>>();

            var t = new Test();
            var r = inst(t);

            Assert.AreSame(t, r.Func2.Test);
        }

        public class FuncTest
        {
            public FunctTest2 Func2 { get; set; }

            public FuncTest(FunctTest2 func2)
            {
                this.Func2 = func2;
            }
        }

        public class FunctTest2
        {
            public ITest Test { get; set; }

            public FunctTest2(ITest test)
            {
                this.Test = test;
            }
        }

        public interface ITest
        { }

        public interface IFTest1
        {
            ITest Test { get; set; }
        }

        public interface IFTest2
        {
            IFTest1 Test1 { get; set; }

            ITest Test { get; set; }
        }

        public interface IFTest3
        {
            IFTest2 Test2 { get; set; }

            IFTest1 Test1 { get; set; }

            ITest Test { get; set; }
        }

        public class FTest1 : IFTest1
        {
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init(ITest test)
            {
                this.Test = test;
            }
        }

        public class FTest2 : IFTest2
        {
            [Dependency]
            public ITest Test { get; set; }

            public IFTest1 Test1 { get; set; }

            public FTest2(IFTest1 test1)
            {
                this.Test1 = test1;
            }
        }

        public class FTest3 : IFTest3
        {
            [Dependency]
            public IFTest2 Test2 { get; set; }

            public IFTest1 Test1 { get; set; }

            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init(ITest test)
            {
                this.Test = test;
            }

            public FTest3(IFTest1 test1)
            {
                this.Test1 = test1;
            }
        }

        public class Test : ITest
        { }

        public class Test2
        {
            public Func<ITest> Test { get; private set; }

            public Test2(Func<ITest> test)
            {
                this.Test = test;
            }
        }
    }
}
