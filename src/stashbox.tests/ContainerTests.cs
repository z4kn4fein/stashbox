using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Resolution;
using System.Linq;
using Stashbox.Lifetime;

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
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();

            var child = container.BeginScope();

            var test1 = child.Resolve<ITest1>();

            Assert.IsNotNull(test1);
            Assert.IsInstanceOfType(test1, typeof(Test1));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ContainerTests_Validate_MissingDependency()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest2, Test2>();
                container.Validate();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void ContainerTests_Validate_CircularDependency()
        {
            using (var container = new StashboxContainer(config => config.WithCircularDependencyTracking()))
            {
                container.RegisterType<ITest1, Test4>();
                container.RegisterType<ITest3, Test3>();
                container.Validate();
            }
        }

        [TestMethod]
        public void ContainerTests_CheckRegistration()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();

                var reg = container.ContainerContext.RegistrationRepository.GetAllRegistrations().FirstOrDefault(r => r.ServiceType == typeof(ITest1));

                Assert.IsNotNull(reg);

                reg = container.ContainerContext.RegistrationRepository.GetAllRegistrations().FirstOrDefault(r => r.ImplementationType == typeof(Test1));

                Assert.IsNotNull(reg);
                Assert.IsInstanceOfType(reg.LifetimeManager, typeof(TransientLifetime));
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
            container.RegisterResolver(new TestResolver());
            var inst = container.Resolve<ITest1>();

            Assert.IsInstanceOfType(inst, typeof(Test1));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ContainerTests_ResolverTest_SupportsMany_Throw()
        {
            var container = new StashboxContainer();
            container.RegisterResolver(new TestResolver());
            container.Resolve<IEnumerable<ITest1>>();
        }

        [TestMethod]
        public void ContainerTests_ResolverTest_SupportsMany()
        {
            var container = new StashboxContainer();
            container.RegisterResolver(new TestResolver2());
            var inst = container.Resolve<IEnumerable<ITest1>>();

            Assert.IsInstanceOfType(inst.First(), typeof(Test1));
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

        public class Test4 : ITest1
        {
            public Test4(ITest3 test3)
            {
                
            }
        }

        public class TestResolver : Resolver
        {
            public override bool SupportsMany => true;

            public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo)
            {
                return typeInfo.Type == typeof(ITest1);
            }

            public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
            {
                return Expression.Constant(new Test1());
            }
        }

        public class TestResolver2 : Resolver
        {
            public override bool SupportsMany => true;

            public override bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo)
            {
                return typeInfo.Type == typeof(ITest1);
            }

            public override Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
            {
                return Expression.Constant(new Test1());
            }

            public override Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
            {
                return new Expression[] { Expression.Constant(new Test1()) };
            }
        }
    }
}
