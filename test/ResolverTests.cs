using Stashbox.Configuration;
using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests
{

    public class ResolverTests
    {
        [Fact]
        public void ResolverTests_DefaultValue()
        {
            using var container = new StashboxContainer(config => config.WithDefaultValueInjection());
            container.Register<Test>();
            var inst = container.Resolve<Test>();

            Assert.Equal(default, inst.I);
        }

        [Fact]
        public void ResolverTests_DefaultValue_WithOptional()
        {
            using var container = new StashboxContainer();
            container.Register<Test1>();
            var inst = container.Resolve<Test1>();

            Assert.Equal(5, inst.I);
        }

        [Fact]
        public void ResolverTests_DefaultValue_WithOptional_LateConfig()
        {
            using var container = new StashboxContainer();
            container.Register<Test1>();
            container.Configure(config => config
                .WithDefaultValueInjection());
            var inst = container.Resolve<Test1>();

            Assert.Equal(5, inst.I);
        }

        [Fact]
        public void ResolverTests_DefaultValue_RefType_WithOptional()
        {
            using var container = new StashboxContainer(config => config.WithDefaultValueInjection());
            container.Register<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.Null(inst.I);
        }

        [Fact]
        public void ResolverTests_DefaultValue_RefType_WithOutOptional()
        {
            using var container = new StashboxContainer(config => config.WithDefaultValueInjection());
            container.Register<Test3>();
            Assert.Throws<ResolutionFailedException>(() => container.Resolve<Test3>());
        }

        [Fact]
        public void ResolverTests_DefaultValue_RefType_WithOutOptional_AllowNull()
        {
            using var container = new StashboxContainer(config => config.WithDefaultValueInjection());
            container.Register<Test3>();
            var result = container.Resolve<Test3>(nullResultAllowed: true);

            Assert.Null(result);
        }

        [Fact]
        public void ResolverTests_DefaultValue_String()
        {
            using var container = new StashboxContainer(config => config.WithDefaultValueInjection());
            container.Register<Test4>();
            var inst = container.Resolve<Test4>();

            Assert.Null(inst.I);
        }

        [Fact]
        public void ResolverTests_DefaultValue_Null()
        {
            using var container = new StashboxContainer();
            container.Register<Test4>();
            var inst = container.Resolve<Test4>(nullResultAllowed: true);

            Assert.Null(inst);
        }

        [Fact]
        public void ResolverTests_DefaultValue_Nullable()
        {
            using var container = new StashboxContainer(config => config.WithDefaultValueInjection());
            container.Register<Test9>();
            var inst = container.Resolve<Test9>();

            Assert.Null(inst.I);
        }

        [Fact]
        public void ResolverTests_UnknownType()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            var inst = container.Resolve<RefDep>();

            Assert.NotNull(inst);
        }

        [Fact]
        public void ResolverTests_UnknownType_Dependency()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            container.Register<Test3>();
            var inst = container.Resolve<Test3>();

            Assert.NotNull(inst.I);
        }

        [Fact]
        public void ResolverTests_PreferDefaultValueOverUnknownType()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution().WithDefaultValueInjection());
            container.Register<Test2>();
            var inst = container.Resolve<Test2>();

            Assert.Null(inst.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithoutAnnotation()
        {
            using var container = new StashboxContainer(config => config
            .WithMemberInjectionWithoutAnnotation()
            .WithUnknownTypeResolution());
            container.Register<Test5>();
            var inst = container.Resolve<Test5>();

            Assert.NotNull(inst.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithoutAnnotation_LateConfig()
        {
            using var container = new StashboxContainer();
            container.Register<Test5>();
            container.Configure(config => config.WithUnknownTypeResolution().WithMemberInjectionWithoutAnnotation());
            var inst = container.Resolve<Test5>();

            Assert.NotNull(inst.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithAutoMemberInjection()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            container.Register<Test5>(context => context.WithAutoMemberInjection());
            var inst = container.Resolve<Test5>();

            Assert.NotNull(inst.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Field()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            container.Register<Test6>(context => context.WithName("fail").WithAutoMemberInjection());
            var inst = container.Resolve<Test6>("fail");

            Assert.Null(inst.I);

            container.Register<Test6>(context => context.WithName("success").WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields));
            var inst1 = container.Resolve<Test6>("success");

            Assert.NotNull(inst1.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_PrivateSetter()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            container.Register<Test7>(context => context.WithName("fail").WithAutoMemberInjection());
            var inst = container.Resolve<Test7>("fail");

            Assert.Null(inst.I);

            container.Register<Test7>(context => context.WithName("success").WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));
            var inst1 = container.Resolve<Test7>("success");

            Assert.NotNull(inst1.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Mixed()
        {
            using var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            container.Register<Test8>(context => context.WithName("justfield").WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields));
            var inst = container.Resolve<Test8>("justfield");

            Assert.NotNull(inst.I);
            Assert.Null(inst.I1);

            container.Register<Test8>(context => context.WithName("justprivatesetter").WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));
            var inst1 = container.Resolve<Test8>("justprivatesetter");

            Assert.NotNull(inst1.I1);
            Assert.Null(inst1.I);

            container.Register<Test8>(context => context.WithName("mixed")
                .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess | Rules.AutoMemberInjectionRules.PrivateFields));
            var inst2 = container.Resolve<Test8>("mixed");

            Assert.NotNull(inst2.I1);
            Assert.NotNull(inst2.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Mixed_ContainerConfig()
        {
            using var container = new StashboxContainer(config => config
                .WithUnknownTypeResolution()
                .WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess | Rules.AutoMemberInjectionRules.PrivateFields));
            container.Register<Test8>(context => context.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess | Rules.AutoMemberInjectionRules.PrivateFields));
            var inst2 = container.Resolve<Test8>();

            Assert.NotNull(inst2.I1);
            Assert.NotNull(inst2.I);
        }

        [Fact]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Mixed_PreferRegistrationRuleOverContainerRule()
        {
            using var container = new StashboxContainer(config => config
                .WithUnknownTypeResolution()
                .WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess | Rules.AutoMemberInjectionRules.PrivateFields));
            container.Register<Test8>(context => context.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields));
            var inst2 = container.Resolve<Test8>();

            Assert.Null(inst2.I1);
            Assert.NotNull(inst2.I);
        }

        class Test
        {
            public int I { get; set; }

            public Test(int i)
            {
                this.I = i;
            }
        }

        class Test1
        {
            public int I { get; private set; }

            public Test1(int i = 5)
            {
                this.I = i;
            }
        }

        class Test2
        {
            public RefDep I { get; set; }

            public Test2(RefDep i = null)
            {
                this.I = i;
            }
        }

        class Test3
        {
            public RefDep I { get; set; }

            public Test3(RefDep i)
            {
                this.I = i;
            }
        }

        class Test4
        {
            public string I { get; set; }

            public Test4(string i)
            {
                this.I = i;
            }
        }

        class Test5
        {
            public RefDep I { get; set; }
        }

        class Test6
        {
            public RefDep I => this.i;

            private RefDep i = null;
        }

        class Test7
        {
            public RefDep I { get; private set; }
        }

        class Test8
        {
            public RefDep I1 { get; private set; }

            public RefDep I => this.i;

            private RefDep i = null;
        }

        class Test9
        {
            public Test9(int? i = null)
            {
                this.I = i;
            }

            public int? I { get; private set; }
        }

        class RefDep
        { }
    }
}
