using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests
{

    public class TupleTests
    {
        [Fact]
        public void TupleTests_Resolve()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            container.Register<ITest1, Test1>();
            var inst = container.Resolve<Tuple<ITest, ITest1>>();

            Assert.NotNull(inst);
            Assert.IsType<Tuple<ITest, ITest1>>(inst);
            Assert.IsType<Test>(inst.Item1);
            Assert.IsType<Test1>(inst.Item2);
        }

        [Fact]
        public void TupleTests_Resolve_Null()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            var inst = container.Resolve<Tuple<ITest, ITest1>>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void TupleTests_Resolve_Lazy()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            container.Register<ITest1, Test1>();
            var inst = container.Resolve<Tuple<ITest, Lazy<ITest1>>>();

            Assert.NotNull(inst);
            Assert.IsType<Tuple<ITest, Lazy<ITest1>>>(inst);
            Assert.IsType<Test>(inst.Item1);
            Assert.IsType<Test1>(inst.Item2.Value);
        }

        [Fact]
        public void TupleTests_Resolve_Func()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            container.Register<ITest1, Test1>();
            var inst = container.Resolve<Tuple<ITest, Func<ITest1>>>();

            Assert.NotNull(inst);
            Assert.IsType<Tuple<ITest, Func<ITest1>>>(inst);
            Assert.IsType<Test>(inst.Item1);
            Assert.IsType<Test1>(inst.Item2());
        }

        [Fact]
        public void TupleTests_Resolve_Enumerable()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            container.Register<ITest1, Test1>();
            var inst = container.Resolve<Tuple<IEnumerable<ITest>, Func<ITest1>>>();

            Assert.NotNull(inst);
            Assert.IsType<Tuple<IEnumerable<ITest>, Func<ITest1>>>(inst);
            Assert.IsType<Test>(inst.Item1.First());
            Assert.IsType<Test1>(inst.Item2());
        }

        [Fact]
        public void TupleTests_Resolve_Constructor()
        {
            var container = new StashboxContainer();
            container.Register<ITest, Test>();
            container.Register<ITest1, Test1>();
            container.Register<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.NotNull(inst);
            Assert.IsType<Tuple<ITest, ITest1>>(inst.Test);
            Assert.IsType<Test>(inst.Test.Item1);
            Assert.IsType<Test1>(inst.Test.Item2);
        }

        interface ITest
        { }

        interface ITest1
        { }

        class Test : ITest
        { }

        class Test1 : ITest1
        { }

        class Test2
        {
            public Tuple<ITest, ITest1> Test { get; }

            public Test2(Tuple<ITest, ITest1> test)
            {
                this.Test = test;
            }
        }
    }
}
