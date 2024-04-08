using Stashbox.Configuration;
using Stashbox.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stashbox.Tests;

public class MetadataTests
{
    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Tuple<ITest, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Tuple<ITest, object>>(inst);
        Assert.IsType<Test>(inst.Item1);
        Assert.Same(inst.Item2, meta);
    }

    [Fact]
    public void TupleTests_Resolve_Null()
    {
        var container = new StashboxContainer();
        var inst = container.ResolveOrDefault<Tuple<ITest, object>>();

        Assert.Null(inst);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Lazy(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Tuple<Lazy<ITest>, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Tuple<Lazy<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Item1.Value);
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Func(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Tuple<Func<ITest>, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Tuple<Func<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Item1());
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Enumerable(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<IEnumerable<Tuple<ITest, object>>>();

        Assert.NotNull(inst);
        Assert.IsAssignableFrom<IEnumerable<Tuple<ITest, object>>>(inst);
        Assert.IsType<Test>(inst.First().Item1);
        Assert.Same(inst.First().Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Enumerable_ShouldNull(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Tuple<IEnumerable<ITest>, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Tuple<IEnumerable<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Item1.First());
        Assert.Null(inst.Item2);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Constructor(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        container.Register<Test2>();
        var inst = container.Resolve<Test2>();

        Assert.NotNull(inst);
        Assert.IsType<Tuple<ITest, object>>(inst.Test);
        Assert.IsType<Test>(inst.Test.Item1);
        Assert.Same(inst.Test.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Custom_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        ITest1 meta = new Test1();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Tuple<ITest, ITest1>>();

        Assert.NotNull(inst);
        Assert.IsType<Tuple<ITest, ITest1>>(inst);
        Assert.IsType<Test>(inst.Item1);
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Custom_Meta_Implementation_Type(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        Test1 meta = new Test1();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Tuple<ITest, ITest1>>();

        Assert.NotNull(inst);
        Assert.IsType<Tuple<ITest, ITest1>>(inst);
        Assert.IsType<Test>(inst.Item1);
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Custom_Meta_Chooses_Best_Match(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>(c => c.WithMetadata(1));
        var inst1 = container.Resolve<Tuple<ITest, string>>();
        var inst2 = container.Resolve<Tuple<ITest, int>>();

        Assert.NotNull(inst1);
        Assert.NotNull(inst2);
        Assert.IsType<A>(inst1.Item1);
        Assert.Equal("A", inst1.Item2);
        Assert.IsType<B>(inst2.Item1);
        Assert.Equal(1, inst2.Item2);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Custom_Meta_Enumerable_Filter(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>(c => c.WithMetadata(1));
        var filtered1 = container.Resolve<IEnumerable<Tuple<ITest, string>>>();
        var filtered2 = container.Resolve<IEnumerable<Tuple<ITest, int>>>();

        Assert.Single(filtered1);
        Assert.IsType<A>(filtered1.First().Item1);
        Assert.Equal("A", filtered1.First().Item2);
        Assert.Single(filtered2);
        Assert.IsType<B>(filtered2.First().Item1);
        Assert.Equal(1, filtered2.First().Item2);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Custom_Meta_Enumerable_Choose_Only_With_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>();
        var filtered = container.Resolve<IEnumerable<Tuple<ITest, string>>>();
        var unfiltered = container.Resolve<IEnumerable<ITest>>().ToArray();

        Assert.Single(filtered);
        Assert.IsType<A>(filtered.First().Item1);
        Assert.Equal("A", filtered.First().Item2);
        Assert.Equal(2, unfiltered.Length);
        Assert.IsType<A>(unfiltered[0]);
        Assert.IsType<B>(unfiltered[1]);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void TupleTests_Resolve_Custom_Meta_Enumerable_Empty_When_Nothing_Registered_With_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>();
        container.Register<ITest, B>();
        var filtered = container.Resolve<IEnumerable<Tuple<ITest, object>>>();
        var unfiltered = container.Resolve<IEnumerable<ITest>>().ToArray();

        Assert.Empty(filtered);
        Assert.Equal(2, unfiltered.Length);
        Assert.IsType<A>(unfiltered[0]);
        Assert.IsType<B>(unfiltered[1]);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<ValueTuple<ITest, object>>();

        Assert.NotEqual(default(ValueTuple<ITest, object>), inst);
        Assert.IsType<ValueTuple<ITest, object>>(inst);
        Assert.IsType<Test>(inst.Item1);
        Assert.Same(inst.Item2, meta);
    }

    [Fact]
    public void ValueTupleTests_Resolve_Null()
    {
        var container = new StashboxContainer();
        var inst = container.ResolveOrDefault<ValueTuple<ITest, object>>();

        Assert.Equal(default(ValueTuple<ITest, object>), inst);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Lazy(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<ValueTuple<Lazy<ITest>, object>>();

        Assert.NotEqual(default, inst);
        Assert.IsType<ValueTuple<Lazy<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Item1.Value);
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Func(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<ValueTuple<Func<ITest>, object>>();

        Assert.NotEqual(default, inst);
        Assert.IsType<ValueTuple<Func<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Item1());
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Enumerable(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<IEnumerable<ValueTuple<ITest, object>>>();

        Assert.NotNull(inst);
        Assert.IsAssignableFrom<IEnumerable<ValueTuple<ITest, object>>>(inst);
        Assert.IsType<Test>(inst.First().Item1);
        Assert.Same(inst.First().Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Enumerable_ShouldNull(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<ValueTuple<IEnumerable<ITest>, object>>();

        Assert.NotEqual(default, inst);
        Assert.IsType<ValueTuple<IEnumerable<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Item1.First());
        Assert.Null(inst.Item2);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Constructor(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        container.Register<Test4>();
        var inst = container.Resolve<Test4>();

        Assert.NotNull(inst);
        Assert.IsType<ValueTuple<ITest, object>>(inst.Test);
        Assert.IsType<Test>(inst.Test.Item1);
        Assert.Same(inst.Test.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Custom_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        ITest1 meta = new Test1();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<ValueTuple<ITest, ITest1>>();

        Assert.NotEqual(default, inst);
        Assert.IsType<ValueTuple<ITest, ITest1>>(inst);
        Assert.IsType<Test>(inst.Item1);
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Custom_Meta_Implementation_Type(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        Test1 meta = new Test1();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<ValueTuple<ITest, ITest1>>();

        Assert.NotEqual(default, inst);
        Assert.IsType<ValueTuple<ITest, ITest1>>(inst);
        Assert.IsType<Test>(inst.Item1);
        Assert.Same(inst.Item2, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Custom_Meta_Chooses_Best_Match(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>(c => c.WithMetadata(1));
        var inst1 = container.Resolve<ValueTuple<ITest, string>>();
        var inst2 = container.Resolve<ValueTuple<ITest, int>>();

        Assert.NotEqual(default, inst1);
        Assert.NotEqual(default, inst2);
        Assert.IsType<A>(inst1.Item1);
        Assert.Equal("A", inst1.Item2);
        Assert.IsType<B>(inst2.Item1);
        Assert.Equal(1, inst2.Item2);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Custom_Meta_Enumerable_Filter(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>(c => c.WithMetadata(1));
        var filtered1 = container.Resolve<IEnumerable<ValueTuple<ITest, string>>>();
        var filtered2 = container.Resolve<IEnumerable<ValueTuple<ITest, int>>>();

        Assert.Single(filtered1);
        Assert.IsType<A>(filtered1.First().Item1);
        Assert.Equal("A", filtered1.First().Item2);
        Assert.Single(filtered2);
        Assert.IsType<B>(filtered2.First().Item1);
        Assert.Equal(1, filtered2.First().Item2);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Custom_Meta_Enumerable_Choose_Only_With_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>();
        var filtered = container.Resolve<IEnumerable<ValueTuple<ITest, string>>>();
        var unfiltered = container.Resolve<IEnumerable<ITest>>().ToArray();

        Assert.Single(filtered);
        Assert.IsType<A>(filtered.First().Item1);
        Assert.Equal("A", filtered.First().Item2);
        Assert.Equal(2, unfiltered.Length);
        Assert.IsType<A>(unfiltered[0]);
        Assert.IsType<B>(unfiltered[1]);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void ValueTupleTests_Resolve_Custom_Meta_Enumerable_Empty_When_Nothing_Registered_With_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>();
        container.Register<ITest, B>();
        var filtered = container.Resolve<IEnumerable<ValueTuple<ITest, object>>>();
        var unfiltered = container.Resolve<IEnumerable<ITest>>().ToArray();

        Assert.Empty(filtered);
        Assert.Equal(2, unfiltered.Length);
        Assert.IsType<A>(unfiltered[0]);
        Assert.IsType<B>(unfiltered[1]);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Metadata<ITest, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Metadata<ITest, object>>(inst);
        Assert.IsType<Test>(inst.Service);
        Assert.Same(inst.Data, meta);
    }

    [Fact]
    public void MetadataTests_Resolve_Null()
    {
        var container = new StashboxContainer();
        var inst = container.ResolveOrDefault<Metadata<ITest, object>>();

        Assert.Null(inst);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Lazy(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Metadata<Lazy<ITest>, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Metadata<Lazy<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Service.Value);
        Assert.Same(inst.Data, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Func(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Metadata<Func<ITest>, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Metadata<Func<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Service());
        Assert.Same(inst.Data, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Enumerable(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<IEnumerable<Metadata<ITest, object>>>();

        Assert.NotNull(inst);
        Assert.IsAssignableFrom<IEnumerable<Metadata<ITest, object>>>(inst);
        Assert.IsType<Test>(inst.First().Service);
        Assert.Same(inst.First().Data, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Enumerable_ShouldNull(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Metadata<IEnumerable<ITest>, object>>();

        Assert.NotNull(inst);
        Assert.IsType<Metadata<IEnumerable<ITest>, object>>(inst);
        Assert.IsType<Test>(inst.Service.First());
        Assert.Null(inst.Data);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Constructor(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        var meta = new object();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        container.Register<Test3>();
        var inst = container.Resolve<Test3>();

        Assert.NotNull(inst);
        Assert.IsType<Metadata<ITest, object>>(inst.Test);
        Assert.IsType<Test>(inst.Test.Service);
        Assert.Same(inst.Test.Data, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Custom_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        ITest1 meta = new Test1();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Metadata<ITest, ITest1>>();

        Assert.NotNull(inst);
        Assert.IsType<Metadata<ITest, ITest1>>(inst);
        Assert.IsType<Test>(inst.Service);
        Assert.Same(inst.Data, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Custom_Meta_ImplementationType(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        Test1 meta = new Test1();
        container.Register<ITest, Test>(c => c.WithMetadata(meta));
        var inst = container.Resolve<Metadata<ITest, ITest1>>();

        Assert.NotNull(inst);
        Assert.IsType<Metadata<ITest, ITest1>>(inst);
        Assert.IsType<Test>(inst.Service);
        Assert.Same(inst.Data, meta);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Custom_Meta_Chooses_Best_Match(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>(c => c.WithMetadata(1));
        var inst1 = container.Resolve<Metadata<ITest, string>>();
        var inst2 = container.Resolve<Metadata<ITest, int>>();

        Assert.NotNull(inst1);
        Assert.NotNull(inst2);
        Assert.IsType<A>(inst1.Service);
        Assert.Equal("A", inst1.Data);
        Assert.IsType<B>(inst2.Service);
        Assert.Equal(1, inst2.Data);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Custom_Meta_Enumerable_Filter(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>(c => c.WithMetadata(1));
        var filtered1 = container.Resolve<IEnumerable<Metadata<ITest, string>>>();
        var filtered2 = container.Resolve<IEnumerable<Metadata<ITest, int>>>();

        Assert.Single(filtered1);
        Assert.IsType<A>(filtered1.First().Service);
        Assert.Equal("A", filtered1.First().Data);
        Assert.Single(filtered2);
        Assert.IsType<B>(filtered2.First().Service);
        Assert.Equal(1, filtered2.First().Data);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Custom_Meta_Enumerable_Choose_Only_With_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>(c => c.WithMetadata("A"));
        container.Register<ITest, B>();
        var filtered = container.Resolve<IEnumerable<Metadata<ITest, string>>>();
        var unfiltered = container.Resolve<IEnumerable<ITest>>().ToArray();

        Assert.Single(filtered);
        Assert.IsType<A>(filtered.First().Service);
        Assert.Equal("A", filtered.First().Data);
        Assert.Equal(2, unfiltered.Length);
        Assert.IsType<A>(unfiltered[0]);
        Assert.IsType<B>(unfiltered[1]);
    }

    [Theory]
    [ClassData(typeof(CompilerTypeTestData))]
    public void MetadataTests_Resolve_Custom_Meta_Enumerable_Empty_When_Nothing_Registered_With_Meta(CompilerType compilerType)
    {
        var container = new StashboxContainer(c => c.WithCompiler(compilerType));
        container.Register<ITest, A>();
        container.Register<ITest, B>();
        var filtered = container.Resolve<IEnumerable<Metadata<ITest, object>>>();
        var unfiltered = container.Resolve<IEnumerable<ITest>>().ToArray();

        Assert.Empty(filtered);
        Assert.Equal(2, unfiltered.Length);
        Assert.IsType<A>(unfiltered[0]);
        Assert.IsType<B>(unfiltered[1]);
    }

    interface ITest;

    interface ITest1;

    class Test : ITest;

    class A : ITest;

    class B : ITest;

    class Test1 : ITest1;

    class Test2
    {
        public Tuple<ITest, object> Test { get; }

        public Test2(Tuple<ITest, object> test)
        {
            this.Test = test;
        }
    }

    class Test3
    {
        public Metadata<ITest, object> Test { get; }

        public Test3(Metadata<ITest, object> test)
        {
            this.Test = test;
        }
    }

    class Test4
    {
        public ValueTuple<ITest, object> Test { get; }

        public Test4(ValueTuple<ITest, object> test)
        {
            this.Test = test;
        }
    }
}