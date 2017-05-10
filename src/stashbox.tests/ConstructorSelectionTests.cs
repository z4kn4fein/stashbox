using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Exceptions;

namespace Stashbox.Tests
{
    [TestClass]
    public class ConstructorSelectionTests
    {
        [TestMethod]
        public void ConstructorSelectionTests_ArgTypes()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                container.RegisterType<Test>(context => context.WithConstructorByArgumentTypes(typeof(Dep), typeof(Dep2)));
                container.Resolve<Test>();
            }
        }

        [TestMethod]
        public void ConstructorSelectionTests_Args()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Dep();
                var dep2 = new Dep2();

                container.RegisterType<Test>(context => context.WithConstructorByArguments(dep, dep2));
                var test = container.Resolve<Test>();

                Assert.AreSame(dep, test.Dep);
                Assert.AreSame(dep2, test.Dep2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ConstructorSelectionTests_ArgTypes_Throws_ResolutionFailed()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>(context => context.WithConstructorByArgumentTypes(typeof(Dep), typeof(Dep2)));
                container.Resolve<Test>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConstructorNotFoundException))]
        public void ConstructorSelectionTests_ArgTypes_Throws_MissingConstructor()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>(context => context.WithConstructorByArgumentTypes());
                container.Resolve<Test>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConstructorNotFoundException))]
        public void ConstructorSelectionTests_Args_Throws_MissingConstructor()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>(context => context.WithConstructorByArguments());
                container.Resolve<Test>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConstructorNotFoundException))]
        public void ConstructorSelectionTests_Args_Throws_MissingConstructor_OneParam()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>(context => context.WithConstructorByArguments(new object()));
                container.Resolve<Test>();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConstructorNotFoundException))]
        public void ConstructorSelectionTests_Args_Throws_MissingConstructor_MoreParams()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test>(context => context.WithConstructorByArguments(new object(), new object()));
                container.Resolve<Test>();
            }
        }

        [TestMethod]
        public void ConstructorSelectionTests_Decorator_ArgTypes()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                container.RegisterDecorator<Dep, DepDecorator>(context => context.WithConstructorByArgumentTypes(typeof(Dep), typeof(Dep2)));
                var test = container.Resolve<Dep>();

                Assert.IsInstanceOfType(test, typeof(DepDecorator));
            }
        }

        [TestMethod]
        public void ConstructorSelectionTests_Decorator_Args()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                var dep = new Dep();
                var dep2 = new Dep2();

                container.RegisterDecorator<Dep, DepDecorator>(context => context.WithConstructorByArguments(dep, dep2));
                var test = container.Resolve<Dep>();

                Assert.AreSame(dep, ((DepDecorator)test).Dep);
                Assert.AreSame(dep2, ((DepDecorator)test).Dep2);
            }
        }

        public class Dep
        { }

        public class Dep2
        { }

        public class Dep3
        { }

        public class DepDecorator : Dep
        {
            public Dep Dep { get; }
            public Dep2 Dep2 { get; }

            public DepDecorator(Dep dep)
            {
                Assert.Fail("wrong constructor");
            }

            public DepDecorator(Dep dep, Dep2 dep2)
            {
                this.Dep = dep;
                this.Dep2 = dep2;
            }

            public DepDecorator(Dep dep, Dep2 dep2, Dep3 dep3)
            {
                Assert.Fail("wrong constructor");
            }
        }

        public class Test
        {
            public Dep Dep { get; }
            public Dep2 Dep2 { get; }

            public Test(Dep dep)
            {
                Assert.Fail("wrong constructor");
            }

            public Test(Dep dep, Dep2 dep2)
            {
                this.Dep = dep;
                this.Dep2 = dep2;
            }

            public Test(Dep dep, Dep2 dep2, Dep3 dep3)
            {
                Assert.Fail("wrong constructor");
            }
        }
    }
}
