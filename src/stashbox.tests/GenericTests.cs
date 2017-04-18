using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using Stashbox.Exceptions;

namespace Stashbox.Tests
{
    [TestClass]
    public class GenericTests
    {
        [TestMethod]
        public void GenericTests_Resolve()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.IsNotNull(inst);
                Assert.IsInstanceOfType(inst, typeof(Test1<int, string>));
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_SameTime_DifferentParameter()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();
                var inst2 = container.Resolve<ITest1<int, int>>();

                Assert.IsNotNull(inst);
                Assert.IsInstanceOfType(inst, typeof(Test1<int, string>));

                Assert.IsNotNull(inst2);
                Assert.IsInstanceOfType(inst2, typeof(Test1<int, int>));
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_SameTime_DifferentParameter_Singleton()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();
                var inst2 = container.Resolve<ITest1<int, int>>();

                Assert.IsNotNull(inst);
                Assert.IsInstanceOfType(inst, typeof(Test1<int, string>));

                Assert.IsNotNull(inst2);
                Assert.IsInstanceOfType(inst2, typeof(Test1<int, int>));
            }
        }

        [TestMethod]
        public void GenericTests_CanResolve()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                Assert.IsTrue(container.CanResolve(typeof(ITest1<,>)));
                Assert.IsTrue(container.CanResolve(typeof(ITest1<int, int>)));
            }
        }

        [TestMethod]
        public void GenericTests_DependencyResolve()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                container.RegisterType(typeof(ITest2<,>), typeof(Test2<,>));
                var inst = container.Resolve<ITest2<int, string>>();

                Assert.IsNotNull(inst.Test);
                Assert.IsInstanceOfType(inst.Test, typeof(Test1<int, string>));
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_Fluent()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.IsNotNull(inst);
                Assert.IsInstanceOfType(inst, typeof(Test1<int, string>));
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_ReMap()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                container.ReMap(typeof(ITest1<,>), typeof(Test12<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.IsNotNull(inst);
                Assert.IsInstanceOfType(inst, typeof(Test12<int, string>));
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_ReMap_Fluent()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));
                container.ReMap(typeof(ITest1<,>), typeof(Test12<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.IsNotNull(inst);
                Assert.IsInstanceOfType(inst, typeof(Test12<int, string>));
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_Parallel()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>));

                Parallel.For(0, 50000, (i) =>
                {
                    var inst = container.Resolve<ITest1<int, string>>();

                    Assert.IsNotNull(inst);
                    Assert.IsInstanceOfType(inst, typeof(Test1<int, string>));
                });
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_Constraint_Array()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest<>));
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));

                var inst = container.ResolveAll<IConstraintTest<ConstraintArgument>>().ToArray();
                Assert.AreEqual(1, inst.Length);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void GenericTests_Resolve_Constraint()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                container.Resolve<IConstraintTest<ConstraintArgument>>();
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_Constraint_Constructor()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest<>));
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                container.RegisterType<ConstraintTest3>();
                var inst = container.Resolve<ConstraintTest3>();

                Assert.IsInstanceOfType(inst.Test, typeof(ConstraintTest<ConstraintArgument>));
            }
        }

        public interface Constraint { }

        public interface IConstraintTest<T> { }

        public class ConstraintTest<T> : IConstraintTest<T> { }

        public class ConstraintTest2<T> : IConstraintTest<T> where T : Constraint { }

        public class ConstraintArgument { }

        public class ConstraintTest3
        {
            public IConstraintTest<ConstraintArgument> Test { get; set; }

            public ConstraintTest3(IConstraintTest<ConstraintArgument> test)
            {
                this.Test = test;
            }
        }

        public interface ITest1<I, K>
        {
            I IProp { get; }
            K KProp { get; }
        }

        public interface ITest2<I, K>
        {
            ITest1<I, K> Test { get; }
        }

        public class Test1<I, K> : ITest1<I, K>
        {
            public I IProp { get; }
            public K KProp { get; }
        }

        public class Test12<I, K> : ITest1<I, K>
        {
            public I IProp { get; }
            public K KProp { get; }
        }

        public class Test2<I, K> : ITest2<I, K>
        {
            public ITest1<I, K> Test { get; private set; }

            public Test2(ITest1<I, K> test1, ITest1<I, K> test2)
            {
                Test = test1;
            }
        }
    }
}
