using Stashbox.Attributes;
using System;
using Xunit;

namespace Stashbox.Tests;

public class ConditionalTests
{
    [Fact]
    public void ConditionalTests_ParentTypeCondition_First()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenDependantIs<Test2>());
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>();

        var test2 = container.Resolve<ITest2>();

        Assert.IsType<Test1>(test2.test1);
        Assert.IsType<Test1>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_When_First()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.When(type => type.ParentType == typeof(Test2)));
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>();

        var test2 = container.Resolve<ITest2>();

        Assert.IsType<Test1>(test2.test1);
        Assert.IsType<Test1>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_First_NonGeneric()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenDependantIs(typeof(Test2)));
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>();

        var test2 = container.Resolve<ITest2>();

        Assert.IsType<Test1>(test2.test1);
        Assert.IsType<Test1>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_Second()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>(context => context.WhenDependantIs<Test2>());
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>();

        var test2 = container.Resolve<ITest2>();

        Assert.IsType<Test11>(test2.test1);
        Assert.IsType<Test11>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_Second_NonGeneric()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>(context => context.WhenDependantIs(typeof(Test2)));
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test2>();

        var test2 = container.Resolve<ITest2>();

        Assert.IsType<Test11>(test2.test1);
        Assert.IsType<Test11>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_Third()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>(context => context.WhenDependantIs<Test2>());
        container.Register<ITest2, Test2>();

        var test2 = container.Resolve<ITest2>();

        Assert.IsType<Test12>(test2.test1);
        Assert.IsType<Test12>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_Third_NonGeneric()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>(context => context.WhenDependantIs(typeof(Test2)));
        container.Register<ITest2, Test2>();

        var test2 = container.Resolve<ITest2>();

        Assert.IsType<Test12>(test2.test1);
        Assert.IsType<Test12>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_Third_WithName()
    {
        var name = "A".ToLower();
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>(context => context.WhenDependantIs<Test2>(name));
        container.Register<ITest2, Test2>(name);

        var test2 = container.Resolve<ITest2>(name);

        Assert.IsType<Test12>(test2.test1);
        Assert.IsType<Test12>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_ParentTypeCondition_Third_NonGenericWithName()
    {
        var name = "A".ToLower();
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>(context => context.WhenDependantIs(typeof(Test2), name));
        container.Register<ITest2, Test2>(name);

        var test2 = container.Resolve<ITest2>(name);

        Assert.IsType<Test12>(test2.test1);
        Assert.IsType<Test12>(test2.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_First()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenHas<TestConditionAttribute>());
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>(context => context.WhenHas<TestCondition2Attribute>());
        container.Register<ITest2, Test3>();

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test1>(test3.test1);
        Assert.IsType<Test12>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_First_NonGeneric()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenHas(typeof(TestConditionAttribute)));
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>(context => context.WhenHas(typeof(TestCondition2Attribute)));
        container.Register<ITest2, Test3>();

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test1>(test3.test1);
        Assert.IsType<Test12>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_Second()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>(context => context.WhenHas<TestCondition2Attribute>());
        container.Register<ITest1, Test12>(context => context.WhenHas<TestConditionAttribute>());
        container.Register<ITest2, Test3>();

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test12>(test3.test1);
        Assert.IsType<Test11>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_Second_NonGeneric()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>();
        container.Register<ITest1, Test11>(context => context.WhenHas(typeof(TestCondition2Attribute)));
        container.Register<ITest1, Test12>(context => context.WhenHas(typeof(TestConditionAttribute)));
        container.Register<ITest2, Test3>();

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test12>(test3.test1);
        Assert.IsType<Test11>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_Third()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenHas<TestCondition2Attribute>());
        container.Register<ITest1, Test11>(context => context.WhenHas<TestConditionAttribute>());
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test3>();

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test11>(test3.test1);
        Assert.IsType<Test1>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_Third_NonGeneric()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenHas(typeof(TestCondition2Attribute)));
        container.Register<ITest1, Test11>(context => context.WhenHas(typeof(TestConditionAttribute)));
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test3>();

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test11>(test3.test1);
        Assert.IsType<Test1>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_Third_WithName()
    {
        var name = "A".ToLower();
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenHas<TestCondition2Attribute>().WithName(name));
        container.Register<ITest1, Test11>(context => context.WhenHas<TestCondition2Attribute>().WithName("B"));
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test3>(context => context.WithDependencyBinding<ITest1>(name));

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test12>(test3.test1);
        Assert.IsType<Test1>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_AttributeCondition_Third_NonGeneric_WithName()
    {
        var name = "A".ToLower();
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenHas(typeof(TestCondition2Attribute)).WithName(name));
        container.Register<ITest1, Test11>(context => context.WhenHas(typeof(TestConditionAttribute)).WithName("B"));
        container.Register<ITest1, Test12>();
        container.Register<ITest2, Test3>(context => context.WithDependencyBinding<ITest1>(name));

        var test3 = container.Resolve<ITest2>();

        Assert.IsType<Test12>(test3.test1);
        Assert.IsType<Test1>(test3.test12);
    }

    [Fact]
    public void ConditionalTests_Combined()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.WhenDependantIs<Test4>().WhenDependantIs<Test5>());
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<Test4>();
        container.Register<Test5>();

        var t1 = container.Resolve<Test4>();
        var t2 = container.Resolve<Test5>();

        Assert.IsType<Test1>(t1.Test);
        Assert.IsType<Test1>(t2.Test);
    }

    [Fact]
    public void ConditionalTests_Combined_Common()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.When(t => t.ParentType == typeof(Test4)).When(t => t.ParentType == typeof(Test5)));
        container.Register<ITest1, Test11>();
        container.Register<ITest1, Test12>();
        container.Register<Test4>();
        container.Register<Test5>();

        var t1 = container.Resolve<Test4>();
        var t2 = container.Resolve<Test5>();

        Assert.IsType<Test1>(t1.Test);
        Assert.IsType<Test1>(t2.Test);
    }

    [Fact]
    public void ConditionalTests_InResolutionPath()
    {
        var container = new StashboxContainer();
        container.Register<Dummy>(context => context.WhenInResolutionPathOf<Test4>());
        container.Register<Test4>();
        container.Register<ITest1, Test13>();

        var t = container.Resolve<Test4>();

        Assert.NotNull(((Test13)t.Test).Dummy);
    }

    [Fact]
    public void ConditionalTests_InResolutionPath_Attribute()
    {
        var container = new StashboxContainer();
        container.Register<Dummy>(context => context.WhenResolutionPathHas<TestConditionAttribute>());
        container.Register<Test6>();
        container.Register<ITest1, Test13>();

        var t = container.Resolve<Test6>();

        Assert.NotNull(((Test13)t.Test).Dummy);
    }

    [Fact]
    public void ConditionalTests_InResolutionPath_WithName()
    {
        var name = "A".ToLower();
        var container = new StashboxContainer();
        container.Register<Dummy>(context => context.WhenInResolutionPathOf<Test4>(name));
        container.Register<Test4>(name);
        container.Register<ITest1, Test13>();

        var t = container.Resolve<Test4>(name);

        Assert.NotNull(((Test13)t.Test).Dummy);
    }

    [Fact]
    public void ConditionalTests_InResolutionPath_Attribute_WithName()
    {
        var name = "A".ToLower();
        var container = new StashboxContainer();
        container.Register<Dummy>(context => context.WhenResolutionPathHas<TestConditionAttribute>(name));
        container.Register<Test6>(context => context.WithDependencyBinding<ITest1>(name));
        container.Register<ITest1, Test13>(name);

        var t = container.Resolve<Test6>();

        Assert.NotNull(((Test13)t.Test).Dummy);
    }

    interface ITest1 { }

    interface ITest2 { ITest1 test1 { get; set; } ITest1 test12 { get; set; } }

    class Dummy { }

    class Test1 : ITest1
    { }

    class Test11 : ITest1
    { }

    class Test12 : ITest1
    { }

    class Test13 : ITest1
    {
        public Test13(Dummy dummy)
        {
            Dummy = dummy;
        }

        public Dummy Dummy { get; }
    }

    class Test2 : ITest2
    {
        [Dependency]
        public ITest1 test1 { get; set; }
        public ITest1 test12 { get; set; }

        public Test2(ITest1 test12)
        {
            this.test12 = test12;
        }
    }

    class Test3 : ITest2
    {
        [Dependency, TestCondition]
        public ITest1 test1 { get; set; }

        public ITest1 test12 { get; set; }

        public Test3([TestCondition2] ITest1 test12)
        {
            this.test12 = test12;
        }
    }

    class Test4
    {
        public Test4(ITest1 test)
        {
            Test = test;
        }

        public ITest1 Test { get; }
    }

    class Test5
    {
        public Test5(ITest1 test)
        {
            Test = test;
        }

        public ITest1 Test { get; }
    }

    class Test6
    {
        public Test6([TestCondition]ITest1 test)
        {
            Test = test;
        }

        public ITest1 Test { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
    class TestConditionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
    class TestCondition2Attribute : Attribute
    {
    }
}