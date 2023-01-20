using System.Collections.Generic;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class MakeInjectionParameterConfigurationMoreFluent
{
    [Fact]
    public void MakeInjectionParameterConfigurationMoreFluent_Mixed()
    {
        var instance = new StashboxContainer()
            .Register<Test1>(c => c.WithAutoMemberInjection()
                .WithInjectionParameter(nameof(Test1.TestString), "sample")
                .WithInjectionParameters(new KeyValuePair<string, object>(nameof(Test1.TestInt), 5)))
            .Resolve<Test1>();

        Assert.Equal("sample", instance.TestString);
        Assert.Equal(5, instance.TestInt);
    }

    class Test1
    {
        public string TestString { get; set; }

        public int TestInt { get; set; }
    }
}