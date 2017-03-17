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
                container.PrepareDecorator<ITest1, TestDecorator4>()
                    .WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess)
                    .Register();
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
                container.PrepareDecorator<ITest1, TestDecorator4>()
                    .WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess)
                    .WithInjectionParameters(new InjectionParameter { Name = "Name", Value = "test" })
                    .Register();
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
                container.PrepareDecorator<ITest1, TestDecorator5>()
                    .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLessParameters)
                    .Register();
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
                container.PrepareDecorator<ITest1, TestDecorator5>()
                    .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters)
                    .Register();
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

        public interface ITest1 { ITest1 Test { get; } }

        public class Test1 : ITest1
        {
            public ITest1 Test { get; }
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
    }
}
