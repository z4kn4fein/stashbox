using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.Tests
{
    [TestClass]
    public class LazyTests
    {
        [TestMethod]
        public void LazyTests_Resolve()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            var inst = container.Resolve<Lazy<ITest>>();

            Assert.IsNotNull(inst);
            Assert.IsFalse(inst.IsValueCreated);
            Assert.IsInstanceOfType(inst, typeof(Lazy<ITest>));
            Assert.IsInstanceOfType(inst.Value, typeof(Test));
        }

        [TestMethod]
        public void LazyTests_Resolve_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Lazy<ITest>>(nullResultAllowed: true);

            Assert.IsNull(inst);
        }

        [TestMethod]
        public void LazyTests_Resolve_Func()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            var inst = container.Resolve<Lazy<Func<ITest>>>();

            Assert.IsNotNull(inst);
            Assert.IsFalse(inst.IsValueCreated);
            Assert.IsInstanceOfType(inst, typeof(Lazy<Func<ITest>>));
            Assert.IsInstanceOfType(inst.Value(), typeof(Test));
        }

        [TestMethod]
        public void LazyTests_Resolve_Func_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Lazy<Func<ITest>>>(nullResultAllowed: true);

            Assert.IsNull(inst);
        }

        [TestMethod]
        public void LazyTests_Resolve_Enumerable()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            var inst = container.Resolve<Lazy<IEnumerable<ITest>>>();

            Assert.IsNotNull(inst);
            Assert.IsFalse(inst.IsValueCreated);
            Assert.IsInstanceOfType(inst, typeof(Lazy<IEnumerable<ITest>>));
            Assert.IsInstanceOfType(inst.Value.First(), typeof(Test));
        }

        [TestMethod]
        public void LazyTests_Resolve_Enumerable_Null()
        {
            var container = new StashboxContainer();
            var inst = container.Resolve<Lazy<IEnumerable<ITest>>>();
            
            Assert.AreEqual(0, inst.Value.Count());
        }

        [TestMethod]
        public void LazyTests_Resolve_ConstructorDependency()
        {
            var container = new StashboxContainer();
            container.RegisterType<ITest, Test>();
            container.RegisterType<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.IsNotNull(inst.Test);
            Assert.IsFalse(inst.Test.IsValueCreated);
            Assert.IsInstanceOfType(inst.Test, typeof(Lazy<ITest>));
            Assert.IsInstanceOfType(inst.Test.Value, typeof(Test));
        }

        [TestMethod]
        public void LazyTests_Resolve_ConstructorDependency_Null()
        {
            var container = new StashboxContainer();
            container.RegisterType<Test2>();
            var inst = container.Resolve<Test2>(nullResultAllowed: true);

            Assert.IsNull(inst);
        }

        [TestMethod]
        public void LazyTests_Resolve_Circular()
        {
            var container = new StashboxContainer(config => 
            config.WithCircularDependencyTracking()
            .WithCircularDependencyWithLazy());

            container.RegisterSingleton<Circular1>();
            container.RegisterSingleton<Circular2>();

            var inst1 = container.Resolve<Circular1>();
            var inst2 = container.Resolve<Circular2>();

            Assert.AreSame(inst2, inst1.Dep.Value);
            Assert.AreSame(inst1, inst2.Dep.Value);
        }

        [TestMethod]
        public void LazyTests_Resolve_Circular_Evaluate()
        {
            var container = new StashboxContainer(config =>
            config.WithCircularDependencyTracking()
            .WithCircularDependencyWithLazy());

            container.RegisterSingleton<Circular1>();
            container.RegisterSingleton<Circular2>();

            var inst1 = container.Resolve<Circular1>();

            var val = inst1.Dep.Value.Dep.Value.Dep.Value.Dep;

            Assert.IsNotNull(val);
        }

        [TestMethod]
        public void LazyTests_Resolve_IEnumerable_Circular()
        {
            var container = new StashboxContainer(config => 
            config.WithCircularDependencyTracking()
            .WithCircularDependencyWithLazy());

            container.RegisterSingleton<Circular3>();
            container.RegisterSingleton<Circular4>();

            var inst1 = container.Resolve<Circular3>();
            var inst2 = container.Resolve<Circular4>();

            Assert.AreSame(inst2, inst1.Dep.First().Value);
            Assert.AreSame(inst1, inst2.Dep.First().Value);
        }

        public interface ITest
        { }

        public class Test : ITest
        { }

        public class Test2
        {
            public Lazy<ITest> Test { get; }

            public Test2(Lazy<ITest> test)
            {
                this.Test = test;
            }
        }

        public class Circular1
        {
            public Lazy<Circular2> Dep { get; set; }

            public Circular1(Lazy<Circular2> dep)
            {
                Dep = dep;
            }
        }

        public class Circular2
        {
            public Lazy<Circular1> Dep { get; set; }

            public Circular2(Lazy<Circular1> dep)
            {
                Dep = dep;
            }
        }

        public class Circular3
        {
            public IEnumerable<Lazy<Circular4>> Dep { get; set; }

            public Circular3(IEnumerable<Lazy<Circular4>> dep)
            {
                Dep = dep;
            }
        }

        public class Circular4
        {
            public IEnumerable<Lazy<Circular3>> Dep { get; set; }

            public Circular4(IEnumerable<Lazy<Circular3>> dep)
            {
                Dep = dep;
            }
        }
    }
}
