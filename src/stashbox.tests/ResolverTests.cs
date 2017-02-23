using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Configuration;
using Stashbox.Exceptions;

namespace Stashbox.Tests
{
    [TestClass]
    public class ResolverTests
    {
        [TestMethod]
        public void ResolverTests_DefaultValue()
        {
            using (var container = new StashboxContainer(config => config.WithOptionalAndDefaultValueInjection()))
            {
                container.RegisterType<Test>();
                var inst = container.Resolve<Test>();

                Assert.AreEqual(inst.I, default(int));
            }
        }

        [TestMethod]
        public void ResolverTests_DefaultValue_WithOptional()
        {
            using (var container = new StashboxContainer(config => config.WithOptionalAndDefaultValueInjection()
            .WithMemberInjectionWithoutAnnotation()))
            {
                container.RegisterType<Test1>();
                var inst = container.Resolve<Test1>();

                Assert.AreEqual(5, inst.I);
            }
        }

        [TestMethod]
        public void ResolverTests_DefaultValue_WithOptional_LateConfig()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test1>();
                container.Configure(config => config
                    .WithOptionalAndDefaultValueInjection());
                var inst = container.Resolve<Test1>();

                Assert.AreEqual(5, inst.I);
            }
        }

        [TestMethod]
        public void ResolverTests_DefaultValue_RefType_WithOptional()
        {
            using (var container = new StashboxContainer(config => config.WithOptionalAndDefaultValueInjection()))
            {
                container.RegisterType<Test2>();
                var inst = container.Resolve<Test2>();

                Assert.AreEqual(inst.I, null);
            }
        }
        
        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ResolverTests_DefaultValue_RefType_WithOutOptional()
        {
            using (var container = new StashboxContainer(config => config.WithOptionalAndDefaultValueInjection()))
            {
                container.RegisterType<Test3>();
                container.Resolve<Test3>();
            }
        }

        [TestMethod]
        public void ResolverTests_DefaultValue_String()
        {
            using (var container = new StashboxContainer(config => config.WithOptionalAndDefaultValueInjection()))
            {
                container.RegisterType<Test4>();
                var inst = container.Resolve<Test4>();

                Assert.AreEqual(inst.I, null);
            }
        }

        [TestMethod]
        public void ResolverTests_UnknownType()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                var inst = container.Resolve<RefDep>();

                Assert.IsNotNull(inst);
            }
        }

        [TestMethod]
        public void ResolverTests_UnknownType_Dependency()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                container.RegisterType<Test3>();
                var inst = container.Resolve<Test3>();

                Assert.IsNotNull(inst.I);
            }
        }

        [TestMethod]
        public void ResolverTests_PreferDefaultValueOverUnknownType()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution().WithOptionalAndDefaultValueInjection()))
            {
                container.RegisterType<Test2>();
                var inst = container.Resolve<Test2>();

                Assert.IsNull(inst.I);
            }
        }

        [TestMethod]
        public void ResolverTests_MemberInject_WithoutAnnotation()
        {
            using (var container = new StashboxContainer(config => config
            .WithMemberInjectionWithoutAnnotation()
            .WithUnknownTypeResolution()))
            {
                container.RegisterType<Test5>();
                var inst = container.Resolve<Test5>();

                Assert.IsNotNull(inst.I);
            }
        }

        [TestMethod]
        public void ResolverTests_MemberInject_WithoutAnnotation_LateConfig()
        {
            using (var container = new StashboxContainer())
            {
                container.RegisterType<Test5>();
                container.Configure(config => config.WithUnknownTypeResolution().WithMemberInjectionWithoutAnnotation());
                var inst = container.Resolve<Test5>();

                Assert.IsNull(inst.I);
            }
        }

        [TestMethod]
        public void ResolverTests_MemberInject_WithAutoMemberInjection()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                container.PrepareType<Test5>().WithAutoMemberInjection().Register();
                var inst = container.Resolve<Test5>();

                Assert.IsNotNull(inst.I);
            }
        }
        
        [TestMethod]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Field()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                container.PrepareType<Test6>().WithName("fail").WithAutoMemberInjection().Register();
                var inst = container.Resolve<Test6>("fail");

                Assert.IsNull(inst.I);

                container.PrepareType<Test6>().WithName("success").WithAutoMemberInjection(Rules.AutoMemberInjection.PrivateFields).Register();
                var inst1 = container.Resolve<Test6>("success");

                Assert.IsNotNull(inst1.I);
            }
        }

        [TestMethod]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_PrivateSetter()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                container.PrepareType<Test7>().WithName("fail").WithAutoMemberInjection().Register();
                var inst = container.Resolve<Test7>("fail");

                Assert.IsNull(inst.I);

                container.PrepareType<Test7>().WithName("success").WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess).Register();
                var inst1 = container.Resolve<Test7>("success");

                Assert.IsNotNull(inst1.I);
            }
        }

        [TestMethod]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Mixed()
        {
            using (var container = new StashboxContainer(config => config.WithUnknownTypeResolution()))
            {
                container.PrepareType<Test8>().WithName("justfield").WithAutoMemberInjection(Rules.AutoMemberInjection.PrivateFields).Register();
                var inst = container.Resolve<Test8>("justfield");

                Assert.IsNotNull(inst.I);
                Assert.IsNull(inst.I1);

                container.PrepareType<Test8>().WithName("justprivatesetter").WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess).Register();
                var inst1 = container.Resolve<Test8>("justprivatesetter");

                Assert.IsNotNull(inst1.I1);
                Assert.IsNull(inst1.I);

                container.PrepareType<Test8>().WithName("mixed")
                    .WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess | Rules.AutoMemberInjection.PrivateFields)
                    .Register();
                var inst2 = container.Resolve<Test8>("mixed");

                Assert.IsNotNull(inst2.I1);
                Assert.IsNotNull(inst2.I);
            }
        }

        [TestMethod]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Mixed_ContainerConfig()
        {
            using (var container = new StashboxContainer(config => config
                .WithUnknownTypeResolution()
                .WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjection.PropertiesWithLimitedAccess | Rules.AutoMemberInjection.PrivateFields)))
            {
                container.PrepareType<Test8>()
                    .WithAutoMemberInjection(Rules.AutoMemberInjection.PropertiesWithLimitedAccess | Rules.AutoMemberInjection.PrivateFields)
                    .Register();
                var inst2 = container.Resolve<Test8>();

                Assert.IsNotNull(inst2.I1);
                Assert.IsNotNull(inst2.I);
            }
        }

        [TestMethod]
        public void ResolverTests_MemberInject_WithAutoMemberInjection_Mixed_PreferRegistrationRuleOverContainerRule()
        {
            using (var container = new StashboxContainer(config => config
                .WithUnknownTypeResolution()
                .WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjection.PropertiesWithLimitedAccess | Rules.AutoMemberInjection.PrivateFields)))
            {
                container.PrepareType<Test8>()
                    .WithAutoMemberInjection(Rules.AutoMemberInjection.PrivateFields)
                    .Register();
                var inst2 = container.Resolve<Test8>();

                Assert.IsNull(inst2.I1);
                Assert.IsNotNull(inst2.I);
            }
        }

        public class Test
        {
            public int I { get; set; }

            public Test(int i)
            {
                this.I = i;
            }
        }

        public class Test1
        {
            public int I { get; private set; }

            public Test1(int i = 5)
            {
                this.I = i;
            }
        }

        public class Test2
        {
            public RefDep I { get; set; }

            public Test2(RefDep i = null)
            {
                this.I = i;
            }
        }

        public class Test3
        {
            public RefDep I { get; set; }

            public Test3(RefDep i)
            {
                this.I = i;
            }
        }

        public class Test4
        {
            public string I { get; set; }

            public Test4(string i)
            {
                this.I = i;
            }
        }

        public class Test5
        {
            public RefDep I { get; set; }
        }

        public class Test6
        {
            public RefDep I { get { return this.i; } }

            private RefDep i = null;
        }

        public class Test7
        {
            public RefDep I { get; private set; }
        }

        public class Test8
        {
            public RefDep I1 { get; private set; }

            public RefDep I { get { return this.i; } }

            private RefDep i = null;
        }

        public class RefDep
        { }
    }
}
