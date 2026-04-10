#nullable enable

using System.Runtime.InteropServices;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class StashboxDoesNotHandleOptionalCorrectly 
{
    [Fact]
    public void Optional_Works()
    {
        using var container = new StashboxContainer();
        container.Register<B>();
        container.Register<C>();
        container.Register<D>();

        Assert.NotNull(container.Resolve<B>());
        Assert.NotNull(container.Resolve<C>());
        Assert.NotNull(container.Resolve<D>());
    }
    
    private class A { }
    
    private class B([Optional] A? a) { }
    
    private class C([Optional] int? a) { }
    
    private class D([Optional] string? a) { }
}