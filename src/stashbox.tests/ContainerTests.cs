using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.Tests
{
    [TestClass]
    public class ContainerTests
    {
        [TestMethod]
        public void ContainerTests_ChildContainer()
        {
            var container = new StashboxContainer(config => config.WithParentContainerResolution());
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            var child = container.BeginScope();
            child.RegisterType<ITest3, Test3>();

            var test3 = child.Resolve<ITest3>();

            Assert.IsNotNull(test3);
            Assert.IsInstanceOfType(test3, typeof(Test3));
            Assert.AreEqual(container, child.ParentContainer);
        }

        [TestMethod]
        public void ContainerTests_ChildContainer_ResolveFromParent()
        {
            var container = new StashboxContainer(config => config.WithParentContainerResolution());
            container.RegisterType<ITest1, Test1>();

            var child = container.BeginScope();

            var test1 = child.Resolve<ITest1>();

            Assert.IsNotNull(test1);
            Assert.IsInstanceOfType(test1, typeof(Test1));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ContainerTests_ChildContainer_WithoutResolveFromParent()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();

                var child = container.BeginScope();

                var test1 = child.Resolve<ITest1>();
            }
        }

        [TestMethod]
        public void ContainerTests_CanResolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            Assert.IsTrue(container.CanResolve<ITest1>());
            Assert.IsTrue(container.CanResolve(typeof(ITest2)));
            Assert.IsFalse(container.CanResolve<ITest3>());
            Assert.IsFalse(container.CanResolve<ITest1>("test"));
            Assert.IsFalse(container.CanResolve(typeof(ITest1), "test"));
        }

        [TestMethod]
        public void ContainerTests_ResolverTest()
        {
            var container = new StashboxContainer();
            container.RegisterResolver<TestResolver>((context, typeInfo) => typeInfo.Type == typeof(ITest1),
                (context, typeInfo) => new TestResolver(context, typeInfo));
            var inst = container.Resolve<ITest1>();

            Assert.IsInstanceOfType(inst, typeof(Test1));
        }

        public interface ITest1 { }

        public interface ITest2 { }

        public interface ITest3 { }

        public class Test1 : ITest1
        {
        }

        public class Test2 : ITest2
        {
            public Test2(ITest1 test1)
            {
            }
        }

        public class Test3 : ITest3
        {
            public Test3(ITest1 test1, ITest2 test2)
            {
            }
        }

        public class TestResolver : Resolver
        {
            public TestResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
            {
            }

            public override Expression GetExpression(ResolutionInfo resolutionInfo)
            {
                return Expression.New(typeof(Test1).GetConstructor(new Type[] { }));
            }
        }
    }
}
