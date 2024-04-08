using Xunit;

namespace Stashbox.Tests.IssueTests;

public class ResolvingWithCustomParameterValues
{
    [Fact]
    public void Ensure_Dependency_Override_Works()
    {
        var name = "PAS.LEV";
        using var container = new StashboxContainer()
            .Register<IVariantSubproduct, StainlessSteelPlate>(name);

        var subProduct = new Subproduct();
        var variant = container.Resolve<IVariantSubproduct>(name, [subProduct]);
        Assert.NotNull(variant);
        Assert.Same(subProduct, variant.Subproduct);
    }

    interface IVariantSubproduct
    {
        ISubproduct Subproduct { get; }
    }

    interface ISubproduct;

    class Subproduct : ISubproduct;

    class VariantSubproduct : IVariantSubproduct
    {
        public ISubproduct Subproduct { get; }

        protected VariantSubproduct(ISubproduct subproduct)
        {
            Subproduct = subproduct;
        }
    }

    class StainlessSteelPlate : VariantSubproduct
    {
        public StainlessSteelPlate(ISubproduct subproduct) : base(subproduct) { }
    }
}