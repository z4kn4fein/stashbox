using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class ResolutionFromParentContainerTests
    {
        [Fact]
        public void ContainerTests_ChildContainer_Resolve_Dependency_From_Child()
        {
            var container = new StashboxContainer();
            container.Register<ITest3, Test3>();

            var child = container.CreateChildContainer();
            child.Register<ITest1, Test1>();
            child.Register<ITest2, Test2>();

            var test3 = child.Resolve<ITest3>();

            Assert.NotNull(test3);
            Assert.IsType<Test3>(test3);
            Assert.Equal(container, child.ParentContainer);
        }

        [Fact]
        public void ContainerTests_ChildContainer_Resolve_Dependency_Across_Childs()
        {
            var container = new StashboxContainer();
            container.Register<ITest3, Test3>();

            var child = container.CreateChildContainer().CreateChildContainer();
            child.Register<ITest1, Test1>();
            child.Register<ITest2, Test2>();

            var test3 = child.Resolve<ITest3>();

            Assert.NotNull(test3);
        }

        [Fact]
        public void ContainerTests_ChildContainer_Resolve_Dependency_Across_Childs_2()
        {
            var container = new StashboxContainer();
            container.Register<ITest3, Test3>();

            var child = container.CreateChildContainer();
            child.Register<ITest1, Test1>();

            var child2 = child.CreateChildContainer();
            child2.Register<ITest2, Test2>();

            var test3 = child2.Resolve<ITest3>();

            Assert.NotNull(test3);
        }

        [Fact]
        public void ContainerTests_ChildContainer_Resolve_Dependency_Across_Childs_3()
        {
            var container = new StashboxContainer();
            container.Register<ITest3, Test3>();

            var child = container.CreateChildContainer();
            child.Register<ITest2, Test2>();

            var child2 = child.CreateChildContainer();
            child2.Register<ITest1, Test1>();

            var test3 = child2.Resolve<ITest3>();

            Assert.NotNull(test3);
        }

        [Fact]
        public void ContainerTests_ChildContainer_Resolve_Dependency_Across_Childs_Wrapper()
        {
            var container = new StashboxContainer();
            container.Register<ITest5, Test5>();

            var child = container.CreateChildContainer();
            child.Register<ITest2, Test2>();

            var child2 = child.CreateChildContainer();
            child2.Register<ITest3, Test3>();

            var child3 = child2.CreateChildContainer();
            child3.Register<ITest1, Test1>();

            var test3 = child3.Resolve<ITest5>();

            Assert.NotNull(test3);
            Assert.NotNull(test3.Func());
            Assert.NotNull(test3.Lazy.Value);
            Assert.NotNull(test3.Enumerable);
            Assert.Single(test3.Enumerable);
            Assert.NotNull(test3.Tuple.Item1);
            Assert.NotNull(test3.Tuple.Item2);
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
        { }

        class Test2 : ITest2
        {
            public Test2(ITest1 test1)
            { }
        }

        class Test3 : ITest3
        {
            public Test3(ITest1 test1, ITest2 test2)
            { }
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
