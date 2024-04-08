using Stashbox.Configuration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests;

public class EnumerableTests
{
    [Fact]
    public void EnumerableTests_Resolve_Array_PreserveOrder()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var all = container.Resolve<ITest1[]>();

        Assert.Equal(3, all.Length);
    }

    [Fact]
    public void EnumerableTests_Resolve_IList()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var all = container.Resolve<IList<ITest1>>();

        Assert.Equal(3, all.Count);
    }

    [Fact]
    public void EnumerableTests_Resolve_ICollection()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var all = container.Resolve<ICollection<ITest1>>();

        Assert.Equal(3, all.Count);
    }

    [Fact]
    public void EnumerableTests_Resolve_IReadonlyCollection()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var all = container.Resolve<IReadOnlyCollection<ITest1>>();

        Assert.Equal(3, all.Count);
    }

    [Fact]
    public void EnumerableTests_Resolve_IReadOnlyList()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var all = container.Resolve<IReadOnlyList<ITest1>>();

        Assert.Equal(3, all.Count);
    }

    [Fact]
    public void EnumerableTests_Resolve()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>(context => context.WithName("enumerable"));
        container.Register<ITest2, Test22>(context => context.WithName("array"));

        container.Resolve<ITest2>("enumerable");
        container.Resolve<ITest2>("array");

        var all = container.Resolve<IEnumerable<ITest2>>();
        var all2 = container.ResolveAll<ITest2>();

        Assert.Equal(2, all.Count());
        Assert.Equal(2, all2.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var all = container.Resolve<IEnumerable<ITest2>>();
        var all2 = container.ResolveAll<ITest2>();

        Assert.Empty(all);
        Assert.Empty(all2);
    }

    [Fact]
    public void EnumerableTests_Resolve_Scoped_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var scope = container.BeginScope();

        var all = scope.Resolve<IEnumerable<ITest2>>();
        var all2 = scope.ResolveAll<ITest2>();

        Assert.Empty(all);
        Assert.Empty(all2);
    }

    [Fact]
    public void EnumerableTests_Resolve_Scoped()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.BeginScope();

        var all = child.Resolve<IEnumerable<ITest1>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Parent()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.CreateChildContainer();

        var all = child.Resolve<IEnumerable<ITest1>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Parent_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var child = container.CreateChildContainer();

        var all = child.Resolve<IEnumerable<ITest1>>();

        Assert.Empty(all);
    }

    [Fact]
    public void EnumerableTests_Resolve_Scoped_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.BeginScope();

        var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Parent_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.CreateChildContainer();

        var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Scoped_Lazy_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var child = container.BeginScope();

        var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

        Assert.Empty(all);
    }

    [Fact]
    public void EnumerableTests_Resolve_Parent_Lazy_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var child = container.CreateChildContainer();

        var all = child.Resolve<IEnumerable<Lazy<ITest1>>>();

        Assert.Empty(all);
    }

    [Fact]
    public void EnumerableTests_Resolve_Scoped_Func()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.BeginScope();

        var all = child.Resolve<IEnumerable<Func<ITest1>>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Parent_Func()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.CreateChildContainer();

        var all = child.Resolve<IEnumerable<Func<ITest1>>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Scoped_Func_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var child = container.BeginScope();

        var all = child.Resolve<IEnumerable<Func<ITest1>>>();

        Assert.Empty(all);
    }

    [Fact]
    public void EnumerableTests_Resolve_Parent_Func_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var child = container.CreateChildContainer();

        var all = child.Resolve<IEnumerable<Func<ITest1>>>();

        Assert.Empty(all);
    }

    [Fact]
    public void EnumerableTests_Resolve_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var all = container.Resolve<IEnumerable<Lazy<ITest1>>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Lazy_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var all = container.Resolve<IEnumerable<Lazy<ITest1>>>();

        Assert.Empty(all);
    }

    [Fact]
    public void EnumerableTests_Resolve_Func()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var all = container.Resolve<IEnumerable<Func<ITest1>>>();

        Assert.Equal(3, all.Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_Func_Null()
    {
        IStashboxContainer container = new StashboxContainer();

        var all = container.Resolve<IEnumerable<Func<ITest1>>>();

        Assert.Empty(all);
    }

    [Fact]
    public void EnumerableTests_ResolveNonGeneric()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>(context => context.WithName("enumerable"));
        container.Register<ITest2, Test22>(context => context.WithName("array"));

        container.Resolve<ITest2>("enumerable");
        container.Resolve<ITest2>("array");

        var all = container.Resolve<IEnumerable<ITest2>>();
        var all2 = (IEnumerable<ITest2>)container.ResolveAll(typeof(ITest2));
        var all3 = container.ResolveAll(typeof(ITest2));

        Assert.Equal(2, all.Count());
        Assert.Equal(2, all2.Count());
        Assert.Equal(2, all3.Count());
    }

    [Fact]
    public void EnumerableTests_ResolveNonGeneric_Scoped()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>(context => context.WithName("enumerable"));
        container.Register<ITest2, Test22>(context => context.WithName("array"));

        var scope = container.BeginScope();

        scope.Resolve<ITest2>("enumerable");
        scope.Resolve<ITest2>("array");

        var all = scope.Resolve<IEnumerable<ITest2>>();
        var all2 = (IEnumerable<ITest2>)scope.ResolveAll(typeof(ITest2));
        var all3 = scope.ResolveAll(typeof(ITest2));

        Assert.Equal(2, all.Count());
        Assert.Equal(2, all2.Count());
        Assert.Equal(2, all3.Count());
    }

    [Fact]
    public void EnumerableTests_Parallel_Resolve()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>(context => context.WithName("enumerable"));
        container.Register<ITest2, Test22>(context => context.WithName("array"));

        Parallel.For(0, 10000, (i) =>
        {
            container.Resolve<ITest2>("enumerable");
            container.Resolve<ITest2>("array");
            var all = container.Resolve<IEnumerable<ITest2>>();

            Assert.Equal(2, all.Count());
        });
    }

    [Fact]
    public void EnumerableTests_Parallel_Resolve_NonGeneric()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>(context => context.WithName("enumerable"));
        container.Register<ITest2, Test22>(context => context.WithName("array"));

        Parallel.For(0, 10000, (i) =>
        {
            container.Resolve<ITest2>("enumerable");
            container.Resolve<ITest2>("array");

            var all = container.Resolve<IEnumerable<ITest2>>();
            var all2 = (IEnumerable<ITest2>)container.ResolveAll(typeof(ITest2));

            Assert.Equal(2, all2.Count());
            Assert.Equal(2, all.Count());
        });
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var services = container.Resolve<IEnumerable<ITest1>>().ToArray();

        Assert.IsType<Test1>(services[0]);
        Assert.IsType<Test11>(services[1]);
        Assert.IsType<Test12>(services[2]);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_PreserveOrder()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var services = container.ResolveAll<ITest1>().ToArray();

        Assert.IsType<Test1>(services[0]);
        Assert.IsType<Test11>(services[1]);
        Assert.IsType<Test12>(services[2]);
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Scoped()
    {
        IStashboxContainer container = new StashboxContainer();

        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.BeginScope();

        var services = child.Resolve<IEnumerable<ITest1>>().ToArray();

        Assert.IsType<Test1>(services[0]);
        Assert.IsType<Test11>(services[1]);
        Assert.IsType<Test12>(services[2]);
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Parent()
    {
        IStashboxContainer container = new StashboxContainer();

        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.CreateChildContainer();

        var services = child.Resolve<IEnumerable<ITest1>>().ToArray();

        Assert.IsType<Test1>(services[0]);
        Assert.IsType<Test11>(services[1]);
        Assert.IsType<Test12>(services[2]);
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Scoped_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();

        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.BeginScope();

        var services = child.Resolve<IEnumerable<Lazy<ITest1>>>().ToArray();

        Assert.IsType<Test1>(services[0].Value);
        Assert.IsType<Test11>(services[1].Value);
        Assert.IsType<Test12>(services[2].Value);
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Parent_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();

        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.CreateChildContainer();

        var services = child.Resolve<IEnumerable<Lazy<ITest1>>>().ToArray();

        Assert.IsType<Test1>(services[0].Value);
        Assert.IsType<Test11>(services[1].Value);
        Assert.IsType<Test12>(services[2].Value);
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Scoped_Func()
    {
        IStashboxContainer container = new StashboxContainer();

        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.BeginScope();

        var services = child.Resolve<IEnumerable<Func<ITest1>>>().ToArray();

        Assert.IsType<Test1>(services[0]());
        Assert.IsType<Test11>(services[1]());
        Assert.IsType<Test12>(services[2]());
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Parent_Func()
    {
        IStashboxContainer container = new StashboxContainer();

        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var child = container.CreateChildContainer();

        var services = child.Resolve<IEnumerable<Func<ITest1>>>().ToArray();

        Assert.IsType<Test1>(services[0]());
        Assert.IsType<Test11>(services[1]());
        Assert.IsType<Test12>(services[2]());
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var services = container.Resolve<IEnumerable<Lazy<ITest1>>>().ToArray();

        Assert.IsType<Test1>(services[0].Value);
        Assert.IsType<Test11>(services[1].Value);
        Assert.IsType<Test12>(services[2].Value);
    }

    [Fact]
    public void EnumerableTests_Resolve_PreserveOrder_Func()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();

        var services = container.Resolve<IEnumerable<Func<ITest1>>>().ToArray();

        Assert.IsType<Test1>(services[0]());
        Assert.IsType<Test11>(services[1]());
        Assert.IsType<Test12>(services[2]());
    }

    [Fact]
    public void EnumerableTests_Resolve_UniqueIds()
    {
        IStashboxContainer container = new StashboxContainer(config => config
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test1>();

        Assert.Equal(3, container.Resolve<IEnumerable<ITest1>>().Count());
    }

    [Fact]
    public void EnumerableTests_Resolve_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.Resolve<IEnumerable<ITest1>>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_Resolve_WithName_Single()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t1");
        container.Register<ITest1, Test11>("t2");
        container.Register<ITest1, Test12>();

        Assert.Single(container.Resolve<IEnumerable<ITest1>>("t1"));
    }

    [Fact]
    public void EnumerableTests_Resolve_WithName_FromCache()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.Resolve<IEnumerable<ITest1>>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);

        var instances2 = container.Resolve<IEnumerable<ITest1>>("t").ToArray();

        Assert.Equal(2, instances2.Length);
        Assert.IsType<Test1>(instances2[0]);
        Assert.IsType<Test11>(instances2[1]);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_Generic_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.ResolveAll<ITest1>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_Generic_WithName_FromCache()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.ResolveAll<ITest1>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);

        var instances2 = container.ResolveAll<ITest1>("t").ToArray();

        Assert.Equal(2, instances2.Length);
        Assert.IsType<Test1>(instances2[0]);
        Assert.IsType<Test11>(instances2[1]);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_WithName_FromCache()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);

        var instances2 = container.ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances2.Length);
        Assert.IsType<Test1>(instances2[0]);
        Assert.IsType<Test11>(instances2[1]);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_PerRequest_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>(c => c.WithName("t").WithPerRequestLifetime());
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_Scope_Resolve_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.BeginScope().Resolve<IEnumerable<ITest1>>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_Scope_Resolve_WithName_FromCache()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.BeginScope().Resolve<IEnumerable<ITest1>>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]); 
            
        var instances2 = container.BeginScope().Resolve<IEnumerable<ITest1>>("t").ToArray();

        Assert.Equal(2, instances2.Length);
        Assert.IsType<Test1>(instances2[0]);
        Assert.IsType<Test11>(instances2[1]);
    }

    [Fact]
    public void EnumerableTests_Scope_ResolveAll_Generic_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.BeginScope().ResolveAll<ITest1>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_Scope_ResolveAll_Generic_WithName_FromCache()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.BeginScope().ResolveAll<ITest1>("t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);

        var instances2 = container.BeginScope().ResolveAll<ITest1>("t").ToArray();

        Assert.Equal(2, instances2.Length);
        Assert.IsType<Test1>(instances2[0]);
        Assert.IsType<Test11>(instances2[1]);
    }

    [Fact]
    public void EnumerableTests_Scope_ResolveAll_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.BeginScope().ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_Scope_ResolveAll_WithName_FromCache()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>("t");
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.BeginScope().ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);

        var instances2 = container.BeginScope().ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances2.Length);
        Assert.IsType<Test1>(instances2[0]);
        Assert.IsType<Test11>(instances2[1]);
    }

    [Fact]
    public void EnumerableTests_Scope_ResolveAll_PerRequest_WithName()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>(c => c.WithName("t").WithPerRequestLifetime());
        container.Register<ITest1, Test11>("t");
        container.Register<ITest1, Test12>();

        var instances = container.BeginScope().ResolveAll(typeof(ITest1), "t").ToArray();

        Assert.Equal(2, instances.Length);
        Assert.IsType<Test1>(instances[0]);
        Assert.IsType<Test11>(instances[1]);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_WithName_WithOverrides()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest3, Test31>("t");
        container.Register<ITest3, Test32>("t");
        container.Register<ITest3, Test33>();

        var inst = container.ResolveAll(typeof(ITest3), "t", [new Test1()]).OfType<ITest3>().ToArray();
        Assert.Equal(2, inst.Length);
        Assert.IsType<Test1>(inst[0].Test);
        Assert.IsType<Test1>(inst[1].Test);
    }

    [Fact]
    public void EnumerableTests_ResolveAll_Generic_WithName_WithOverrides()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest3, Test31>("t");
        container.Register<ITest3, Test32>("t");
        container.Register<ITest3, Test33>();

        var inst = container.ResolveAll<ITest3>("t", [new Test1()]).ToArray();
        Assert.Equal(2, inst.Length);
        Assert.IsType<Test1>(inst[0].Test);
        Assert.IsType<Test1>(inst[1].Test);
    }

    interface ITest1;

    interface ITest2;

    interface ITest3 { ITest1 Test { get; } }

    class Test1 : ITest1;

    class Test11 : ITest1;

    class Test12 : ITest1;

    class Test2 : ITest2
    {
        public Test2(IEnumerable<ITest1> tests)
        {
            Shield.EnsureNotNull(tests, nameof(tests));
            Assert.Equal(3, tests.Count());
        }
    }

    class Test22 : ITest2
    {
        public Test22(ITest1[] tests)
        {
            Shield.EnsureNotNull(tests, nameof(tests));
            Assert.Equal(3, tests.Length);
        }
    }

    class Test31 : ITest3
    {
        public ITest1 Test { get; }

        public Test31(ITest1 test)
        {
            this.Test = test;
        }
    }

    class Test32 : ITest3
    {
        public ITest1 Test { get; }

        public Test32(ITest1 test)
        {
            this.Test = test;
        }
    }

    class Test33 : ITest3
    {
        public ITest1 Test { get; }

        public Test33(ITest1 test)
        {
            this.Test = test;
        }
    }
}