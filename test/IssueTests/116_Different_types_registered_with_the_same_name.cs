using Xunit;

namespace Stashbox.Tests.IssueTests;

public class DifferentTypesRegisteredWithTheSameName
{
    [Fact]
    public void Ensure_different_types_with_same_name_doesnt_throw_exception()
    {
        var container = new StashboxContainer();
        container.Register<Foo>(options => options.WithName(Name1).WithSingletonLifetime())
                 .Register<Bar>(options => options.WithName(Name2).WithSingletonLifetime().WithFactory(resolver => resolver.Resolve<Foo>(Name1).Bar));

        Assert.NotNull(container.Resolve<Foo>(Name1));
        Assert.NotNull(container.Resolve<Bar>(Name2));
    }

    [Fact]
    public void Ensure_name_is_not_unique_between_types()
    {
        var container = new StashboxContainer();
        container.Register<Foo>(options => options.WithName(Name1).WithSingletonLifetime())
                 .Register<Bar>(options => options.WithName(Name2).WithSingletonLifetime().WithFactory(resolver => resolver.Resolve<Foo>(Name1).Bar));

        Assert.NotNull(container.Resolve<Bar>(Name2));
    }

    const string Name1 = nameof(Name1);
    const string Name2 = nameof(Name1);

    sealed class Foo
    {
        public Bar Bar { get; }

        public Foo()
        {
            this.Bar = new();
        }
    }

    sealed class Bar
    {
    }
}
