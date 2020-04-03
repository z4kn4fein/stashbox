using Xunit;

namespace Stashbox.Tests
{

    public class ResolveFactoryTests
    {
        [Fact]
        public void ResolveFactoryTests_ParameterLess()
        {
            using var container = new StashboxContainer();
            container.Register<Test>();
            var factory = container.ResolveFactory<Test>();

            Assert.NotNull(factory());
        }

        [Fact]
        public void ResolveFactoryTests_ParameterLess_Named()
        {
            using var container = new StashboxContainer();
            container.Register<IService, Service>(c => c.WithName("service"));
            container.Register<IService, Service1>(c => c.WithName("service1"));
            var factory = container.ResolveFactory<IService>("service");

            Assert.IsType<Service>(factory());
        }

        [Fact]
        public void ResolveFactoryTests_ParameterLess_Scoped()
        {
            using var container = new StashboxContainer();
            container.Register<Test>();

            using var scope = container.BeginScope();
            var factory = scope.ResolveFactory<Test>();

            Assert.NotNull(factory());
        }

        [Fact]
        public void ResolveFactoryTests_OneParam()
        {
            using var container = new StashboxContainer();
            container.Register<Test1>();
            var factory = container.ResolveFactory<Test, Test1>();

            var test = new Test();
            var inst = factory(test);

            Assert.Same(test, inst.Test);
        }

        [Fact]
        public void ResolveFactoryTests_OneParam_Scoped()
        {
            using var container = new StashboxContainer();
            container.Register<Test1>();

            using var scope = container.BeginScope();
            var factory = scope.ResolveFactory<Test, Test1>();

            var test = new Test();
            var inst = factory(test);

            Assert.Same(test, inst.Test);
        }

        [Fact]
        public void ResolveFactoryTests_TwoParams()
        {
            using var container = new StashboxContainer();
            container.Register<Test2>();
            var factory = container.ResolveFactory<Test, Test1, Test2>();

            var test = new Test();
            var test1 = new Test1(test);
            var inst = factory(test, test1);

            Assert.Same(test, inst.Test);
            Assert.Same(test1, inst.Test1);
        }

        [Fact]
        public void ResolveFactoryTests_TwoParams_Scoped()
        {
            using var container = new StashboxContainer();
            container.Register<Test2>();

            using var scope = container.BeginScope();
            var factory = scope.ResolveFactory<Test, Test1, Test2>();

            var test = new Test();
            var test1 = new Test1(test);
            var inst = factory(test, test1);

            Assert.Same(test, inst.Test);
            Assert.Same(test1, inst.Test1);
        }

        [Fact]
        public void ResolveFactoryTests_ThreeParams()
        {
            using var container = new StashboxContainer();
            container.Register<Test3>();
            var factory = container.ResolveFactory<Test, Test1, Test2, Test3>();

            var test = new Test();
            var test1 = new Test1(test);
            var test2 = new Test2(test1, test);
            var inst = factory(test, test1, test2);

            Assert.Same(test, inst.Test);
            Assert.Same(test1, inst.Test1);
            Assert.Same(test2, inst.Test2);
        }

        [Fact]
        public void ResolveFactoryTests_ThreeParams_Scoped()
        {
            using var container = new StashboxContainer();
            container.Register<Test3>();

            using var scope = container.BeginScope();
            var factory = scope.ResolveFactory<Test, Test1, Test2, Test3>();

            var test = new Test();
            var test1 = new Test1(test);
            var test2 = new Test2(test1, test);
            var inst = factory(test, test1, test2);

            Assert.Same(test, inst.Test);
            Assert.Same(test1, inst.Test1);
            Assert.Same(test2, inst.Test2);
        }

        [Fact]
        public void ResolveFactoryTests_FourParams()
        {
            using var container = new StashboxContainer();
            container.Register<Test4>();
            var factory = container.ResolveFactory<Test, Test1, Test2, Test3, Test4>();

            var test = new Test();
            var test1 = new Test1(test);
            var test2 = new Test2(test1, test);
            var test3 = new Test3(test1, test, test2);
            var inst = factory(test, test1, test2, test3);

            Assert.Same(test, inst.Test);
            Assert.Same(test1, inst.Test1);
            Assert.Same(test2, inst.Test2);
            Assert.Same(test3, inst.Test3);
        }

        [Fact]
        public void ResolveFactoryTests_FourParams_Scoped()
        {
            using var container = new StashboxContainer();
            container.Register<Test4>();

            using var scope = container.BeginScope();
            var factory = scope.ResolveFactory<Test, Test1, Test2, Test3, Test4>();

            var test = new Test();
            var test1 = new Test1(test);
            var test2 = new Test2(test1, test);
            var test3 = new Test3(test1, test, test2);
            var inst = factory(test, test1, test2, test3);

            Assert.Same(test, inst.Test);
            Assert.Same(test1, inst.Test1);
            Assert.Same(test2, inst.Test2);
            Assert.Same(test3, inst.Test3);
        }

        interface IService
        { }

        class Service : IService
        { }

        class Service1 : IService
        { }

        class Test
        { }

        class Test1
        {
            public Test1(Test test)
            {
                this.Test = test;
            }

            public Test Test { get; }
        }

        class Test2
        {
            public Test2(Test1 test1, Test test)
            {
                this.Test1 = test1;
                this.Test = test;
            }

            public Test1 Test1 { get; }
            public Test Test { get; }
        }

        class Test3
        {
            public Test3(Test1 test1, Test test, Test2 test2)
            {
                this.Test1 = test1;
                this.Test = test;
                this.Test2 = test2;
            }

            public Test1 Test1 { get; }
            public Test Test { get; }
            public Test2 Test2 { get; }
        }

        class Test4
        {
            public Test4(Test1 test1, Test test, Test2 test2, Test3 test3)
            {
                this.Test1 = test1;
                this.Test = test;
                this.Test2 = test2;
                this.Test3 = test3;
            }

            public Test1 Test1 { get; }
            public Test Test { get; }
            public Test2 Test2 { get; }
            public Test3 Test3 { get; }
        }
    }
}
