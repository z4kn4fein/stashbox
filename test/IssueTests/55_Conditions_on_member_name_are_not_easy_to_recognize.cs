using Xunit;

namespace Stashbox.Tests.IssueTests;

public class ConditionsOnMemberNameAreNotEasyToRecognize
{
    [Fact]
    public void ConditionsOnMemberNameAreNotEasyToRecognize_UseParameterOrMemberName()
    {
        var container = new StashboxContainer();
        container.Register<ITest1, Test1>(context => context.When(t => t.ParameterOrMemberName == "Test1"));
        container.Register<ITest1, Test11>(context => context.When(t => t.ParameterOrMemberName == "Test11"));
        container.Register<Test2>(c => c.WithAutoMemberInjection());

        var test5 = container.Resolve<Test2>();

        Assert.IsType<Test1>(test5.Test1);
        Assert.IsType<Test11>(test5.Test11);
    }

    interface ITest1;

    class Test1 : ITest1;

    class Test11 : ITest1;

    class Test2
    {
        public ITest1 Test1 { get; set; }

        public ITest1 Test11 { get; set; }
    }
}