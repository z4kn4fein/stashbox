using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Entity;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class MakeInjectionParameterConfigurationMoreFluent
    {
        [TestMethod]
        public void MakeInjectionParameterConfigurationMoreFluent_Mixed()
        {
            var instance = new StashboxContainer()
                .Register<Test1>(c => c.WithAutoMemberInjection()
                    .WithInjectionParameter(nameof(Test1.TestString), "sample")
                    .WithInjectionParameters(new InjectionParameter { Name = nameof(Test1.TestInt), Value = 5 }))
                .Resolve<Test1>();

            Assert.AreEqual("sample", instance.TestString);
            Assert.AreEqual(5, instance.TestInt);
        }

        class Test1
        {
            public string TestString { get; set; }

            public int TestInt { get; set; }
        }
    }
}
