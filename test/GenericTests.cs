using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public void GenericTests_Resolve_Singleton()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                var en = container.Resolve<IEnumerable<ITest1<int, string>>>();
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.AreEqual(inst, en.ToArray()[0]);
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_Singleton_Many()
        {
            using (var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers()))
            {
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                var en = container.Resolve<IEnumerable<ITest1<int, string>>>();
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.AreEqual(inst, en.ToArray()[1]);
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_Singleton_Many_Mixed()
        {
            using (var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers()))
            {
                var inst = new Test1<int, string>();
                container.RegisterSingleton(typeof(ITest1<int, string>), typeof(Test1<int, string>));
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                container.RegisterType(typeof(ITest1<int, string>), typeof(Test1<int, string>), config => config.WithInstance(inst));
                var en = container.Resolve<IEnumerable<ITest1<int, string>>>().ToArray();

                Assert.AreEqual(3, en.Length);
                Assert.AreEqual(inst, en[2]);
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
        public void GenericTests_Named()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<,>), typeof(Test1<,>), config => config.WithName("G1"));
                container.RegisterType(typeof(ITest1<,>), typeof(Test12<,>), config => config.WithName("G2"));

                Assert.IsInstanceOfType(container.Resolve<ITest1<int, string>>("G1"), typeof(Test1<int, string>));
                Assert.IsInstanceOfType(container.Resolve<ITest1<int, double>>("G2"), typeof(Test12<int, double>));
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
        public void GenericTests_Resolve_Constraint_Multiple()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest<>));

                var inst = container.Resolve<IConstraintTest<ConstraintArgument>>();
                Assert.IsInstanceOfType(inst, typeof(ConstraintTest<ConstraintArgument>));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
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

        [TestMethod]
        public void GenericTests_Resolve_Constraint_Pick_RightImpl()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                container.RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest3<>));

                var inst = container.Resolve<IConstraintTest<ConstraintArgument1>>();
                Assert.IsInstanceOfType(inst, typeof(ConstraintTest3<ConstraintArgument1>));
            }
        }

        [TestMethod]
        public void GenericTests_Resolve_Prefer_Open_Generic_In_Named_Scope()
        {
            var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers())
               .RegisterType<ITest1<int, string>, Test1<int, string>>()
               .RegisterType(typeof(ITest1<,>), typeof(Test1<,>), config => config.InNamedScope("A"));

            container.BeginScope("A").Resolve<ITest1<int, string>>();

            Assert.AreEqual(3, container.ContainerContext.RegistrationRepository.GetAllRegistrations().Count());
        }

        [TestMethod]
        public void GenericTests_Resolve_Prefer_Open_Generic_Enumerable_In_Named_Scope()
        {
            var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers())
               .RegisterType<ITest1<int, string>, Test1<int, string>>(config => config.InNamedScope("A"))
               .RegisterType(typeof(ITest1<,>), typeof(Test1<,>), config => config.InNamedScope("A"));

            var res = container.BeginScope("A").Resolve<IEnumerable<ITest1<int, string>>>();

            Assert.AreEqual(2, res.Count());
        }

        [TestMethod]
        public void GenericTests_Resolve_Prefer_Valid_Constraint_In_Named_Scope()
        {
            var inst = new StashboxContainer()
               .RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest3<>), config => config.InNamedScope("A"))
               .RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest2<>), config => config.InNamedScope("A"))
               .BeginScope("A")
               .Resolve<IConstraintTest<ConstraintArgument1>>();

            Assert.IsInstanceOfType(inst, typeof(ConstraintTest3<ConstraintArgument1>));
        }

        [TestMethod]
        public void GenericTests_Resolve_Prefer_Valid_Constraint_In_Named_Scope_Enumerable()
        {
            var inst = new StashboxContainer()
               .RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest3<>), config => config.InNamedScope("A"))
               .RegisterType(typeof(IConstraintTest<>), typeof(ConstraintTest2<>), config => config.InNamedScope("A"))
               .BeginScope("A")
               .ResolveAll<IConstraintTest<ConstraintArgument1>>();

            Assert.AreEqual(1, inst.Count());
        }

        [TestMethod]
        public void GenericTests_Nested_Generics()
        {
            var inst = new StashboxContainer()
               .RegisterType(typeof(IGen1<>), typeof(Gen1<>))
               .RegisterType(typeof(IGen2<>), typeof(Gen2<>))
               .RegisterType<ConstraintArgument>()
               .Resolve<IGen2<IGen1<ConstraintArgument>>>();

            Assert.IsNotNull(inst.Value);
            Assert.IsInstanceOfType(inst.Value, typeof(Gen1<ConstraintArgument>));

            Assert.IsNotNull(inst.Value.Value);
            Assert.IsInstanceOfType(inst.Value.Value, typeof(ConstraintArgument));
        }

        [TestMethod]
        public void GenericTests_Nested_Generics_Decorator()
        {
            var inst = new StashboxContainer()
               .RegisterType(typeof(IGen3<>), typeof(Gen3<>))
               .RegisterDecorator(typeof(IGen3<>), typeof(Gen3Decorator<>))
               .RegisterType<ConstraintArgument>()
               .Resolve<IGen3<ConstraintArgument>>();

            Assert.IsNotNull(inst.Value);
            Assert.IsInstanceOfType(inst.Value, typeof(ConstraintArgument));

            var decorator = (Gen3Decorator<ConstraintArgument>)inst;

            Assert.IsNotNull(decorator.Decorated);
            Assert.IsInstanceOfType(decorator.Decorated, typeof(Gen3<ConstraintArgument>));
        }

        public interface IConstraint { }

        public interface IConstraint1 { }

        public interface IConstraintTest<T> { }

        public class ConstraintTest<T> : IConstraintTest<T> { }

        public class ConstraintTest2<T> : IConstraintTest<T> where T : IConstraint { }

        public class ConstraintTest3<T> : IConstraintTest<T> where T : IConstraint1 { }

        public class ConstraintArgument { }

        public class ConstraintArgument1 : IConstraint1 { }

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

        interface IGen1<T> { T Value { get; } }

        interface IGen2<T> { T Value { get; } }

        interface IGen3<T> { T Value { get; } }

        class Gen1<T> : IGen1<T>
        {
            public Gen1(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }

        class Gen2<T> : IGen2<T>
        {
            public Gen2(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }

        class Gen3<T> : IGen3<T>
        {
            public Gen3(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }

        class Gen3Decorator<T> : IGen3<T>
        {
            public IGen3<T> Decorated { get; }

            public Gen3Decorator(IGen3<T> value)
            {
                this.Decorated = value;
                this.Value = value.Value;
            }

            public T Value { get; }
        }
    }
}
