using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class IssueTests
    {
        public void Mixture_of_named_and_non_named_registrations_result_in_the_wrong_type_resolved()
        {
            var sb = new StashboxContainer();

            sb.RegisterSingleton(typeof(ITest), typeof(Test));
            sb.RegisterSingleton(typeof(ITest), typeof(Test1), "Test2");
            sb.RegisterSingleton(typeof(ITest), typeof(Test2), "Test3");
            var test = sb.Resolve<ITest>();

            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test, typeof(Test));
        }

        interface ITest
        { }

        class Test : ITest1
        { }

        class Test1 : ITest1
        { }

        class Test2 : ITest1
        { }
    }
}
