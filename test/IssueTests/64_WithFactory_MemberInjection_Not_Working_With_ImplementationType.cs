using Xunit;

namespace Stashbox.Tests.IssueTests
{
    
    public class WithFactoryMemberInjectionNotWorkingWithImplementationType
    {
        [Fact]
        public void Ensure_MemberInjection_Works_WithFactory()
        {
            var inst = new StashboxContainer(c => c.WithUnknownTypeResolution().WithAutoMemberInjection())
                .Register<ITest, Test>(ctx => ctx.WithFactory(r => new Test()))
                .Resolve<ITest>();

            Assert.NotNull(((Test)inst).Dummy);
        }

        class Dummy { }

        interface ITest { }

        class Test : ITest
        {
            public Dummy Dummy { get; set; }
        }


    }
}
