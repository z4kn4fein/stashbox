using Xunit;

namespace Stashbox.Tests
{
    
    public class InstanceBuilderTests
    {
        [Fact]
        public void InstanceBuilderTests_Resolve()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                container.RegisterInstanceAs<ITest>(dep);
                var inst = container.Resolve<ITest>();

                Assert.Same(inst, dep);
            }
        }

        [Fact]
        public void InstanceBuilderTests_DependencyResolve()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                container.RegisterInstanceAs<ITest>(dep);
                container.Register<ITest1, Test1>();

                var inst = container.Resolve<ITest1>();

                Assert.Same(inst.Test, dep);
            }
        }

        [Fact]
        public void InstanceBuilderTests_Resolve_Fluent()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                container.Register<ITest>(context => context.WithInstance(dep));
                var inst = container.Resolve<ITest>();

                Assert.Same(inst, dep);
            }
        }

        [Fact]
        public void InstanceBuilderTests_Resolve_Fluent_ReMap()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                var dep1 = new Test();
                container.Register<ITest>(context => context.WithInstance(dep));
                container.ReMap<ITest>(context => context.WithInstance(dep1));
                var inst = container.Resolve<ITest>();

                Assert.Same(inst, dep1);
            }
        }

        [Fact]
        public void InstanceBuilderTests_Resolve_Fluent_ReMap_Self()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                var dep1 = new Test();
                container.Register(dep.GetType(), context => context.WithInstance(dep));
                container.ReMap(dep1.GetType(), context => context.WithInstance(dep1));
                var inst = container.Resolve<Test>();

                Assert.Same(inst, dep1);
            }
        }

        [Fact]
        public void InstanceBuilderTests_DependencyResolve_Fluent()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                container.Register<ITest>(context => context.WithInstance(dep));
                container.Register<ITest1, Test1>();

                var inst = container.Resolve<ITest1>();

                Assert.Same(inst.Test, dep);
            }
        }

        [Fact]
        public void InstanceBuilderTests_Multiple()
        {
            using (var container = new StashboxContainer())
            {
                var dep = new Test();
                var dep2 = new Test1(new Test());
                container.RegisterInstance(dep);
                container.RegisterInstance(dep2);
                container.Resolve<Test>();
                var inst = container.Resolve<Test1>();

                Assert.Same(inst, dep2);
            }
        }

        public interface ITest { }

        public interface ITest1 { ITest Test { get; } }

        public class Test : ITest { }

        public class Test1 : ITest1
        {
            public ITest Test { get; }
            public Test1(ITest test)
            {
                this.Test = test;
            }
        }
    }
}
