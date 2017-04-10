using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Entity;

namespace Stashbox.Tests
{
    [TestClass]
    public class DecoratorTests
    {
        [TestMethod]
        public void DecoratorTests_Simple()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator1>();
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator1));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Simple2()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1>(typeof(TestDecorator1));
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator1));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Simple3()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator(typeof(ITest1), typeof(TestDecorator1));
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator1));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Simple_Lazy()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator1>();
                var test = container.Resolve<Lazy<ITest1>>();

                Assert.IsNotNull(test.Value);
                Assert.IsInstanceOfType(test.Value, typeof(TestDecorator1));

                Assert.IsNotNull(test.Value.Test);
                Assert.IsInstanceOfType(test.Value.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Simple_Func()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator1>();
                var test = container.Resolve<Func<ITest1>>();

                Assert.IsNotNull(test());
                Assert.IsInstanceOfType(test(), typeof(TestDecorator1));

                Assert.IsNotNull(test().Test);
                Assert.IsInstanceOfType(test().Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Simple_Enumerable()
        {
            using (var container = new StashboxContainer(config => config.WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder)))
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterType<ITest1, Test11>();
                container.RegisterDecorator<ITest1, TestDecorator1>();
                var test = container.Resolve<IEnumerable<ITest1>>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(IEnumerable<ITest1>));

                var arr = test.ToArray();

                Assert.IsNotNull(arr[0]);
                Assert.IsInstanceOfType(arr[0], typeof(TestDecorator1));

                Assert.IsNotNull(arr[1]);
                Assert.IsInstanceOfType(arr[1], typeof(TestDecorator1));

                Assert.IsNotNull(arr[0].Test);
                Assert.IsInstanceOfType(arr[0].Test, typeof(Test1));

                Assert.IsNotNull(arr[1].Test);
                Assert.IsInstanceOfType(arr[1].Test, typeof(Test11));
            }
        }

        [TestMethod]
        public void DecoratorTests_Decorator_Holds_Lazy()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator6>();
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator6));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Decorator_Holds_Func()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator7>();
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator7));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Decorator_Holds_Enumerable()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator8>();
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator8));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Dependency()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator3>();
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator3));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_AutoMemberInjection()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator4>(context => context.WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess));
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator4));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_AutoMemberInjection_InjectionParameter()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator4>(context => context
                    .WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess)
                    .WithInjectionParameters(new InjectionParameter { Name = "Name", Value = "test" }));
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator4));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
                Assert.AreEqual("test", ((TestDecorator4)test).Name);
            }
        }

        [TestMethod]
        public void DecoratorTests_ConstructorSelection_LessParameters()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator5>(context => context.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters));
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator5));

                Assert.IsNull(test.Test);
            }
        }

        [TestMethod]
        public void DecoratorTests_ConstructorSelection_MostParameters()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator5>(context => context.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters));
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator5));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Multiple()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator1>();
                container.RegisterDecorator<ITest1, TestDecorator2>();
                var test = container.Resolve<ITest1>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator2));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(TestDecorator1));

                Assert.IsNotNull(test.Test.Test);
                Assert.IsInstanceOfType(test.Test.Test, typeof(Test1));
            }
        }

        [TestMethod]
        public void DecoratorTests_Multiple_Scoped()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterScoped<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator1>();
                container.RegisterDecorator<ITest1, TestDecorator2>();

                using (var child = container.BeginScope())
                {
                    var test = child.Resolve<ITest1>();

                    Assert.IsNotNull(test);
                    Assert.IsInstanceOfType(test, typeof(TestDecorator2));

                    Assert.IsNotNull(test.Test);
                    Assert.IsInstanceOfType(test.Test, typeof(TestDecorator1));

                    Assert.IsNotNull(test.Test.Test);
                    Assert.IsInstanceOfType(test.Test.Test, typeof(Test1));
                }
            }
        }

        [TestMethod]
        public void DecoratorTests_OpenGeneric()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType(typeof(ITest1<>), typeof(Test1<>));
                container.RegisterDecorator(typeof(ITest1<>), typeof(TestDecorator1<>));
                var test = container.Resolve<ITest1<int>>();

                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(TestDecorator1<int>));

                Assert.IsNotNull(test.Test);
                Assert.IsInstanceOfType(test.Test, typeof(Test1<int>));
            }
        }

        [TestMethod]
        public void DecoratorTests_DecoratorDependency_Null()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test1>();
                container.RegisterDecorator<ITest1, TestDecorator9>();

                var inst = container.Resolve<ITest1>(nullResultAllowed: true);

                Assert.IsNull(inst);
            }
        }

        [TestMethod]
        public void DecoratorTests_DecoreteeDependency_Null()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<ITest1, Test12>();
                container.RegisterDecorator<ITest1, TestDecorator9>();

                var inst = container.Resolve<ITest1>(nullResultAllowed: true);

                Assert.IsNull(inst);
            }
        }

        [TestMethod]
        public void DecoratorTests_Disposed()
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.RegisterType<IDisp, TestDisp>();
                container.RegisterDecorator<IDisp, TestDispDecorator>();

                test = container.Resolve<IDisp>();

                Assert.IsNotNull(test);
                Assert.IsNotNull(test.Test);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test.Test.Disposed);
        }

        [TestMethod]
        public void DecoratorTests_Disposed_OnlyDecoreteeDisposal()
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.RegisterType<IDisp, TestDisp>(context => context.WithoutDisposalTracking());
                container.RegisterDecorator<IDisp, TestDispDecorator>();

                test = container.Resolve<IDisp>();

                Assert.IsNotNull(test);
                Assert.IsNotNull(test.Test);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsFalse(test.Test.Disposed);
        }

        [TestMethod]
        public void DecoratorTests_Disposed_BothDisposal()
        {
            IDisp test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.RegisterType<IDisp, TestDisp>(context => context.WithoutDisposalTracking());
                container.RegisterDecorator<IDisp, TestDispDecorator>(context => context.WithoutDisposalTracking());

                test = container.Resolve<IDisp>();

                Assert.IsNotNull(test);
                Assert.IsNotNull(test.Test);
            }

            Assert.IsFalse(test.Disposed);
            Assert.IsFalse(test.Test.Disposed);
        }

        public interface ITest1 { ITest1 Test { get; } }

        public interface IDecoratorDep { }

        public interface IDep { }

        public interface ITest1<T> { ITest1<T> Test { get; } }

        public interface IDisp : IDisposable
        {
            IDisp Test { get; }

            bool Disposed { get; }
        }
        
        public class Test1 : ITest1
        {
            public ITest1 Test { get; }
        }

        public class Test11 : ITest1
        {
            public ITest1 Test { get; }
        }

        public class Test12 : ITest1
        {
            public ITest1 Test { get; }

            public Test12(IDep dep)
            {
                
            }
        }

        public class TestDisp : IDisp
        {
            public void Dispose()
            {
                if(this.Disposed)
                    throw new ObjectDisposedException(nameof(TestDisp));

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
            public IDisp Test { get; }
        }

        public class TestDispDecorator : IDisp
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

        public class Test1<T> : ITest1<T>
        {
            public ITest1<T> Test { get; }
        }

        public class TestDecorator1<T> : ITest1<T>
        {
            public ITest1<T> Test { get; }

            public TestDecorator1(ITest1<T> test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator1 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator1(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator2 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator2(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator3 : ITest1
        {
            [Dependency]
            public ITest1 Test { get; set; }
        }

        public class TestDecorator4 : ITest1
        {
            public string Name { get; private set; }

            public ITest1 Test { get; private set; }
        }

        public class TestDecorator5 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator5()
            { }

            public TestDecorator5(ITest1 test1)
            {
                this.Test = test1;
            }
        }

        public class TestDecorator6 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator6(Lazy<ITest1> test1)
            {
                this.Test = test1.Value;
            }
        }

        public class TestDecorator7 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator7(Func<ITest1> test1)
            {
                this.Test = test1();
            }
        }

        public class TestDecorator8 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator8(IEnumerable<ITest1> test1)
            {
                this.Test = test1.First();
            }
        }

        public class TestDecorator9 : ITest1
        {
            public ITest1 Test { get; }

            public TestDecorator9(ITest1 test1, IDecoratorDep dep)
            {
                this.Test = test1;
            }
        }
    }
}
