using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Tests.Utils;
using Xunit;

namespace Stashbox.Tests
{

    public class FactoryBuildUpTests
    {
        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void FactoryBuildUpTests_DependencyResolve(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest, Test>(context => context.WithFactory(() => new Test("test")));
            container.Register<ITest1, Test12>();

            var inst = container.Resolve<ITest1>();

            Assert.IsType<Test>(inst.Test);
            Assert.Equal("test", inst.Test.Name);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void FactoryBuildUpTests_DependencyResolve_ServiceUpdated(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest, Test>(context => context.WithFactory(() => new Test("test")));
            container.Register<ITest2, Test2>();
            container.ReMap<ITest, Test>(context => context.WithFactory(() => new Test("test1")));
            var inst = container.Resolve<ITest2>();

            Assert.IsType<Test>(inst.Test);
            Assert.Equal("test1", inst.Test.Name);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void FactoryBuildUpTests_Resolve(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest, Test>(context => context.WithFactory(() => new Test("test")));
            container.Register<ITest1, Test1>();

            var inst = container.Resolve<ITest1>();

            Assert.IsType<Test>(inst.Test);
            Assert.Equal("test", inst.Test.Name);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void FactoryBuildUpTests_Resolve_NotSame(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest, Test>(context =>
                context.WithInjectionParameter("name", "test"));
            container.Register<ITest1>(context => context.WithFactory(cont =>
            {
                var test1 = cont.Resolve<ITest>();
                return new Test12(test1);
            }));

            var inst1 = container.Resolve<ITest1>();
            var inst2 = container.Resolve<ITest1>();

            Assert.NotSame(inst1.Test, inst2.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void FactoryBuildUpTests_Resolve_ContainerFactory(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test3>();
            container.Register<ITest>(context => context.WithFactory(c => c.Resolve<Test3>()));

            var inst = container.Resolve<ITest>();

            Assert.IsType<Test3>(inst);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void FactoryBuildUpTests_Resolve_ContainerFactory_Constructor(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<Test3>();
            container.Register<ITest1, Test12>();
            container.Register(typeof(ITest), context => context.WithFactory(() => new Test3()));

            var test1 = container.Resolve<ITest1>();
            Assert.IsType<Test3>(test1.Test);
        }

        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void FactoryBuildUpTests_Resolve_ContainerFactory_Initializer(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithCompiler(compilerType));
            container.Register<ITest4>(context =>
                context.WithFactory(() => new Test4()).WithInitializer((t, r) => t.Init("Test")));

            var test1 = container.Resolve<ITest4>();
            Assert.Equal("Test", test1.Name);
        }

        interface ITest { string Name { get; } }

        interface ITest1 { ITest Test { get; } }

        interface ITest2 { ITest Test { get; } }

        class Test3 : ITest
        {
            public string Name { get; }
        }

        class Test : ITest
        {
            public string Name { get; }

            public Test(string name)
            {
                this.Name = name;
            }
        }

        class Test2 : ITest2
        {
            [Dependency]
            public ITest Test { get; set; }
        }

        class Test1 : ITest1
        {
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init(ITest test)
            {
                this.Test = test;
            }
        }

        class Test12 : ITest1
        {
            public ITest Test { get; private set; }

            public Test12(ITest test)
            {
                this.Test = test;
            }
        }

        interface ITest4 : ITest
        {
            void Init(string name);
        }

        class Test4 : ITest4
        {
            public string Name { get; private set; }

            public void Init(string name) => Name = name;
        }
    }
}
