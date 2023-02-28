using TestAssembly;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class Issue129 
{
    [Fact]
    public void ResolveAllResolvesExistingInstance()
    {
        using var container = new StashboxContainer();
        container.RegisterAssemblyContaining<ITA_T1>(
            type => typeof(ITA_T1).IsAssignableFrom(type),
            configurator: opt => opt.WithSingletonLifetime()
        );

        var all = container.ResolveAll<ITA_T1>();
        var instance = container.Resolve<TA_T1>();

        Assert.Contains(all, it => it is TA_T1);
        Assert.Contains(all, it => it == instance);
    }
    
    [Fact]
    public void ResolveAllResolvesExistingInstance_Ensure_AsImplementedTypes_Doesnt_Replace()
    {
        using var container = new StashboxContainer();
        container.RegisterAssemblyContaining<ITA_T1>(
            type => typeof(ITA_T1).IsAssignableFrom(type),
            configurator: opt => opt.WithSingletonLifetime().AsImplementedTypes()
        );

        var all = container.ResolveAll<ITA_T1>();
        var instance = container.Resolve<TA_T1>();

        Assert.Contains(all, it => it is TA_T1);
        Assert.Contains(all, it => it == instance);
    }
}