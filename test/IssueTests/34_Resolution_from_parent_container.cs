using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class ResolutionFromParentContainerTests
    {
        [TestMethod]
        public void ContainerTests_ChildContainer_Resolve_Dependency_From_Child()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest3, Test3>();

            var child = container.CreateChildContainer();
            child.RegisterType<ITest1, Test1>();
            child.RegisterType<ITest2, Test2>();

            var test3 = child.Resolve<ITest3>();

            Assert.IsNotNull(test3);
            Assert.IsInstanceOfType(test3, typeof(Test3));
            Assert.AreEqual(container, child.ParentContainer);
        }

        [TestMethod]
        public void ContainerTests_ChildContainer_Resolve_Dependency_Across_Childs()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest3, Test3>();

            var child = container.CreateChildContainer().CreateChildContainer();
            child.RegisterType<ITest1, Test1>();
            child.RegisterType<ITest2, Test2>();

            var test3 = child.Resolve<ITest3>();

            Assert.IsNotNull(test3);
        }

        [TestMethod]
        public void ContainerTests_ChildContainer_Resolve_Dependency_Across_Childs_2()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest3, Test3>();

            var child = container.CreateChildContainer();
            child.RegisterType<ITest1, Test1>();

            var child2 = child.CreateChildContainer();
            child2.RegisterType<ITest2, Test2>();

            var test3 = child2.Resolve<ITest3>();

            Assert.IsNotNull(test3);
        }

        [TestMethod]
        public void ContainerTests_ChildContainer_Resolve_Dependency_Across_Childs_Wrapper()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest5, Test5>();

            var child = container.CreateChildContainer();
            child.RegisterType<ITest2, Test2>();

            var child2 = child.CreateChildContainer();
            child2.RegisterType<ITest3, Test3>();

            var child3 = child2.CreateChildContainer();
            child3.RegisterType<ITest1, Test1>();

            var test3 = child3.Resolve<ITest5>();

            Assert.IsNotNull(test3);
            Assert.IsNotNull(test3.Func());
            Assert.IsNotNull(test3.Lazy.Value);
            Assert.IsNotNull(test3.Enumerable);
            Assert.AreEqual(1, test3.Enumerable.Count());
            Assert.IsNotNull(test3.Tuple.Item1);
            Assert.IsNotNull(test3.Tuple.Item2);
        }

        interface ITest1 { }

        interface ITest2 { }

        interface ITest3 { }

        interface ITest5
        {
            Func<ITest2> Func { get; }
            Lazy<ITest2> Lazy { get; }
            IEnumerable<ITest3> Enumerable { get; }
            Tuple<ITest2, ITest3> Tuple { get; }
        }

        class Test1 : ITest1
        {}

        class Test2 : ITest2
        {
            public Test2(ITest1 test1)
            {}
        }

        class Test3 : ITest3
        {
            public Test3(ITest1 test1, ITest2 test2)
            {}
        }
        
        class Test5 : ITest5
        {
            public Test5(Func<ITest2> func, Lazy<ITest2> lazy, IEnumerable<ITest3> enumerable, Tuple<ITest2, ITest3> tuple)
            {
                Func = func;
                Lazy = lazy;
                Enumerable = enumerable;
                Tuple = tuple;
            }

            public Func<ITest2> Func { get; }
            public Lazy<ITest2> Lazy { get; }
            public IEnumerable<ITest3> Enumerable { get; }
            public Tuple<ITest2, ITest3> Tuple { get; }
        }

    }
}
