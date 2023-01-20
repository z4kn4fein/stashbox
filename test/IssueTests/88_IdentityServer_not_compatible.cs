using Xunit;

namespace Stashbox.Tests.IssueTests;

public class IdentityServerNotCompatible
{
    [Fact]
    public void Ensure_SubGraph_Cache_Not_Messes_Up_The_Graph()
    {
        using var container = new StashboxContainer()
            .Register<TransientRoot>()
            .Register<TransientProxy>()
            .RegisterScoped<ScopedProxy>()
            .RegisterScoped<Scoped>();

        Assert.NotNull(container.Resolve<TransientRoot>());
    }

    [Fact]
    public void Ensure_SubGraph_Cache_Not_Messes_Up_The_Graph_2()
    {
        using var container = new StashboxContainer()
            .Register<TransientRoot2>()
            .Register<TransientProxy>()
            .RegisterScoped<ScopedProxy>()
            .RegisterScoped<Scoped>();

        Assert.NotNull(container.Resolve<TransientRoot2>());
    }

    class TransientRoot2
    {
        public TransientRoot2(TransientProxy d, ScopedProxy c)
        { }
    }

    class TransientRoot
    {
        public TransientRoot(ScopedProxy c, TransientProxy d)
        { }
    }

    class TransientProxy
    {
        public TransientProxy(Scoped d)
        { }
    }

    class Scoped
    { }

    class ScopedProxy
    {
        public ScopedProxy(TransientProxy b)
        {

        }
    }
}