using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{

    public class GenericTests
    {
        [Fact]
        public void GenericTests_Resolve()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.NotNull(inst);
                Assert.IsType<Test1<int, string>>(inst);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Singleton()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                var en = container.Resolve<IEnumerable<ITest1<int, string>>>();
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.Equal(inst, en.ToArray()[0]);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Singleton_Many()
        {
            using (var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers()))
            {
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                var en = container.Resolve<IEnumerable<ITest1<int, string>>>();
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.Equal(inst, en.ToArray()[1]);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Singleton_Many_Mixed()
        {
            using (var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers()))
            {
                var inst = new Test1<int, string>();
                container.RegisterSingleton(typeof(ITest1<int, string>), typeof(Test1<int, string>));
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                container.Register(typeof(ITest1<int, string>), typeof(Test1<int, string>), config => config.WithInstance(inst));
                var en = container.Resolve<IEnumerable<ITest1<int, string>>>().ToArray();

                Assert.Equal(3, en.Length);
                Assert.Equal(inst, en[2]);
            }
        }

        [Fact]
        public void GenericTests_Resolve_SameTime_DifferentParameter()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();
                var inst2 = container.Resolve<ITest1<int, int>>();

                Assert.NotNull(inst);
                Assert.IsType<Test1<int, string>>(inst);

                Assert.NotNull(inst2);
                Assert.IsType<Test1<int, int>>(inst2);
            }
        }

        [Fact]
        public void GenericTests_Resolve_SameTime_DifferentParameter_Singleton()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterSingleton(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();
                var inst2 = container.Resolve<ITest1<int, int>>();

                Assert.NotNull(inst);
                Assert.IsType<Test1<int, string>>(inst);

                Assert.NotNull(inst2);
                Assert.IsType<Test1<int, int>>(inst2);
            }
        }

        [Fact]
        public void GenericTests_CanResolve()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                Assert.True(container.CanResolve(typeof(ITest1<,>)));
                Assert.True(container.CanResolve(typeof(ITest1<int, int>)));
            }
        }

        [Fact]
        public void GenericTests_Named()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>), config => config.WithName("G1"));
                container.Register(typeof(ITest1<,>), typeof(Test12<,>), config => config.WithName("G2"));

                Assert.IsType<Test1<int, string>>(container.Resolve<ITest1<int, string>>("G1"));
                Assert.IsType<Test12<int, double>>(container.Resolve<ITest1<int, double>>("G2"));
            }
        }

        [Fact]
        public void GenericTests_DependencyResolve()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                container.Register(typeof(ITest2<,>), typeof(Test2<,>));
                var inst = container.Resolve<ITest2<int, string>>();

                Assert.NotNull(inst.Test);
                Assert.IsType<Test1<int, string>>(inst.Test);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Fluent()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.NotNull(inst);
                Assert.IsType<Test1<int, string>>(inst);
            }
        }

        [Fact]
        public void GenericTests_Resolve_ReMap()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                container.ReMap(typeof(ITest1<,>), typeof(Test12<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.NotNull(inst);
                Assert.IsType<Test12<int, string>>(inst);
            }
        }

        [Fact]
        public void GenericTests_Resolve_ReMap_Fluent()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));
                container.ReMap(typeof(ITest1<,>), typeof(Test12<,>));
                var inst = container.Resolve<ITest1<int, string>>();

                Assert.NotNull(inst);
                Assert.IsType<Test12<int, string>>(inst);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Parallel()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(ITest1<,>), typeof(Test1<,>));

                Parallel.For(0, 50000, (i) =>
                {
                    var inst = container.Resolve<ITest1<int, string>>();

                    Assert.NotNull(inst);
                    Assert.IsType<Test1<int, string>>(inst);
                });
            }
        }

        [Fact]
        public void GenericTests_Resolve_Constraint_Array()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest<>));
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));

                var inst = container.ResolveAll<IConstraintTest<ConstraintArgument>>().ToArray();
                Assert.Single(inst);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Constraint_Multiple()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest<>));

                var inst = container.Resolve<IConstraintTest<ConstraintArgument>>();
                Assert.IsType<ConstraintTest<ConstraintArgument>>(inst);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Constraint()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                Assert.Throws<ArgumentException>(() => container.Resolve<IConstraintTest<ConstraintArgument>>());
            }
        }

        [Fact]
        public void GenericTests_Resolve_Constraint_Constructor()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest<>));
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                container.Register<ConstraintTest3>();
                var inst = container.Resolve<ConstraintTest3>();

                Assert.IsType<ConstraintTest<ConstraintArgument>>(inst.Test);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Constraint_Pick_RightImpl()
        {
            using (var container = new StashboxContainer())
            {
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest2<>));
                container.Register(typeof(IConstraintTest<>), typeof(ConstraintTest3<>));

                var inst = container.Resolve<IConstraintTest<ConstraintArgument1>>();
                Assert.IsType<ConstraintTest3<ConstraintArgument1>>(inst);
            }
        }

        [Fact]
        public void GenericTests_Resolve_Prefer_Open_Generic_In_Named_Scope()
        {
            var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers())
               .Register<ITest1<int, string>, Test1<int, string>>()
               .Register(typeof(ITest1<,>), typeof(Test1<,>), config => config.InNamedScope("A"));

            container.BeginScope("A").Resolve<ITest1<int, string>>();

            Assert.Equal(3, container.ContainerContext.RegistrationRepository.GetRegistrationMappings().Count());
        }

        [Fact]
        public void GenericTests_Resolve_Prefer_Open_Generic_Enumerable_In_Named_Scope()
        {
            var container = new StashboxContainer(config => config.WithUniqueRegistrationIdentifiers())
               .Register<ITest1<int, string>, Test1<int, string>>(config => config.InNamedScope("A"))
               .Register(typeof(ITest1<,>), typeof(Test1<,>), config => config.InNamedScope("A"));

            var res = container.BeginScope("A").Resolve<IEnumerable<ITest1<int, string>>>();

            Assert.Equal(2, res.Count());
        }

        [Fact]
        public void GenericTests_Resolve_Prefer_Valid_Constraint_In_Named_Scope()
        {
            var inst = new StashboxContainer()
               .Register(typeof(IConstraintTest<>), typeof(ConstraintTest3<>), config => config.InNamedScope("A"))
               .Register(typeof(IConstraintTest<>), typeof(ConstraintTest2<>), config => config.InNamedScope("A"))
               .BeginScope("A")
               .Resolve<IConstraintTest<ConstraintArgument1>>();

            Assert.IsType<ConstraintTest3<ConstraintArgument1>>(inst);
        }

        [Fact]
        public void GenericTests_Resolve_Prefer_Valid_Constraint_In_Named_Scope_Enumerable()
        {
            var inst = new StashboxContainer()
               .Register(typeof(IConstraintTest<>), typeof(ConstraintTest3<>), config => config.InNamedScope("A"))
               .Register(typeof(IConstraintTest<>), typeof(ConstraintTest2<>), config => config.InNamedScope("A"))
               .BeginScope("A")
               .ResolveAll<IConstraintTest<ConstraintArgument1>>();

            Assert.Single(inst);
        }

        [Fact]
        public void GenericTests_Nested_Generics()
        {
            var inst = new StashboxContainer()
               .Register(typeof(IGen1<>), typeof(Gen1<>))
               .Register(typeof(IGen2<>), typeof(Gen2<>))
               .Register<ConstraintArgument>()
               .Resolve<IGen2<IGen1<ConstraintArgument>>>();

            Assert.NotNull(inst.Value);
            Assert.IsType<Gen1<ConstraintArgument>>(inst.Value);

            Assert.NotNull(inst.Value.Value);
            Assert.IsType<ConstraintArgument>(inst.Value.Value);
        }

        [Fact]
        public void GenericTests_Nested_Generics_Decorator()
        {
            var inst = new StashboxContainer()
               .Register(typeof(IGen3<>), typeof(Gen3<>))
               .RegisterDecorator(typeof(IGen3<>), typeof(Gen3Decorator<>))
               .Register<ConstraintArgument>()
               .Resolve<IGen3<ConstraintArgument>>();

            Assert.NotNull(inst.Value);
            Assert.IsType<ConstraintArgument>(inst.Value);

            var decorator = (Gen3Decorator<ConstraintArgument>)inst;

            Assert.NotNull(decorator.Decorated);
            Assert.IsType<Gen3<ConstraintArgument>>(decorator.Decorated);
        }

        [Fact]
        public void GenericTests_Nested_Within_Same_Request()
        {
            var inst = new StashboxContainer()
                .Register<Gen>()
                .Register(typeof(IGen<>), typeof(Gen<>))
                .Resolve<Gen>();

            Assert.NotNull(inst);
        }

        [Fact]
        public void PublicNoArgCtorConstrainedOpenGenericServicesCanBeResolved()
        {
            // Arrange
            var container = new StashboxContainer()
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithNoConstraints<>))
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithNewConstraint<>));
           
            // Act
            var allServices = container.ResolveAll<IFakeOpenGenericService<PocoClass>>().ToList();
            var constrainedServices = container.ResolveAll<IFakeOpenGenericService<ClassWithPrivateCtor>>().ToList();
            // Assert
            Assert.Equal(2, allServices.Count);
            Assert.Single(constrainedServices);
        }

        [Fact]
        public void ClassConstrainedOpenGenericServicesCanBeResolved()
        {
            // Arrange
            var container = new StashboxContainer()
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithNoConstraints<>))
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithClassConstraint<>));

            // Act
            var allServices = container.ResolveAll<IFakeOpenGenericService<PocoClass>>().ToList();
            var constrainedServices = container.ResolveAll<IFakeOpenGenericService<int>>().ToList();
            // Assert
            Assert.Equal(2, allServices.Count);
            Assert.Single(constrainedServices);
        }

        [Fact]
        public void SelfReferencingConstrainedOpenGenericServicesCanBeResolved()
        {
            // Arrange
            var container = new StashboxContainer()
                .Register(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>))
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithSelfReferencingConstraint<>));
            var poco = new PocoClass();
            container.RegisterInstance(poco);
            var selfComparable = new ClassImplementingIComparable();
            container.RegisterInstance(selfComparable);
            // Act
            var allServices = container.ResolveAll<IFakeOpenGenericService<ClassImplementingIComparable>>().ToList();
            var constrainedServices = container.ResolveAll<IFakeOpenGenericService<PocoClass>>().ToList();
            // Assert
            Assert.Equal(2, allServices.Count);
            Assert.Same(selfComparable, allServices[0].Value);
            Assert.Same(selfComparable, allServices[1].Value);
            Assert.Single(constrainedServices);
            Assert.Same(poco, constrainedServices[0].Value);
        }

        interface IConstraint { }

        interface IConstraint1 { }

        interface IConstraintTest<T> { }

        class ConstraintTest<T> : IConstraintTest<T> { }

        class ConstraintTest2<T> : IConstraintTest<T> where T : IConstraint { }

        class ConstraintTest3<T> : IConstraintTest<T> where T : IConstraint1 { }

        class ConstraintArgument { }

        class ConstraintArgument1 : IConstraint1 { }

        class ConstraintTest3
        {
            public IConstraintTest<ConstraintArgument> Test { get; set; }

            public ConstraintTest3(IConstraintTest<ConstraintArgument> test)
            {
                this.Test = test;
            }
        }

        interface ITest1<I, K>
        {
            I IProp { get; }
            K KProp { get; }
        }

        interface ITest2<I, K>
        {
            ITest1<I, K> Test { get; }
        }

        class Test1<I, K> : ITest1<I, K>
        {
            public I IProp { get; }
            public K KProp { get; }
        }

        class Test12<I, K> : ITest1<I, K>
        {
            public I IProp { get; }
            public K KProp { get; }
        }

        class Test2<I, K> : ITest2<I, K>
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

        class Stub { }
        class Stub1 { }

        interface IGen<T>
        { }

        class Gen<T> : IGen<T> { }

        class Gen
        {
            public Gen(IGen<Stub> stub, IGen<Stub1> stub1)
            { }
        }

        interface IFakeOpenGenericService<out TValue>
        {
            TValue Value { get; }
        }

        class ClassWithNoConstraints<T> : IFakeOpenGenericService<T>
        {
            public T Value { get; } = default;
        }

        class ClassWithNewConstraint<T> : IFakeOpenGenericService<T>
            where T : new()
        {
            public T Value { get; } = new T();
        }

        class PocoClass
        {
        }

        class FakeOpenGenericService<TVal> : IFakeOpenGenericService<TVal>
        {
            public FakeOpenGenericService(TVal value)
            {
                Value = value;
            }

            public TVal Value { get; }
        }

        class ClassWithSelfReferencingConstraint<T> : IFakeOpenGenericService<T>
            where T : IComparable<T>
        {
            public ClassWithSelfReferencingConstraint(T value) => Value = value;

            public T Value { get; }
        }

        class ClassImplementingIComparable : IComparable<ClassImplementingIComparable>
        {
            public int CompareTo(ClassImplementingIComparable other) => 0;
        }

        class ClassWithClassConstraint<T> : IFakeOpenGenericService<T>
            where T : class
        {
            public T Value { get; } = default;
        }

        class ClassWithPrivateCtor
        {
            private ClassWithPrivateCtor()
            {
            }
        }
    }
}
