using Stashbox.Attributes;
using System;
using Xunit;

namespace Stashbox.Tests;

public class ActivateTests
{
    [Fact]
    public void ActivateTests_Full()
    {
        var inst = new StashboxContainer().Register<Test>().Activate<Test1>();

        Assert.NotNull(inst.Test);
        Assert.NotNull(inst.Test2);

        Assert.True(inst.InjectionMethodCalled);
    }

    [Fact]
    public void ActivateTests_Full_DepOverride()
    {
        var test = new Test();
        var inst = new StashboxContainer().Activate<Test1>(test);

        Assert.Same(test, inst.Test);
        Assert.Same(test, inst.Test2);

        Assert.True(inst.InjectionMethodCalled);
    }

    [Fact]
    public void ActivateTests_Fail()
    {
        Assert.Throws<ArgumentException>(() => new StashboxContainer().Activate<ITest>());
    }

    [Fact]
    public void ActivateTests_Fail_On_Scope()
    {
        Assert.Throws<ArgumentException>(() => new StashboxContainer().BeginScope().Activate<ITest>());
    }

    interface ITest { }

    class Test : ITest { }

    class Test1
    {
        public bool InjectionMethodCalled { get; set; }

        public Test Test { get; set; }

        [Dependency]
        public Test Test2 { get; set; }

        public Test1(Test test)
        {
            this.Test = test;
        }

        [InjectionMethod]
        public void InjectMethod()
        {
            this.InjectionMethodCalled = true;
        }
    }
}