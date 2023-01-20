using Stashbox.Lifetime;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class DefinesScopeDoesNotWorkCorrectly
{
    [Fact]
    public void DefinesScope_does_not_work_correctly()
    {
        using var container = new StashboxContainer()
            .Register<ProductCache>(c => c
                .DefinesScope()
                .WithLifetime(Lifetimes.Singleton)
                .WithoutDisposalTracking())
            .Register<ProductService>()
            .RegisterScoped<ProductRepository>();

        var inst = container.Resolve<ProductService>();

        Assert.NotSame(inst.Repo, inst.Cache.Repo);
    }
}

class ProductCache
{
    public ProductRepository Repo { get; }

    public ProductCache(ProductRepository repo)
    {
        Repo = repo;
    }

}

class ProductService
{
    public ProductRepository Repo { get; }
    public ProductCache Cache { get; }

    public ProductService(ProductRepository repo, ProductCache cache)
    {
        Repo = repo;
        Cache = cache;
    }
}

class ProductRepository
{ }