using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Configuration;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class IssueWithMemberInjectionwithAttributeButPrivateSetter
    {
        [TestMethod]
        public void Issue_with_Member_Injection_with_Attribute_but_private_setter_private_property()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                var test = container.RegisterType<Test2>(config => config.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess))
                    .Resolve<Test2>();

                Assert.IsNotNull(test.Test1Prop2, "test.Test1Prop2 != null");
                Assert.IsNotNull(test.Test1Prop, "test.Test1Prop != null");
            }
        }

        [TestMethod]
        public void Issue_with_Member_Injection_with_Attribute_but_private_setter_private_field()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                var test = container.RegisterType<Test4>(config => config.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields))
                    .Resolve<Test4>();

                Assert.IsNotNull(test.Test1Prop2, "test.Test1Prop2 != null");
                Assert.IsNotNull(test.Test1Prop, "test.Test1Prop != null");
            }
        }

        [TestMethod]
        public void Issue_with_Member_Injection_with_Attribute_but_private_setter_public_property()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                var test = container.RegisterType<Test6>(config => config.WithAutoMemberInjection())
                    .Resolve<Test6>();

                Assert.IsNotNull(test.Test1Prop2, "test.Test1Prop2 != null");
                Assert.IsNotNull(test.Test1Prop, "test.Test1Prop != null");
            }
        }

        class Test
        { }

        class Test1
        {
            public Test Test1Prop { get; private set; }
        }

        class Test2 : Test1
        {
            public Test Test1Prop2 { get; private set; }
        }

        class Test3
        {
            private Test test1 = null;

            public Test Test1Prop => this.test1;
        }

        class Test4 : Test3
        {
            private Test test1 = null;

            public Test Test1Prop2 => this.test1;
        }

        class Test5
        {
            public Test Test1Prop { get; set; }
        }

        class Test6 : Test5
        {
            public Test Test1Prop2 { get; set; }
        }
    }
}
