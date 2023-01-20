using Xunit;

namespace Stashbox.Tests.IssueTests;

public class MixtureOfNamedAndNonNamedRegistrationTests
{
    [Fact]
    public void Mixture_of_named_and_non_named_registrations_result_in_the_wrong_type_resolved()
    {
        var sb = new StashboxContainer();

        sb.RegisterSingleton(typeof(ITest), typeof(Test));
        sb.RegisterSingleton(typeof(ITest), typeof(Test1), "Test2");
        sb.RegisterSingleton(typeof(ITest), typeof(Test2), "Test3");
        var test = sb.Resolve<ITest>();

        Assert.NotNull(test);
        Assert.IsType<Test>(test);
    }

    interface ITest
    { }

    class Test : ITest
    { }

    class Test1 : ITest
    { }

    class Test2 : ITest
    { }
}