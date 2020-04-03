using Stashbox.Exceptions;
using Stashbox.Lifetime;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests
{

    public class NamedScopeTests
    {
        [Fact]
        public void NamedScope_Simple_Resolve_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .Register<ITest, Test11>()
                .Register<ITest, Test>(config => config.InNamedScope("A"))
                .Register<ITest, Test1>()
                .BeginScope("A")
                .Resolve<ITest>();

            Assert.IsType<Test>(inst);
        }

        [Fact]
        public void NamedScope_Dependency_Resolve_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .Register<Test2>()
                .Register<ITest, Test11>()
                .Register<ITest, Test>(config => config.InNamedScope("A"))
                .Register<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test2>();

            Assert.IsType<Test>(inst.Test);
        }

        [Fact]
        public void NamedScope_Simple_Resolve_Wrapper_Prefer_Named()
        {
            var scope = new StashboxContainer()
                .Register<ITest, Test11>()
                .Register<ITest, Test>(config => config.InNamedScope("A"))
                .Register<ITest, Test1>()
                .BeginScope("A");

            var func = scope.Resolve<Func<ITest>>();
            var lazy = scope.Resolve<Lazy<ITest>>();
            var tuple = scope.Resolve<Tuple<ITest>>();
            var enumerable = scope.Resolve<IEnumerable<ITest>>();
            var all = scope.ResolveAll<ITest>();

            Assert.IsType<Test>(func());
            Assert.IsType<Test>(lazy.Value);
            Assert.IsType<Test>(tuple.Item1);
            Assert.IsType<Test>(enumerable.Last());
            Assert.IsType<Test>(all.First());
        }

        [Fact]
        public void NamedScope_Dependency_Resolve_Wrapper_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .Register<Test3>()
                .Register<ITest, Test11>()
                .Register<ITest, Test>(config => config.InNamedScope("A"))
                .Register<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test3>();

            Assert.IsType<Test>(inst.Func());
            Assert.IsType<Test>(inst.Lazy.Value);
            Assert.IsType<Test>(inst.Enumerable.Last());
            Assert.IsType<Test>(inst.Tuple.Item1);
        }

        [Fact]
        public void NamedScope_Simple_Resolve_Prefer_Named_Last()
        {
            var inst = new StashboxContainer()
                .Register<ITest, Test11>(config => config.InNamedScope("A"))
                .Register<ITest, Test>()
                .Register<ITest, Test1>(config => config.InNamedScope("A"))
                .BeginScope("A")
                .Resolve<ITest>();

            Assert.IsType<Test1>(inst);
        }

        [Fact]
        public void NamedScope_Simple_Resolve_Gets_Same_Within_Scope()
        {
            var scope = new StashboxContainer()
                .Register<ITest, Test>()
                .Register<ITest, Test1>(config => config.InNamedScope("A"))
                .BeginScope("A");

            var a = scope.Resolve<ITest>();
            var b = scope.Resolve<ITest>();

            Assert.Same(a, b);
        }

        [Fact]
        public void NamedScope_Simple_Resolve_Gets_Named_Within_Scope()
        {
            var scope = new StashboxContainer()
                .Register<ITest, Test11>(config => config.InNamedScope("A"))
                .Register<ITest, Test>(config => config.InNamedScope("A").WithName("T"))
                .Register<ITest, Test1>(config => config.InNamedScope("A"))
                .BeginScope("A");

            var a = scope.Resolve<ITest>("T");
            var b = scope.Resolve<ITest>("T");
            var c = scope.Resolve<ITest>();

            Assert.Same(a, b);
            Assert.NotSame(a, c);
            Assert.IsType<Test>(a);
        }

        [Fact]
        public void NamedScope_Simple_Resolve_Gets_Named_When_Scoped_Doesnt_Exist()
        {
            var scope = new StashboxContainer()
                .Register<ITest, Test11>()
                .Register<ITest, Test>(config => config.WithName("T"))
                .Register<ITest, Test1>()
                .BeginScope("A");

            var a = scope.Resolve<ITest>("T");
            var b = scope.Resolve<ITest>("T");
            var c = scope.Resolve<ITest>();

            Assert.IsType<Test1>(c);
            Assert.IsType<Test>(b);
            Assert.IsType<Test>(a);
        }

        [Fact]
        public void NamedScope_Dependency_Resolve_Wrapper_Gets_Same_Within_Scope()
        {
            var inst = new StashboxContainer()
                .Register<Test3>()
                .Register<ITest, Test>(config => config.InNamedScope("A"))
                .Register<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test3>();

            Assert.Same(inst.Func(), inst.Lazy.Value);
            Assert.Same(inst.Lazy.Value, inst.Enumerable.Last());
            Assert.Same(inst.Enumerable.Last(), inst.Tuple.Item1);
        }

        [Fact]
        public void NamedScope_Simple_Resolve_Get_Last_If_Scoped_Doesnt_Exist()
        {
            var inst = new StashboxContainer()
                .Register<ITest, Test>()
                .Register<ITest, Test1>()
                .BeginScope("A")
                .Resolve<ITest>();

            Assert.IsType<Test1>(inst);
        }

        [Fact]
        public void NamedScope_Dependency_Get_Last_If_Scoped_Doesnt_Exist()
        {
            var inst = new StashboxContainer()
                .Register<Test3>()
                .Register<ITest, Test>()
                .Register<ITest, Test1>()
                .BeginScope("A")
                .Resolve<Test3>();

            Assert.IsType<Test1>(inst.Func());
            Assert.IsType<Test1>(inst.Lazy.Value);
            Assert.IsType<Test1>(inst.Enumerable.Last());
            Assert.IsType<Test1>(inst.Tuple.Item1);
        }

        [Fact]
        public void NamedScope_Defines_Scope_Prefer_Named()
        {
            var inst = new StashboxContainer()
                .Register<Test3>(config => config.DefinesScope("A"))
                .Register<ITest, Test11>()
                .Register<ITest, Test>(config => config.InNamedScope("A"))
                .Register<ITest, Test1>()
                .Resolve<Test3>();

            Assert.IsType<Test>(inst.Func());
            Assert.IsType<Test>(inst.Lazy.Value);
            Assert.IsType<Test>(inst.Enumerable.Last());
            Assert.IsType<Test>(inst.Tuple.Item1);

            Assert.Same(inst.Func(), inst.Lazy.Value);
            Assert.Same(inst.Lazy.Value, inst.Enumerable.Last());
            Assert.Same(inst.Enumerable.Last(), inst.Tuple.Item1);
        }

        [Fact]
        public void NamedScope_Preserve_Instance_Through_Nested_Scopes()
        {
            var container = new StashboxContainer()
                .Register<ITest, Test>(config => config.InNamedScope("A"));

            using var s1 = container.BeginScope("A");
            var i1 = s1.Resolve<ITest>();
            using var s2 = s1.BeginScope("C");
            var i2 = s2.Resolve<ITest>();

            Assert.Same(i2, i1);
        }

        [Fact]
        public void NamedScope_Dispose_Instance_Through_Nested_Scopes()
        {
            var container = new StashboxContainer()
                .Register<ITest, Test12>(config => config.InNamedScope("A"));

            ITest i1;
            using (var s1 = container.BeginScope("A"))
            {
                i1 = s1.Resolve<ITest>();
                using (var s2 = s1.BeginScope())
                {
                    var i2 = s2.Resolve<ITest>();

                    Assert.Same(i2, i1);
                }

                Assert.False(((Test12)i1).Disposed);
            }

            Assert.True(((Test12)i1).Disposed);
        }

        [Fact]
        public void NamedScope_Dispose_Instance_Defines_Named_Scope()
        {
            var container = new StashboxContainer()
                .Register<ITest, Test12>(config => config.InNamedScope("A"))
                .Register<Test2>(config => config.DefinesScope("A"));

            Test2 i1;
            using (var s1 = container.BeginScope())
            {
                i1 = s1.Resolve<Test2>();
            }

            Assert.True(((Test12)i1.Test).Disposed);
        }

        [Fact]
        public void NamedScope_Lifetime_Check()
        {
            var inst = new StashboxContainer()
                .Register<ITest, Test>(config => config.InNamedScope("A"))
                .ContainerContext.RegistrationRepository.GetRegistrationMappings().First(reg => reg.Key == typeof(ITest));

            Assert.IsType<NamedScopeLifetime>(inst.Value.RegistrationContext.Lifetime);
        }

        [Fact]
        public void NamedScope_Throws_ResolutionFailedException_Without_Scope()
        {
            var container = new StashboxContainer()
                .Register<ITest, Test>(config => config.InNamedScope("A"));

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest>());
        }

        [Fact]
        public void NamedScope_WithNull()
        {
            var container = new StashboxContainer()
                .Register<Test2>(config => config.InNamedScope("A"));

            Assert.Null(container.BeginScope("A").Resolve<Test2>(nullResultAllowed: true));
        }

        [Fact]
        public void NamedScope_ChildContainer_Chain()
        {
            var container = new StashboxContainer()
                .Register<Test2>(config => config.DefinesScope("B").InNamedScope("A"));

            var child = container.CreateChildContainer()
                .Register<ITest, Test1>(config => config.InNamedScope("B"))
                .Register<Test4>(config => config.DefinesScope("A"));

            var inst = child.Resolve<Test4>();

            Assert.NotNull(inst.Test);
            Assert.NotNull(inst.Test.Test);
        }

        [Fact]
        public void NamedScope_ChildContainer()
        {
            var container = new StashboxContainer()
                .Register<Test2>(config => config.DefinesScope("A"))
                .Register<ITest, Test>(config => config.InNamedScope("A"));

            var child = container.CreateChildContainer()
                .Register<ITest, Test1>(config => config.InNamedScope("A"));

            var inst = child.Resolve<Test2>();

            Assert.IsType<Test1>(inst.Test);
        }

        [Fact]
        public void NamedScope_Chain()
        {
            var container = new StashboxContainer()
                .Register<Test2>(config => config.DefinesScope("B").InNamedScope("A"))
                .Register<ITest, Test1>(config => config.InNamedScope("B"))
                .Register<Test4>(config => config.DefinesScope("A"));

            var inst = container.Resolve<Test4>();

            Assert.NotNull(inst.Test);
            Assert.NotNull(inst.Test.Test);
        }

        [Fact]
        public void NamedScope_ChildContainer_Chain_Reverse()
        {
            var container = new StashboxContainer()
                .Register<Test4>(config => config.DefinesScope("A"))
                .Register<ITest, Test1>(config => config.InNamedScope("B"));

            var child = container.CreateChildContainer()
                .Register<Test2>(config => config.DefinesScope("B").InNamedScope("A"));

            var inst = child.Resolve<Test4>();

            Assert.NotNull(inst.Test);
            Assert.NotNull(inst.Test.Test);
        }

        interface ITest
        { }

        class Test : ITest
        { }

        class Test1 : ITest
        { }

        class Test11 : ITest
        { }

        class Test12 : ITest, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException("");

                this.Disposed = true;
            }
        }

        class Test2
        {
            public Test2(ITest test)
            {
                Test = test;
            }

            public ITest Test { get; }
        }

        class Test4
        {
            public Test4(Test2 test)
            {
                Test = test;
            }

            public Test2 Test { get; }
        }

        class Test3
        {
            public Test3(Func<ITest> func, Lazy<ITest> lazy, IEnumerable<ITest> enumerable, Tuple<ITest> tuple)
            {
                Func = func;
                Lazy = lazy;
                Enumerable = enumerable;
                Tuple = tuple;
            }

            public Func<ITest> Func { get; }
            public Lazy<ITest> Lazy { get; }
            public IEnumerable<ITest> Enumerable { get; }
            public Tuple<ITest> Tuple { get; }
        }
    }
}
