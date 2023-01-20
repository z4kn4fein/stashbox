using Moq;
using Xunit;

namespace Stashbox.Tests.IssueTests;

public class ComposeByWithInstanceOrInjection
{
    [Fact]
    public void ComposeByWithInstance()
    {
        var mock = new Mock<ICompositionRoot>();
        var container = new StashboxContainer().ComposeBy(mock.Object);
        mock.Verify(m => m.Compose(container), Times.Once);
    }

    [Fact]
    public void ComposeByWithInjection()
    {
        new StashboxContainer()
            .Register<TestDep>()
            .ComposeBy<TestRoot1>();
    }

    [Fact]
    public void ComposeByWithMemberInjection()
    {
        new StashboxContainer(c => c.WithAutoMemberInjection())
            .Register<TestDep>()
            .ComposeBy<TestRoot>();
    }

    [Fact]
    public void ComposeByWithInjectionWithDependencyOverride()
    {
        new StashboxContainer()
            .ComposeBy<TestRoot2>(5);
    }

    class TestDep { }

    class TestRoot : ICompositionRoot
    {
        public TestDep Test { get; set; }

        public void Compose(IStashboxContainer container)
        {
            if (this.Test == null)
                Assert.True(false, "Dependency not resolved");
        }
    }

    class TestRoot1 : ICompositionRoot
    {
        public TestDep Test { get; set; }

        public TestRoot1(TestDep test)
        {
            this.Test = test;
        }

        public void Compose(IStashboxContainer container)
        {
            if (this.Test == null)
                Assert.True(false, "Dependency not resolved");
        }
    }

    class TestRoot2 : ICompositionRoot
    {
        public int Test { get; set; }

        public TestRoot2(int test)
        {
            this.Test = test;
        }

        public void Compose(IStashboxContainer container)
        {
            if (this.Test != 5)
                Assert.True(false, "Dependency not resolved");
        }
    }
}