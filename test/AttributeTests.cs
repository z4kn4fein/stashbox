using Stashbox.Attributes;
using Stashbox.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests;

public class AttributeTest
{
    [Fact]
    public void AttributeTests_Resolve()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test1>(context => context.WithName("test1"));
        container.Register<ITest1, Test11>(context => context.WithName("test11"));
        container.Register<ITest1, Test12>(context => context.WithName("test12"));
        container.Register<ITest2, Test2>(context => context.WithName("test2"));
        container.Register<ITest2, Test22>(context => context.WithName("test22"));
        container.Register<ITest3, Test3>();
        container.Register<ITest4, Test4>();

        var test1 = container.Resolve<ITest1>();
        var test2 = container.Resolve<ITest2>("test2");
        var test3 = container.Resolve<ITest3>();
        var test4 = container.Resolve<Lazy<ITest4>>();

        Assert.NotNull(test1);
        Assert.NotNull(test2);
        Assert.NotNull(test3);
        Assert.NotNull(test4.Value);

        Assert.True(test3.MethodInvoked);
        Assert.True(test3.MethodInvoked2);

        Assert.IsType<Test11>(test3.test1);
        Assert.IsType<Test22>(test3.test2);
    }

    [Fact]
    public void AttributeTests_Named_Resolution()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WithName("test1"));
        container.Register<ITest1, Test11>(context => context.WithName("test11"));
        container.Register<ITest1, Test12>(context => context.WithName("test12"));
        container.Register<ITest2, Test2>(context => context.WithName("test2"));
        container.Register<ITest2, Test22>(context => context.WithName("test22"));

        var test1 = container.Resolve<ITest1>("test1");
        var test11 = container.Resolve<ITest1>("test11");
        var test12 = container.Resolve<ITest1>("test12");
        var test2 = container.Resolve<ITest2>("test2");
        var test22 = container.Resolve<ITest2>("test22");

        Assert.IsType<Test1>(test1);
        Assert.IsType<Test11>(test11);
        Assert.IsType<Test12>(test12);
        Assert.IsType<Test2>(test2);
        Assert.IsType<Test22>(test22);
    }

    [Fact]
    public void AttributeTests_Parallel_Resolve()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test1>(context => context.WithName("test1"));
        container.Register<ITest1, Test11>(context => context.WithName("test11"));
        container.Register<ITest1, Test12>(context => context.WithName("test12"));
        container.Register<ITest2, Test2>(context => context.WithName("test2"));
        container.Register<ITest2, Test22>(context => context.WithName("test22"));
        container.Register<ITest3, Test3>();

        Parallel.For(0, 50000, (i) =>
        {
            var test1 = container.Resolve<ITest1>();
            var test2 = container.Resolve<ITest2>("test2");
            var test3 = container.Resolve<ITest3>();

            Assert.NotNull(test1);
            Assert.NotNull(test2);
            Assert.NotNull(test3);

            Assert.True(test3.MethodInvoked);

            Assert.IsType<Test11>(test3.test1);
            Assert.IsType<Test22>(test3.test2);
        });
    }

    [Fact]
    public void AttributeTests_Parallel_Lazy_Resolve()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test1>(context => context.WithName("test1"));
        container.Register<ITest1, Test11>(context => context.WithName("test11"));
        container.Register<ITest1, Test12>(context => context.WithName("test12"));
        container.Register<ITest2, Test2>(context => context.WithName("test2"));
        container.Register<ITest2, Test22>(context => context.WithName("test22"));
        container.Register<ITest3, Test3>();
        container.Register<ITest4, Test4>();

        Parallel.For(0, 50000, (i) =>
        {
            var test1 = container.Resolve<Lazy<ITest1>>();
            var test2 = container.Resolve<Lazy<ITest2>>("test2");
            var test3 = container.Resolve<Lazy<ITest3>>();
            var test4 = container.Resolve<Lazy<ITest4>>();

            Assert.NotNull(test1.Value);
            Assert.NotNull(test2.Value);
            Assert.NotNull(test3.Value);
            Assert.NotNull(test4.Value);

            Assert.True(test3.Value.MethodInvoked);
            Assert.True(test4.Value.MethodInvoked);

            Assert.IsType<Test11>(test3.Value.test1);
            Assert.IsType<Test22>(test3.Value.test2);

            Assert.IsType<Test11>(test4.Value.test1.Value);
            Assert.IsType<Test22>(test4.Value.test2.Value);
        });
    }

    [Fact]
    public void AttributeTests_InjectionMethod_WithoutMembers()
    {
        var inst = new StashboxContainer()
            .Register<Test5>()
            .Resolve<Test5>();

        Assert.True(inst.MethodInvoked);
    }

    [Fact]
    public void AttributeTests_InjectionMethod_Private()
    {
        var inst = new StashboxContainer()
            .Register<Test6>()
            .Resolve<Test6>();

        Assert.True(inst.MethodInvoked);
    }

    interface ITest1 { }

    interface ITest2 { ITest1 test1 { get; set; } }

    interface ITest3 { ITest2 test2 { get; set; } ITest1 test1 { get; set; } bool MethodInvoked { get; set; } bool MethodInvoked2 { get; set; } }

    interface ITest4 { Lazy<ITest2> test2 { get; set; } Lazy<ITest1> test1 { get; set; } bool MethodInvoked { get; set; } }

    class Test1 : ITest1
    { }

    class Test11 : ITest1
    { }

    class Test12 : ITest1
    { }

    class Test22 : ITest2 { public ITest1 test1 { get; set; } }

    class Test2 : ITest2
    {
        public ITest1 test1 { get; set; }

        public Test2([Dependency("test11")] ITest1 test1)
        {
            Shield.EnsureNotNull(test1, nameof(test1));
            Shield.EnsureTypeOf<Test11>(test1);
        }
    }

    class Test3 : ITest3
    {
        [Dependency("test11")]
        public ITest1 test1 { get; set; }

        [Dependency("test22")]
        public ITest2 test2 { get; set; }

        [InjectionMethod]
        public void MethodTest([Dependency("test22")] ITest2 test2)
        {
            Shield.EnsureNotNull(test2, nameof(test2));
            this.MethodInvoked = true;
        }

        [InjectionMethod]
        public void MethodTest2([Dependency("test22")] ITest2 test2)
        {
            Shield.EnsureNotNull(test2, nameof(test2));
            this.MethodInvoked2 = true;
        }

        public Test3([Dependency("test12")] ITest1 test12, [Dependency("test2")] ITest2 test2)
        {
            Shield.EnsureNotNull(test12, nameof(test12));
            Shield.EnsureNotNull(test2, nameof(test2));

            Shield.EnsureTypeOf<Test12>(test12);
            Shield.EnsureTypeOf<Test2>(test2);
        }

        public bool MethodInvoked { get; set; }
        public bool MethodInvoked2 { get; set; }
    }

    class Test4 : ITest4
    {
        [Dependency("test11")]
        public Lazy<ITest1> test1 { get; set; }

        [Dependency("test22")]
        public Lazy<ITest2> test2 { get; set; }

        [InjectionMethod]
        public void MethodTest([Dependency("test22")] Lazy<ITest2> test2)
        {
            Shield.EnsureNotNull(test2.Value, nameof(test2.Value));
            this.MethodInvoked = true;
        }

        public Test4([Dependency("test12")] Lazy<ITest1> test1, [Dependency("test2")] Lazy<ITest2> test2)
        {
            Shield.EnsureNotNull(test1.Value, nameof(test1.Value));
            Shield.EnsureNotNull(test2.Value, nameof(test2.Value));

            Shield.EnsureTypeOf<Test12>(test1.Value);
            Shield.EnsureTypeOf<Test2>(test2.Value);
        }

        public bool MethodInvoked { get; set; }
    }

    class Test5
    {
        [InjectionMethod]
        public void MethodTest()
        {
            this.MethodInvoked = true;
        }

        public bool MethodInvoked { get; set; }
    }

    class Test6
    {
        [InjectionMethod]
        private void MethodTest()
        {
            this.MethodInvoked = true;
        }

        public bool MethodInvoked { get; set; }
    }
}