using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class WithFactoryMemberInjectionNotWorkingWithImplementationType
    {
        [TestMethod]
        public void Ensure_MemberInjection_Works_WithFactory()
        {
            var inst = new StashboxContainer(c => c.WithUnknownTypeResolution().WithMemberInjectionWithoutAnnotation())
                .Register<ITest, Test>(ctx => ctx.WithFactory(r => new Test()))
                .Resolve<ITest>();

            Assert.IsNotNull(((Test)inst).Dummy);
        }

        class Dummy { }

        interface ITest { }

        class Test : ITest
        {
            public Dummy Dummy { get; set; }
        }


    }
}
