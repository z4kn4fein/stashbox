using System.Collections.Generic;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class ExtensionsIdentityOptionsMonitor 
{
    [Fact]
    public void ExtensionsIdentityOptionsMonitor_WithoutVariance()
    {
        using var container = new StashboxContainer(c => c.WithVariantGenericTypes(false));
        container.Register<IOp<A>, Op<A>>();
        container.Register<IOp<B>, Op<B>>();

        Assert.Single(container.Resolve<IEnumerable<IOp<B>>>());
    }

    private interface IOp<in T>;

    private class Op<T> : IOp<T>;

    private class A;

    private class B : A;
}