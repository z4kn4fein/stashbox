using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Entity;
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
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            var child = container.CreateChildContainer();
            child.RegisterType<ITest3, Test3>();

            var test3 = child.Resolve<ITest3>();

            Assert.IsNotNull(test3);
            Assert.IsInstanceOfType(test3, typeof(Test3));
            Assert.AreEqual(container, child.ParentContainer);
        }

        [TestMethod]
        public void ContainerTests_IsRegistered()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            Assert.IsTrue(container.IsRegistered<ITest1>());
            Assert.IsTrue(container.IsRegistered(typeof(ITest2)));
            Assert.IsFalse(container.IsRegistered<ITest3>());
            Assert.IsFalse(container.IsRegistered<ITest1>("test"));
            Assert.IsFalse(container.IsRegistered(typeof(ITest1), "test"));
        }

        [TestMethod]
        public void ContainerTests_ResolverTest()
        {
            var container = new StashboxContainer();
            container.RegisterResolver((context, typeInfo) => typeInfo.Type == typeof(ITest1), new TestResolverFactory());
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
        }

        public class Test3 : ITest3
        {
        }

        public class TestResolver : Resolver
        {
            public TestResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
            {
            }

            public override Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
            {
                throw new NotImplementedException();
            }

            public override object Resolve(ResolutionInfo resolutionInfo)
            {
                return new Test1();
            }
        }

        public class TestResolverFactory : ResolverFactory
        {
            public override Resolver Create(IContainerContext containerContext, TypeInformation typeInfo)
            {
                return new TestResolver(containerContext, typeInfo);
            }
        }
    }
}
