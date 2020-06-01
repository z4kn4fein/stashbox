using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests
{

    public class DecoratorTests
    {
        [Fact]
        public void DecoratorTests_Simple()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Simple2()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1>(typeof(TestDecorator1));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Simple3()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Simple_Lazy()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<Lazy<ITest1>>();

            Assert.NotNull(test.Value);
            Assert.IsType<TestDecorator1>(test.Value);

            Assert.NotNull(test.Value.Test);
            Assert.IsType<Test1>(test.Value.Test);
        }

        [Fact]
        public void DecoratorTests_Simple_Func()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<Func<ITest1>>();

            Assert.NotNull(test());
            Assert.IsType<TestDecorator1>(test());

            Assert.NotNull(test().Test);
            Assert.IsType<Test1>(test().Test);
        }

        [Fact]
        public void DecoratorTests_Simple_Enumerable()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.Register<ITest1, Test11>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            var test = container.Resolve<IEnumerable<ITest1>>();

            Assert.NotNull(test);
            Assert.IsAssignableFrom<IEnumerable<ITest1>>(test);

            var arr = test.ToArray();

            Assert.NotNull(arr[0]);
            Assert.IsType<TestDecorator1>(arr[0]);

            Assert.NotNull(arr[1]);
            Assert.IsType<TestDecorator1>(arr[1]);

            Assert.NotNull(arr[0].Test);
            Assert.IsType<Test1>(arr[0].Test);

            Assert.NotNull(arr[1].Test);
            Assert.IsType<Test11>(arr[1].Test);
        }

        [Fact]
        public void DecoratorTests_Decorator_Holds_Lazy()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator6>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator6>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Decorator_Holds_Func()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator7>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator7>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Decorator_Holds_Enumerable()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator8>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator8>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Dependency()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator3>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator3>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Inject_Member_With_Config()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator3Attributeless>(config => config.InjectMember("Test"));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator3Attributeless>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_AutoMemberInjection_Throw_When_Member_Unresolvable()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator4>(context => context.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));
            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest1>());
        }

        [Fact]
        public void DecoratorTests_AutoMemberInjection_InjectionParameter()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator4>(context => context
                .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess)
                .WithInjectionParameter("Name", "test"));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator4>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
            Assert.Equal("test", ((TestDecorator4)test).Name);
        }

        [Fact]
        public void DecoratorTests_AutoMemberInjection_InjectionParameter_Fluent()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator4>(context => context
                .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess)
                .WithInjectionParameter("Name", "test"));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator4>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
            Assert.Equal("test", ((TestDecorator4)test).Name);
        }

        [Fact]
        public void DecoratorTests_ConstructorSelection_LeastParameters()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator5>(context => context.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator5>(test);

            Assert.Null(test.Test);
        }

        [Fact]
        public void DecoratorTests_ConstructorSelection_MostParameters()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator5>(context => context.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters));
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator5>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Multiple()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();
            var test = container.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator2>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<TestDecorator1>(test.Test);

            Assert.NotNull(test.Test.Test);
            Assert.IsType<Test1>(test.Test.Test);
        }

        [Fact]
        public void DecoratorTests_Multiple_Scoped()
        {
            using var container = new StashboxContainer();
            container.RegisterScoped<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            using var child = container.BeginScope();
            var test = child.Resolve<ITest1>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator2>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<TestDecorator1>(test.Test);

            Assert.NotNull(test.Test.Test);
            Assert.IsType<Test1>(test.Test.Test);
        }

        [Fact]
        public void DecoratorTests_OpenGeneric()
        {
            using var container = new StashboxContainer();
            container.Register(typeof(ITest1<>), typeof(Test1<>));
            container.RegisterDecorator(typeof(ITest1<>), typeof(TestDecorator1<>));
            var test = container.Resolve<ITest1<int>>();

            Assert.NotNull(test);
            Assert.IsType<TestDecorator1<int>>(test);

            Assert.NotNull(test.Test);
            Assert.IsType<Test1<int>>(test.Test);
        }

        [Fact]
        public void DecoratorTests_DecoratorDependency_Null()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator9>();

            var inst = container.Resolve<ITest1>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void DecoratorTests_DecoreteeDependency_Null()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test12>();
            container.RegisterDecorator<ITest1, TestDecorator9>();

            var inst = container.Resolve<ITest1>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void DecoratorTests_Disposed()
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<IDisp, TestDisp>();
                container.RegisterDecorator<IDisp, TestDispDecorator>();

                test = container.Resolve<IDisp>();

                Assert.NotNull(test);
                Assert.NotNull(test.Test);
            }

            Assert.True(test.Disposed);
            Assert.True(test.Test.Disposed);
        }

        [Fact]
        public void DecoratorTests_Disposed_OnlyDecoreteeDisposal()
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<IDisp, TestDisp>(context => context.WithoutDisposalTracking());
                container.RegisterDecorator<IDisp, TestDispDecorator>();

                test = container.Resolve<IDisp>();

                Assert.NotNull(test);
                Assert.NotNull(test.Test);
            }

            Assert.True(test.Disposed);
            Assert.False(test.Test.Disposed);
        }

        [Fact]
        public void DecoratorTests_Disposed_BothDisposal()
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<IDisp, TestDisp>(context => context.WithoutDisposalTracking());
                container.RegisterDecorator<IDisp, TestDispDecorator>(context => context.WithoutDisposalTracking());

                test = container.Resolve<IDisp>();

                Assert.NotNull(test);
                Assert.NotNull(test.Test);
            }

            Assert.False(test.Disposed);
            Assert.False(test.Test.Disposed);
        }

        [Fact]
        public void DecoratorTests_ReplaceDecorator()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator1>(test);
            Assert.IsType<Test1>(test.Test);

            container.RegisterDecorator<ITest1, TestDecorator1>(context => context.ReplaceExisting());

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator1>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_RemapDecorator()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(test);
            Assert.IsType<TestDecorator1>(test.Test);
            Assert.IsType<Test1>(test.Test.Test);

            container.ReMapDecorator<ITest1, TestDecorator3>();

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator3>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_RemapDecorator_V2()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(test);
            Assert.IsType<TestDecorator1>(test.Test);
            Assert.IsType<Test1>(test.Test.Test);

            container.ReMapDecorator<ITest1>(typeof(TestDecorator3));

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator3>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_RemapDecorator_WithConfig()
        {
            using var container = new StashboxContainer();
            container.Register<ITest1, Test1>();
            container.RegisterDecorator<ITest1, TestDecorator1>();
            container.RegisterDecorator<ITest1, TestDecorator2>();

            var test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator2>(test);
            Assert.IsType<TestDecorator1>(test.Test);
            Assert.IsType<Test1>(test.Test.Test);

            container.ReMapDecorator(typeof(ITest1), typeof(TestDecorator3), context => context.WithoutDisposalTracking());

            test = container.Resolve<ITest1>();

            Assert.IsType<TestDecorator3>(test);
            Assert.IsType<Test1>(test.Test);
        }

        [Fact]
        public void DecoratorTests_Service_ImplementationType()
        {
            using var container = new StashboxContainer();
            container.RegisterDecorator<ITest1, TestDecorator1>(context =>
            {
                Assert.Equal(typeof(ITest1), context.ServiceType);
                Assert.Equal(typeof(TestDecorator1), context.ImplementationType);
            });
        }

        interface ITest1 { ITest1 Test { get; } }

        interface IDecoratorDep { }

        interface IDep { }

        interface ITest1<T> { ITest1<T> Test { get; } }

        interface IDisp : IDisposable
        {
            IDisp Test { get; }

            bool Disposed { get; }
        }

        class Test1 : ITest1
        {
            public ITest1 Test { get; }
        }

        class Test11 : ITest1
        {
            public ITest1 Test { get; }
        }

        class Test12 : ITest1
        {
            public ITest1 Test { get; }

            public Test12(IDep dep)
            {

            }
        }

        class TestDisp : IDisp
        {
            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(TestDisp));

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
            public IDisp Test { get; }
        }

        class TestDispDecorator : IDisp
        {
            public TestDispDecorator(IDisp disp)
            {
                this.Test = disp;
            }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(TestDisp));

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
            public IDisp Test { get; }
        }

        class Test1<T> : ITest1<T>
        {
            public ITest1<T> Test { get; }
        }

        class TestDecorator1<T> : ITest1<T>
        {
            public ITest1<T> Test { get; }

            public TestDecorator1(ITest1<T> test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator1 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator1(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator2 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator2(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator3 : ITest1
        {
            [Dependency]
            public ITest1 Test { get; set; }
        }

        class TestDecorator3Attributeless : ITest1
        {
            public ITest1 Test { get; set; }
        }

        class TestDecorator4 : ITest1
        {
            public string Name { get; private set; }

            public ITest1 Test { get; private set; }
        }

        class TestDecorator5 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator5()
            { }

            public TestDecorator5(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        class TestDecorator6 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator6(Lazy<ITest1> test1)
            {
                this.Test = test1.Value;
            }
        }

        class TestDecorator7 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator7(Func<ITest1> test1)
            {
                this.Test = test1();
            }
        }

        class TestDecorator8 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator8(IEnumerable<ITest1> test1)
            {
                this.Test = test1.First();
            }
        }

        class TestDecorator9 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator9(ITest1 test1, IDecoratorDep dep)
            {
                this.Test = test1;
            }
        }
    }
}
