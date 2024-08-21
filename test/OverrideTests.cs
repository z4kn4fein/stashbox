using Stashbox.Attributes;
using Stashbox.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests;

public class OverrideTests
{
    [Fact]
    public void OverrideTests_Resolve()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>(context =>
            context.WithInjectionParameter("test1", new Test1 { Name = "fakeName" }));
        var inst2 = container.Resolve<ITest2>();

        Assert.IsType<Test2>(inst2);
        Assert.Equal("fakeName", inst2.Name);

        container.Register<ITest3, Test3>();

        var inst1 = container.Resolve<ITest1>();
        inst1.Name = "test1";
        container.Resolve<ITest3>();

        var factory = container.Resolve<Func<ITest1, ITest2, ITest3>>();
        var inst3 = factory(inst1, inst2);

        Assert.NotNull(inst3);
        Assert.IsType<Test3>(inst3);
        Assert.Equal("test1fakeNametest1", inst3.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Parallel()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>(context =>
            context.WithInjectionParameter("test1", new Test1 { Name = "fakeName" }));
        var inst2 = container.Resolve<ITest2>();

        Assert.IsType<Test2>(inst2);
        Assert.Equal("fakeName", inst2.Name);

        container.Register<ITest3, Test3>();

        Parallel.For(0, 50000, (i) =>
        {
            var inst1 = container.Resolve<ITest1>();
            inst1.Name = "test1";
            var factory = container.Resolve<Func<ITest1, ITest2, ITest3>>();
            var inst3 = factory(inst1, inst2);

            Assert.NotNull(inst3);
            Assert.IsType<Test3>(inst3);
            Assert.Equal("test1fakeNametest1", inst3.Name);
        });
    }

    [Fact]
    public void OverrideTests_Resolve_Factory_NonGeneric()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>();

        var inst1 = container.Resolve<ITest1>();
        inst1.Name = "test1";
        container.Resolve<ITest2>();

        var factory = container.ResolveFactory(typeof(ITest2), parameterTypes: typeof(ITest1));
        var inst2 = ((Func<ITest1, ITest2>)factory)(inst1);

        Assert.NotNull(inst2);
        Assert.IsType<Test2>(inst2);
        Assert.Equal("test1", inst2.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Factory_NonGeneric_DerivedParam()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();

        var inst1 = new Test1 { Name = "test" };

        var factory = container.ResolveFactory(typeof(ITest2), parameterTypes: inst1.GetType());
        var inst2 = (ITest2)factory.DynamicInvoke(inst1);

        Assert.NotNull(inst2);
        Assert.Equal("test", inst2.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Factory()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>();

        var inst1 = container.Resolve<ITest1>();
        inst1.Name = "test1";
        container.Resolve<ITest2>();

        var factory = container.Resolve<Func<ITest1, ITest2>>();
        var inst2 = factory(inst1);

        Assert.NotNull(inst2);
        Assert.IsType<Test2>(inst2);
        Assert.Equal("test1", inst2.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Factory_NonGeneric_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>();

        var inst1 = container.Resolve<ITest1>();
        inst1.Name = "test1";
        container.Resolve<ITest2>();

        var factory = container.ResolveFactory(typeof(Lazy<ITest2>), parameterTypes: typeof(ITest1));
        var inst2 = ((Func<ITest1, Lazy<ITest2>>)factory)(inst1);

        Assert.NotNull(inst2);
        Assert.IsType<Lazy<ITest2>>(inst2);
        Assert.Equal("test1", inst2.Value.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Factory_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>();

        var inst1 = container.Resolve<ITest1>();
        inst1.Name = "test1";
        container.Resolve<ITest2>();

        var factory = container.Resolve<Func<ITest1, Lazy<ITest2>>>();
        var inst2 = factory(inst1);

        Assert.NotNull(inst2);
        Assert.IsType<Lazy<ITest2>>(inst2);
        Assert.Equal("test1", inst2.Value.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Parallel_Lazy()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest2, Test2>(context =>
            context.WithInjectionParameter("test1", new Test1 { Name = "fakeName" }));
        var inst2 = container.Resolve<Lazy<ITest2>>();

        Assert.IsType<Test2>(inst2.Value);
        Assert.Equal("fakeName", inst2.Value.Name);

        container.Register<ITest3, Test3>();

        Parallel.For(0, 50000, (i) =>
        {
            var inst1 = container.Resolve<Lazy<ITest1>>();
            inst1.Value.Name = "test1";

            var factory = container.Resolve<Func<ITest1, ITest2, Lazy<ITest3>>>();
            var inst3 = factory(inst1.Value, inst2.Value);

            Assert.NotNull(inst3);
            Assert.IsType<Test3>(inst3.Value);
            Assert.Equal("test1fakeNametest1", inst3.Value.Name);
            Assert.True(inst3.Value.MethodInvoked);
        });
    }

    [Fact]
    public void OverrideTests_Resolve_Multiple()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();
        container.Register<ITest4, Test4>();
        container.Register<ITest6, Test6>();

        var inst4 = container.Resolve<ITest4>();

        Assert.NotNull(inst4);
        Assert.IsType<Test4>(inst4);
        Assert.Equal("test2Test6", inst4.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_DependencyOverride()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();

        var inst = container.Resolve<ITest2>(dependencyOverrides: [new Test1 { Name = "test" }]);

        Assert.NotNull(inst);
        Assert.IsType<Test2>(inst);
        Assert.Equal("test", inst.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_DependencyOverride_Should_Not_Cached()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();

        var inst = container.Resolve<ITest2>(dependencyOverrides: [new Test1 { Name = "test" }]);

        Assert.NotNull(inst);
        Assert.IsType<Test2>(inst);
        Assert.Equal("test", inst.Name);

        var inst2 = container.Resolve<ITest2>(dependencyOverrides: [new Test1 { Name = "test2" }]);

        Assert.NotNull(inst2);
        Assert.IsType<Test2>(inst2);
        Assert.Equal("test2", inst2.Name);
    }
    
    [Fact]
    public void OverrideTests_Resolve_DependencyOverride_Named()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>(c => c.WithDependencyBinding<ITest1>("test"));

        var inst = container.Resolve<ITest2>( 
        [
            Override.Of(typeof(ITest1), new Test1 { Name = "test" }, "test"), 
            Override.Of<ITest1>(new Test1 { Name = "test2" }, "test2")
        ]);

        Assert.NotNull(inst);
        Assert.IsType<Test2>(inst);
        Assert.Equal("test", inst.Name);

        Assert.Throws<ResolutionFailedException>(() =>
        {
            container.Resolve<ITest2>(
            [
                Override.Of<ITest1>(new Test1 { Name = "test" }), 
                Override.Of<ITest1>(new Test1 { Name = "test2" })
            ]);
        });
    }
    
    [Fact]
    public void OverrideTests_Resolve_DependencyOverride_NotNamed()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();

        var inst = container.Resolve<ITest2>(dependencyOverrides: 
        [
            Override.Of<ITest1>(new Test1 { Name = "test" })
        ]);

        Assert.NotNull(inst);
        Assert.IsType<Test2>(inst);
        Assert.Equal("test", inst.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_DependencyOverride_NonGeneric()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();

        var inst = (ITest2)container.Resolve(typeof(ITest2), dependencyOverrides: [new Test1 { Name = "test" }]);

        Assert.NotNull(inst);
        Assert.IsType<Test2>(inst);
        Assert.Equal("test", inst.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_DependencyOverride_All()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();

        var inst = container.ResolveAll<ITest2>([new Test1 { Name = "test" }]).First();

        Assert.NotNull(inst);
        Assert.IsType<Test2>(inst);
        Assert.Equal("test", inst.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_DependencyOverride_All_NonGeneric()
    {
        IStashboxContainer container = new StashboxContainer();
        container.Register<ITest2, Test2>();

        var inst = (ITest2)container.ResolveAll(typeof(ITest2), [new Test1 { Name = "test" }]).First();

        Assert.NotNull(inst);
        Assert.IsType<Test2>(inst);
        Assert.Equal("test", inst.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Func()
    {
        using var container = new StashboxContainer();
        container.RegisterFunc<string, ITest1>((name, dr) => new Test1 { Name = name });
        container.Register<Test7>();

        var inst = container.Resolve<Test7>();

        Assert.NotNull(inst);
        Assert.Equal("testfromfunc", inst.Name);
    }

    [Fact]
    public void OverrideTests_Resolve_Func_From_Parameter()
    {
        using var container = new StashboxContainer();
        container.Register<Test8>().Register<Test9>();

        container.Resolve<Test8>();
        var inst = container.Resolve<Test9>();

        Assert.NotNull(inst);
        Assert.Equal(nameof(Test9), inst.Name);
    }

    private interface ITest1 { string Name { get; set; } }

    private interface ITest2 { string Name { get; set; } }

    private interface ITest3 { string Name { get; set; } bool MethodInvoked { get; } }

    private interface ITest4 { string Name { get; set; } }

    private interface ITest5 { string Name { get; set; } }

    private interface ITest6 { string Name { get; set; } }

    private class Test4 : ITest4
    {
        public string Name { get; set; }

        public Test4(Func<ITest1, ITest2> test1, Func<ITest5, ITest6> test2)
        {
            this.Name = test1(new Test1 { Name = "test2" }).Name + test2(new Test5 { Name = "Test6" }).Name;
        }
    }

    private class Test5 : ITest5
    {
        public string Name { get; set; }
    }

    private class Test6 : ITest6
    {
        public string Name { get; set; }

        public Test6(ITest5 test1)
        {
            this.Name = test1.Name;
        }
    }

    private class Test1 : ITest1
    {
        public string Name { get; set; }
    }

    private class Test2 : ITest2
    {
        public string Name { get; set; }

        public Test2(ITest1 test1)
        {
            this.Name = test1.Name;
        }
    }

    private class Test3 : ITest3
    {
        public string Name { get; set; }

        public bool MethodInvoked { get; set; }

        public Test3(ITest1 test1, ITest2 test2)
        {
            Name = test1.Name + test2.Name;
        }

        [InjectionMethod]
        public void Inject(ITest1 test)
        {
            Shield.EnsureNotNull(test, nameof(test));
            this.MethodInvoked = true;
            Name += test.Name;
        }
    }

    private class Test7
    {
        public string Name { get; set; }

        public Test7(Func<string, ITest1> func)
        {
            this.Name = func("testfromfunc").Name;
        }
    }

    private class Test8
    {
        public string Name { get; set; }

        public Test8(string name = "")
        {
            this.Name = name;
        }
    }

    private class Test9
    {
        public string Name { get; set; }

        public Test9(Func<string, Test8> func)
        {
            this.Name = func(nameof(Test9)).Name;
        }
    }
}