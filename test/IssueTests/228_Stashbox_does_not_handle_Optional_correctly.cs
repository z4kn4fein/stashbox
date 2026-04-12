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
    
    [Fact]
    public void Optional_Works_Unknown()
    {
        using var container = new StashboxContainer(c => c.WithUnknownTypeResolution());
        container.Register<E>();

        Assert.NotNull(container.Resolve<E>());
    }
    
    private class A { }
    
    private class B([Optional] A? a) { }
    
    private class C([Optional] int? a) { }
    
    private class D([Optional] string? a) { }
    
    private class E([Optional] A a) { }
}