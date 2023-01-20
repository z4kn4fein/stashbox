using System.Linq;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class ProgrammaticMultipleInstancesRegistration
{
    [Fact]
    public void Ensure_Multiple_Instance_Registration_Working()
    {
        var regs = new StashboxContainer()
            .RegisterInstances<ITest>(new Test(), new Test1()).GetRegistrationMappings();

        Assert.Equal(2, regs.Count());
    }

    [Fact]
    public void Ensure_Multiple_Instance_Registration_Working_Enumerable()
    {
        var regs = new StashboxContainer()
            .RegisterInstances(new ITest[] { new Test(), new Test1() }).GetRegistrationMappings();

        Assert.Equal(2, regs.Count());
    }

    interface ITest { }

    class Test : ITest { }

    class Test1 : ITest { }
}