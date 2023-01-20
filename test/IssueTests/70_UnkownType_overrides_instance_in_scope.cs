using Xunit;

namespace Stashbox.Tests.IssueTests;

public class UnkownTypOverridesInstanceInScope
{
    [Fact]
    public void Ensure_UnknownType_Doesnt_Overrides_Instance_In_Scope()
    {
        var container = new StashboxContainer(c => c.WithUnknownTypeResolution());
        var inst = container.Resolve<object>();

        using var scope = container.BeginScope();
        var @new = new object();
        scope.PutInstanceInScope(@new);

        var scoped = scope.Resolve<object>();
        Assert.NotSame(inst, scoped);
        Assert.Same(@new, scoped);
    }
}