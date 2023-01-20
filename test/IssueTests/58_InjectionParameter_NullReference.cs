using Xunit;

namespace Stashbox.Tests.IssueTests;

public class InjectionParameterNullReference
{
    [Fact]
    public void InjectionParameter_NullReference()
    {
        var inst = new StashboxContainer()
            .Register<Test>(c => c.WithInjectionParameter("arg", null))
            .Resolve<Test>();

        Assert.NotNull(inst);
    }

    [Fact]
    public void InjectionParameter_NullReference_Object()
    {
        var inst = new StashboxContainer()
            .Register<Test2>(c => c.WithInjectionParameter("arg", null))
            .Resolve<Test2>();

        Assert.NotNull(inst);
    }

    class Test
    {
        public Test(Test2 arg)
        { }
    }

    class Test2
    {
        public Test2(object arg)
        { }
    }
}