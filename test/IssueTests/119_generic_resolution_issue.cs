using System.Linq;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class GenericResolutionIssue
{
    [Fact]
    public void Ensure_AsServiceAlso_works()
    {
        using var container = new StashboxContainer();
        container.Register<AT>(c => c.AsServiceAlso<IA<C>>().AsServiceAlso<IA<C, long>>().AsServiceAlso<IB<C>>().AsServiceAlso<IC<C, string>>());

        var inst = container.Resolve<AT>();

        Assert.NotNull(inst);

        var mappings = container.GetRegistrationMappings();

        Assert.Equal(5, mappings.Count());
    }

    class C;

    class AT : IA<C>, IA<C, long>, IB<C>, IC<C, string>;

    interface IA<T>;

    interface IA<T, R>;

    interface IB<T>;

    interface IC<T, R>;

    class B
    {
        public B(IA<C> c)
        {

        }
    }
}