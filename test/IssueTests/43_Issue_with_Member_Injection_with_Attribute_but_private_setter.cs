using Stashbox.Configuration;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class IssueWithMemberInjectionwithAttributeButPrivateSetter
    {
        [Fact]
        public void Issue_with_Member_Injection_with_Attribute_but_private_setter_private_property()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            var test = container.Register<Test2>(config => config.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess))
.Resolve<Test2>();

            Assert.True(test.Test1Prop2 != null, "test.Test1Prop2 != null");
            Assert.True(test.Test1Prop != null, "test.Test1Prop != null");
        }

        [Fact]
        public void Issue_with_Member_Injection_with_Attribute_but_private_setter_private_field()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            var test = container.Register<Test4>(config => config.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields))
.Resolve<Test4>();

            Assert.True(test.Test1Prop2 != null, "test.Test1Prop2 != null");
            Assert.True(test.Test1Prop != null, "test.Test1Prop != null");
        }

        [Fact]
        public void Issue_with_Member_Injection_with_Attribute_but_private_setter_public_property()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            var test = container.Register<Test6>(config => config.WithAutoMemberInjection())
.Resolve<Test6>();

            Assert.True(test.Test1Prop2 != null, "test.Test1Prop2 != null");
            Assert.True(test.Test1Prop != null, "test.Test1Prop != null");
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
