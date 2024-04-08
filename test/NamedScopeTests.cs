using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests;

public class NamedScopeTests
{
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Simple_Resolve_Prefer_Named(CompilerType compilerType)
    {
        var name = "A".ToLower();
        var inst = new StashboxContainer(config => config.WithCompiler(compilerType))
            .Register<ITest, Test11>()
            .Register<ITest, Test>(config => config.InNamedScope(name))
            .Register<ITest, Test1>()
            .BeginScope(name)
            .Resolve<ITest>();

        Assert.IsType<Test>(inst);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Dependency_Resolve_Prefer_Named(CompilerType compilerType)
    {
        var inst = new StashboxContainer(config => config.WithCompiler(compilerType))
            .Register<Test2>()
            .Register<ITest, Test11>()
            .Register<ITest, Test>(config => config.InNamedScope("A"))
            .Register<ITest, Test1>()
            .BeginScope("A")
            .Resolve<Test2>();

        Assert.IsType<Test>(inst.Test);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Simple_Resolve_Wrapper_Prefer_Named(CompilerType compilerType)
    {
        using var scope = new StashboxContainer(config => config.WithCompiler(compilerType))
            .Register<ITest, Test11>(c => c.WithMetadata(new object()))
            .Register<ITest, Test>(config => config.InNamedScope("A").WithMetadata(new object()))
            .Register<ITest, Test1>(c => c.WithMetadata(new object()))
            .BeginScope("A");

        var func = scope.Resolve<Func<ITest>>();
        var lazy = scope.Resolve<Lazy<ITest>>();
        var tuple = scope.Resolve<Tuple<ITest, object>>();
        var enumerable = scope.Resolve<IEnumerable<ITest>>();
        var all = scope.ResolveAll<ITest>();

        Assert.IsType<Test>(func());
        Assert.IsType<Test>(lazy.Value);
        Assert.IsType<Test>(tuple.Item1);
        Assert.IsType<Test>(enumerable.Last());
        Assert.IsType<Test>(all.First());
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Dependency_Resolve_Wrapper_Prefer_Named(CompilerType compilerType)
    {
        var inst = new StashboxContainer(config => config.WithCompiler(compilerType))
            .Register<Test3>()
            .Register<ITest, Test11>(c => c.WithMetadata(new object()))
            .Register<ITest, Test>(config => config.InNamedScope("A").WithMetadata(new object()))
            .Register<ITest, Test1>(c => c.WithMetadata(new object()))
            .BeginScope("A")
            .Resolve<Test3>();

        Assert.IsType<Test>(inst.Func());
        Assert.IsType<Test>(inst.Lazy.Value);
        Assert.IsType<Test>(inst.Enumerable.Last());
        Assert.IsType<Test>(inst.Tuple.Item1);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Simple_Resolve_Prefer_Named_Last(CompilerType compilerType)
    {
        var inst = new StashboxContainer(config => config.WithCompiler(compilerType))
            .Register<ITest, Test11>(config => config.InNamedScope("A"))
            .Register<ITest, Test>()
            .Register<ITest, Test1>(config => config.InNamedScope("A"))
            .BeginScope("A")
            .Resolve<ITest>();

        Assert.IsType<Test1>(inst);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Simple_Resolve_Gets_Same_Within_Scope(CompilerType compilerType)
    {
        using var scope = new StashboxContainer(config => config.WithCompiler(compilerType))
            .Register<ITest, Test>()
            .Register<ITest, Test1>(config => config.InNamedScope("A"))
            .BeginScope("A");

        var a = scope.Resolve<ITest>();
        var b = scope.Resolve<ITest>();

        Assert.Same(a, b);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Simple_Resolve_Gets_Named_Within_Scope(CompilerType compilerType)
    {
        using var scope = new StashboxContainer(config => config.WithCompiler(compilerType))
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

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Simple_Resolve_Gets_Named_When_Scoped_Doesnt_Exist(CompilerType compilerType)
    {
        using var scope = new StashboxContainer(config => config.WithCompiler(compilerType))
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

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Dependency_Resolve_Wrapper_Gets_Same_Within_Scope(CompilerType compilerType)
    {
        var inst = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test3>()
            .Register<ITest, Test>(config => config.InNamedScope("A").WithMetadata(new object()))
            .Register<ITest, Test1>(c => c.WithMetadata(new object()))
            .BeginScope("A")
            .Resolve<Test3>();

        Assert.Same(inst.Func(), inst.Lazy.Value);
        Assert.Same(inst.Lazy.Value, inst.Enumerable.Last());
        Assert.Same(inst.Enumerable.Last(), inst.Tuple.Item1);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Simple_Resolve_Get_Last_If_Scoped_Doesnt_Exist(CompilerType compilerType)
    {
        var inst = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>()
            .Register<ITest, Test1>()
            .BeginScope("A")
            .Resolve<ITest>();

        Assert.IsType<Test1>(inst);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Dependency_Get_Last_If_Scoped_Doesnt_Exist(CompilerType compilerType)
    {
        var inst = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test3>()
            .Register<ITest, Test>(c => c.WithMetadata(new object()))
            .Register<ITest, Test1>(c => c.WithMetadata(new object()))
            .BeginScope("A")
            .Resolve<Test3>();

        Assert.IsType<Test1>(inst.Func());
        Assert.IsType<Test1>(inst.Lazy.Value);
        Assert.IsType<Test1>(inst.Enumerable.Last());
        Assert.IsType<Test1>(inst.Tuple.Item1);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Defines_Scope_Prefer_Named(CompilerType compilerType)
    {
        var inst = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test3>(config => config.DefinesScope("A"))
            .Register<ITest, Test11>(c => c.WithMetadata(new object()))
            .Register<ITest, Test>(config => config.InNamedScope("A").WithMetadata(new object()))
            .Register<ITest, Test1>(c => c.WithMetadata(new object()))
            .Resolve<Test3>();

        Assert.IsType<Test>(inst.Func());
        Assert.IsType<Test>(inst.Lazy.Value);
        Assert.IsType<Test>(inst.Enumerable.Last());
        Assert.IsType<Test>(inst.Tuple.Item1);

        Assert.Same(inst.Func(), inst.Lazy.Value);
        Assert.Same(inst.Lazy.Value, inst.Enumerable.Last());
        Assert.Same(inst.Enumerable.Last(), inst.Tuple.Item1);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Preserve_Instance_Through_Nested_Scopes(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>(config => config.InNamedScope("A"));

        using var s1 = container.BeginScope("A");
        var i1 = s1.Resolve<ITest>();
        using var s2 = s1.BeginScope("C");
        var i2 = s2.Resolve<ITest>();

        Assert.Same(i2, i1);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Dispose_Instance_Through_Nested_Scopes(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
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

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Dispose_Instance_Defines_Named_Scope(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test12>(config => config.InNamedScope("A"))
            .Register<Test2>(config => config.DefinesScope("A"));

        Test2 i1;
        using (var s1 = container.BeginScope())
        {
            i1 = s1.Resolve<Test2>();
        }

        Assert.True(((Test12)i1.Test).Disposed);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Lifetime_Check(CompilerType compilerType)
    {
        var inst = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>(config => config.InNamedScope("A"))
            .ContainerContext.RegistrationRepository.GetRegistrationMappings().First(reg => reg.Key == typeof(ITest));

        Assert.IsType<NamedScopeLifetime>(inst.Value.Lifetime);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Throws_ResolutionFailedException_Without_Scope(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>(config => config.InNamedScope("A"));

        Assert.Throws<ResolutionFailedException>(() => container.Resolve<ITest>());
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Throws_ResolutionFailedException_Wrong_Scope(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>(config => config.InNamedScope("A"));

        using var scope = container.BeginScope("B");
        Assert.Throws<ResolutionFailedException>(() => scope.Resolve<ITest>());
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Throws_ResolutionFailedException_Wrong_Scope_Null(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>(config => config.InNamedScope("A"));

        using var scope = container.BeginScope("B");
        Assert.Null(scope.ResolveOrDefault<ITest>());
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_WithNull(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test2>(config => config.InNamedScope("A"));

        Assert.Null(container.BeginScope("A").ResolveOrDefault<Test2>());
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_ChildContainer_Chain(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test2>(config => config.DefinesScope("B").InNamedScope("A"));

        var child = container.CreateChildContainer()
            .Register<ITest, Test1>(config => config.InNamedScope("B"))
            .Register<Test4>(config => config.DefinesScope("A"));

        var inst = child.Resolve<Test4>();

        Assert.NotNull(inst.Test);
        Assert.NotNull(inst.Test.Test);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_ChildContainer(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test2>(config => config.DefinesScope("A"))
            .Register<ITest, Test>(config => config.InNamedScope("A"));

        var child = container.CreateChildContainer()
            .Register<ITest, Test1>(config => config.InNamedScope("A"));

        var inst = child.Resolve<Test2>();

        Assert.IsType<Test1>(inst.Test);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Chain(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<Test2>(config => config.DefinesScope("B").InNamedScope("A"))
            .Register<ITest, Test1>(config => config.InNamedScope("B"))
            .Register<Test4>(config => config.DefinesScope("A"));

        var inst = container.Resolve<Test4>();

        Assert.NotNull(inst.Test);
        Assert.NotNull(inst.Test.Test);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_ChildContainer_Chain_Reverse(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register(typeof(Test4), config => config.DefinesScope("A"))
            .Register<ITest, Test1>(config => config.InNamedScope("B"));

        var child = container.CreateChildContainer()
            .Register<Test2>(config => config.DefinesScope("B").InNamedScope("A"));

        var inst = child.Resolve<Test4>();

        Assert.NotNull(inst.Test);
        Assert.NotNull(inst.Test.Test);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void NamedScope_Cache(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType))
            .Register<ITest, Test>(config => config.InNamedScope("A"))
            .Register<ITest, Test1>();

        using var scope = container.BeginScope();
        var inst = scope.Resolve<ITest>();

        Assert.IsType<Test1>(inst);

        using var scope1 = container.BeginScope("A");
        inst = scope1.Resolve<ITest>();

        Assert.IsType<Test>(inst);
    }

    interface ITest;

    class Test : ITest;

    class Test1 : ITest;

    class Test11 : ITest;

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
        public Test3(Func<ITest> func, Lazy<ITest> lazy, IEnumerable<ITest> enumerable, Tuple<ITest, object> tuple)
        {
            Func = func;
            Lazy = lazy;
            Enumerable = enumerable;
            Tuple = tuple;
        }

        public Func<ITest> Func { get; }
        public Lazy<ITest> Lazy { get; }
        public IEnumerable<ITest> Enumerable { get; }
        public Tuple<ITest, object> Tuple { get; }
    }
}