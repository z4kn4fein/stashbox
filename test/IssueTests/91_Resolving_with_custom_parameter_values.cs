using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class ResolvingWithCustomParameterValues
    {
        [Fact]
        public void Ensure_Dependency_Override_Works()
        {
            var name = "PAS.LEV";
            using var container = new StashboxContainer()
                .Register<IVariantSubproduct, StainlessSteelPlate>(name);

            Assert.NotNull(container.Resolve<IVariantSubproduct>(name, false, new[] { new Subproduct() }));
        }

        interface IVariantSubproduct
        { }

        interface ISubproduct
        { }

        class Subproduct : ISubproduct
        { }

        class VariantSubproduct : IVariantSubproduct
        {
            protected VariantSubproduct(ISubproduct subproduct) { }
        }

        class StainlessSteelPlate : VariantSubproduct
        {
            public StainlessSteelPlate(ISubproduct subproduct) : base(subproduct) { }
        }
    }
}
