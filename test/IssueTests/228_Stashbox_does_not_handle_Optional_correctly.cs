#nullable enable

using System;
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

        Assert.NotNull(container.Resolve<B>());
    }
    
    private class A { }
    
    private class B([Optional] A? a) { }
}