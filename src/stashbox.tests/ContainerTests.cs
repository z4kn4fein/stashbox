using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void ContainerTests_ChildContainer_ResolveFromParent()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();

            var child = container.CreateChildContainer();

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
        public void ContainerTests_Validate_Ok()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterType<ITest2, Test2>();
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
            }
        }

        [TestMethod]
        public void ContainerTests_CanResolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>();

            var child = container.CreateChildContainer();

            Assert.IsTrue(container.CanResolve<ITest1>());
            Assert.IsTrue(container.CanResolve(typeof(ITest2)));
            Assert.IsTrue(container.CanResolve<IEnumerable<ITest2>>());
            Assert.IsTrue(container.CanResolve<Lazy<ITest2>>());
            Assert.IsTrue(container.CanResolve<Func<ITest2>>());
            Assert.IsTrue(container.CanResolve<Tuple<ITest2>>());

            Assert.IsTrue(child.CanResolve<ITest1>());
            Assert.IsTrue(child.CanResolve(typeof(ITest2)));
            Assert.IsTrue(child.CanResolve<IEnumerable<ITest2>>());
            Assert.IsTrue(child.CanResolve<Lazy<ITest2>>());
            Assert.IsTrue(child.CanResolve<Func<ITest2>>());
            Assert.IsTrue(child.CanResolve<Tuple<ITest2>>());

            Assert.IsFalse(container.CanResolve<ITest3>());
            Assert.IsFalse(container.CanResolve<ITest1>("test"));
            Assert.IsFalse(container.CanResolve(typeof(ITest1), "test"));
        }

        [TestMethod]
        public void ContainerTests_IsRegistered()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest1, Test1>();
            container.RegisterType<ITest2, Test2>(context => context.WithName("test"));

            var child = container.CreateChildContainer();

            Assert.IsTrue(container.IsRegistered<ITest1>());
            Assert.IsTrue(container.IsRegistered<ITest2>("test"));
            Assert.IsTrue(container.IsRegistered(typeof(ITest1)));
            Assert.IsFalse(container.IsRegistered<IEnumerable<ITest1>>());

            Assert.IsFalse(child.IsRegistered<ITest1>());
            Assert.IsFalse(child.IsRegistered(typeof(ITest1)));
            Assert.IsFalse(child.IsRegistered<IEnumerable<ITest1>>());
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
        public void ContainerTests_ResolverTest_SupportsMany()
        {
            var container = new StashboxContainer();
            container.RegisterResolver(new TestResolver2());
            var inst = container.Resolve<IEnumerable<ITest1>>();

            Assert.IsInstanceOfType(inst.First(), typeof(Test1));
        }

        [TestMethod]
        public void ContainerTests_UnknownType_Config()
        {
            var container = new StashboxContainer(config => config
                .WithUnknownTypeResolution(context => context.WithSingletonLifetime()));

            container.Resolve<Test1>();

            var regs = container.ContainerContext.RegistrationRepository.GetAllRegistrations().ToArray();

            Assert.AreEqual(1, regs.Length);
            Assert.AreEqual(regs[0].ServiceType, typeof(Test1));
            Assert.IsTrue(regs[0].RegistrationContext.Lifetime is SingletonLifetime);
        }

        public interface ITest1 { }

        public interface ITest2 { }

        public interface ITest3 { }

        public interface ITest5
        {
            Func<ITest2> Func { get; }
            Lazy<ITest2> Lazy { get; }
            IEnumerable<ITest3> Enumerable { get; }
            Tuple<ITest2, ITest3> Tuple { get; }
        }

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

        public class Test5 : ITest5
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

        public class TestResolver : IResolver
        {
            public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionInfo)
            {
                return typeInfo.Type == typeof(ITest1);
            }

            public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionInfo)
            {
                return Expression.Constant(new Test1());
            }
        }

        public class TestResolver2 : IMultiServiceResolver
        {
            public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionInfo)
            {
                return typeInfo.Type == typeof(ITest1);
            }

            public Expression GetExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionInfo)
            {
                return Expression.Constant(new Test1());
            }

            public Expression[] GetExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionInfo)
            {
                return new Expression[] { Expression.Constant(new Test1()) };
            }
        }
    }
}
