using Stashbox.Attributes;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class A
{ }
public class B
{ }

public class BaseClass
{
    [Dependency]
    public A A { get; set; }
    public bool DoneA { get; set; }

    [InjectionMethod]
    public void InjectA()
    {
        DoneA = true;
    }
}

public class MainClass : BaseClass
{
    [Dependency]
    public B B { get; set; }
    public bool DoneB { get; set; }

    [InjectionMethod]
    public void InjectB()
    {
        DoneB = true;
    }
}

public class BaseClassMethod
{
    [Fact]
    public void Test()
    {
        var container = new StashboxContainer();
        container.Register<A>().Register<B>().Register<MainClass>();

        var main = container.Resolve<MainClass>();

        Assert.IsType<B>(main.B);
        Assert.IsType<A>(main.A);

        Assert.True(main.DoneA);
        Assert.True(main.DoneB);
    }
}