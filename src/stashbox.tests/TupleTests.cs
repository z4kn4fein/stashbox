using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stashbox.Tests
{
    [TestClass]
    public class TupleTests
    {
        [TestMethod]
        public void TupleTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<ITest1, Test1>();
            var inst = container.Resolve<Tuple<ITest, ITest1>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Tuple<ITest, ITest1>));
            Assert.IsInstanceOfType(inst.Item1, typeof(Test));
            Assert.IsInstanceOfType(inst.Item2, typeof(Test1));
        }

        [TestMethod]
        public void TupleTests_Resolve_Lazy()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<ITest1, Test1>();
            var inst = container.Resolve<Tuple<ITest, Lazy<ITest1>>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Tuple<ITest, Lazy<ITest1>>));
            Assert.IsInstanceOfType(inst.Item1, typeof(Test));
            Assert.IsInstanceOfType(inst.Item2.Value, typeof(Test1));
        }

        [TestMethod]
        public void TupleTests_Resolve_Func()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<ITest1, Test1>();
            var inst = container.Resolve<Tuple<ITest, Func<ITest1>>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Tuple<ITest, Func<ITest1>>));
            Assert.IsInstanceOfType(inst.Item1, typeof(Test));
            Assert.IsInstanceOfType(inst.Item2(), typeof(Test1));
        }

        [TestMethod]
        public void TupleTests_Resolve_Enumerable()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<ITest1, Test1>();
            var inst = container.Resolve<Tuple<IEnumerable<ITest>, Func<ITest1>>>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst, typeof(Tuple<IEnumerable<ITest>, Func<ITest1>>));
            Assert.IsInstanceOfType(inst.Item1.First(), typeof(Test));
            Assert.IsInstanceOfType(inst.Item2(), typeof(Test1));
        }

        [TestMethod]
        public void TupleTests_Resolve_Constructor()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.IsNotNull(inst);
            Assert.IsInstanceOfType(inst.Test, typeof(Tuple<ITest, ITest1>));
            Assert.IsInstanceOfType(inst.Test.Item1, typeof(Test));
            Assert.IsInstanceOfType(inst.Test.Item2, typeof(Test1));
        }

        public interface ITest
        { }

        public interface ITest1
        { }

        public class Test : ITest
        { }

        public class Test1 : ITest1
        { }

        public class Test2
        {
            public Tuple<ITest, ITest1> Test { get; }

            public Test2(Tuple<ITest, ITest1> test)
            {
                this.Test = test;
            }
        }
    }
}
