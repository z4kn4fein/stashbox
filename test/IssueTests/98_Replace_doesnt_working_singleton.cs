using Xunit;

namespace Stashbox.Tests.IssueTests;

public class ReplaceDoesntWorkingSingleton
{
    [Fact]
    public void Ensure_Replace_Works_With_Singleton()
    {
        using var container = new StashboxContainer();

        container.Register<ITest, Test1>(context => context.WithName("test").WithSingletonLifetime());

        var test1 = container.Resolve<ITest>("test");

        Assert.IsType<Test1>(test1);

        container.Register<ITest, Test2>(context => context.WithName("test").ReplaceExisting().WithSingletonLifetime());

        var test2 = container.Resolve<ITest>("test");

        Assert.IsType<Test2>(test2);
    }

    interface ITest { }

    class Test1 : ITest
    { }

    class Test2 : ITest
    { }
}