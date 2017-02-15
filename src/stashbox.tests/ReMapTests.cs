using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Infrastructure;
using Stashbox.Configuration;

namespace Stashbox.Tests
{
    [TestClass]
    public class ReMapTests
    {
        [TestMethod]
        public void ReMapTests_SingleResolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>("teszt");
            container.RegisterType<ITest1, Test12>("teszt2");

            var test1 = container.Resolve<ITest1>("teszt");
            var test2 = container.Resolve<ITest1>("teszt2");

            Assert.IsInstanceOfType(test1, typeof(Test1));
            Assert.IsInstanceOfType(test2, typeof(Test12));

            container.ReMap<ITest1, Test11>("teszt");

            var test11 = container.Resolve<ITest1>("teszt");
            var test12 = container.Resolve<ITest1>("teszt2");

            Assert.IsInstanceOfType(test11, typeof(Test11));
            Assert.IsInstanceOfType(test12, typeof(Test12));
        }

        [TestMethod]
        public void ReMapTests_Enumerable_Named()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));
            container.RegisterType<ITest1, Test1>("teszt");
            container.RegisterType<ITest1, Test12>("teszt2");

            var coll = container.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.IsInstanceOfType(coll[0], typeof(Test1));
            Assert.IsInstanceOfType(coll[1], typeof(Test12));

            container.ReMap<ITest1, Test11>("teszt");

            var coll2 = container.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.IsInstanceOfType(coll2[0], typeof(Test12));
            Assert.IsInstanceOfType(coll2[1], typeof(Test11));
        }

        [TestMethod]
        public void ReMapTests_Func_Named()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test12>("teszt2");
            container.RegisterType<ITest1, Test1>("teszt");

            container.Resolve<Func<ITest1>>("teszt2");

            var func = container.Resolve<Func<ITest1>>("teszt");

            Assert.IsInstanceOfType(func(), typeof(Test1));

            container.ReMap<ITest1, Test11>("teszt");

            var func2 = container.Resolve<Func<ITest1>>("teszt");

            Assert.IsInstanceOfType(func2(), typeof(Test11));
        }

        [TestMethod]
        public void ReMapTests_Lazy_Named()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test12>("teszt2");
            container.RegisterType<ITest1, Test1>("teszt");

            container.Resolve<Lazy<ITest1>>("teszt2");

            var lazy = container.Resolve<Lazy<ITest1>>("teszt");

            Assert.IsInstanceOfType(lazy.Value, typeof(Test1));

            container.ReMap<ITest1, Test11>("teszt");

            var lazy2 = container.Resolve<Lazy<ITest1>>("teszt");

            Assert.IsInstanceOfType(lazy2.Value, typeof(Test11));
        }

        [TestMethod]
        public void ReMapTests_Enumerable()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();

            var coll = container.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.IsInstanceOfType(coll[0], typeof(Test1));

            container.ReMap<ITest1, Test11>();

            var coll2 = container.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.IsInstanceOfType(coll2[0], typeof(Test11));
        }

        [TestMethod]
        public void ReMapTests_Func()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            
            var func = container.Resolve<Func<ITest1>>();

            Assert.IsInstanceOfType(func(), typeof(Test1));

            container.ReMap<ITest1, Test11>();

            var func2 = container.Resolve<Func<ITest1>>();

            Assert.IsInstanceOfType(func2(), typeof(Test11));
        }

        [TestMethod]
        public void ReMapTests_Lazy()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            
            var lazy = container.Resolve<Lazy<ITest1>>();

            Assert.IsInstanceOfType(lazy.Value, typeof(Test1));

            container.ReMap<ITest1, Test11>();

            var lazy2 = container.Resolve<Lazy<ITest1>>();

            Assert.IsInstanceOfType(lazy2.Value, typeof(Test11));
        }

        [TestMethod]
        public void ReMapTests_DependencyResolve()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsNotNull(test2.Test1);
            Assert.IsInstanceOfType(test2.Test1, typeof(Test1));

            container.ReMap<ITest1>(typeof(Test11));

            var test22 = container.Resolve<ITest2>();

            Assert.IsNotNull(test22.Test1);
            Assert.IsInstanceOfType(test22.Test1, typeof(Test11));
        }

        [TestMethod]
        public void ReMapTests_DependencyResolve_Fluent()
        {
            IStashboxContainer container = new StashboxContainer();
            container.RegisterType<ITest1>(typeof(Test1));
            container.RegisterType<ITest2, Test2>();

            var test2 = container.Resolve<ITest2>();

            Assert.IsNotNull(test2.Test1);
            Assert.IsInstanceOfType(test2.Test1, typeof(Test1));

            container.PrepareType<ITest1>(typeof(Test11)).ReMap();

            var test22 = container.Resolve<ITest2>();

            Assert.IsNotNull(test22.Test1);
            Assert.IsInstanceOfType(test22.Test1, typeof(Test11));
        }

        public interface ITest1 { }

        public interface ITest2
        {
            ITest1 Test1 { get; }
        }

        public class Test1 : ITest1
        { }

        public class Test11 : ITest1
        { }

        public class Test12 : ITest1
        { }

        public class Test2 : ITest2
        {
            public ITest1 Test1 { get; }

            public Test2(ITest1 test1)
            {
                this.Test1 = test1;
            }
        }
    }
}
