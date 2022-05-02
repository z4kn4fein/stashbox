using Stashbox.Attributes;
using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class NamedUnnamedResolutionNotWorkingAsExpected
    {
        [Fact]
        public void Ensures_Named_Dependency_Selected_When_Convention_Enabled()
        {
            var inst = new StashboxContainer(c => c.TreatParameterAndMemberNameAsDependencyName())
                .Register<ITest, Test1>(c => c.WithName("t1"))
                .Register<ITest, Test2>(c => c.WithName("t2"))
                .Register<Test3>()
                .Resolve<Test3>();

            Assert.IsType<Test1>(inst.T1);
            Assert.IsType<Test2>(inst.T2);
        }

        [Fact]
        public void Ensures_Named_Dependency_Selected_When_Convention_Enabled_InjectionMembers()
        {
            var inst = new StashboxContainer(c => c.TreatParameterAndMemberNameAsDependencyName())
                .Register<ITest, Test1>(c => c.WithName("T1"))
                .Register<ITest, Test2>(c => c.WithName("T2"))
                .Register<Test4>(c => c.WithAutoMemberInjection())
                .Resolve<Test4>();

            Assert.IsType<Test1>(inst.T1);
            Assert.IsType<Test2>(inst.T2);
        }

        [Fact]
        public void Ensures_Named_Dependency_Selected_When_Convention_Enabled_InjectionMethod()
        {
            var inst = new StashboxContainer(c => c.TreatParameterAndMemberNameAsDependencyName())
                .Register<ITest, Test1>(c => c.WithName("t1"))
                .Register<ITest, Test2>(c => c.WithName("t2"))
                .Register<Test5>()
                .Resolve<Test5>();

            Assert.IsType<Test1>(inst.T1);
            Assert.IsType<Test2>(inst.T2);
        }

        [Fact]
        public void Ensures_UnNamed_Dependency_Selected_When_Named_Not_Available()
        {
            var container = new StashboxContainer(c => c.WithNamedDependencyResolutionForUnNamedRequests())
                .Register<ITest, Test1>(c => c.WithName("t1").WithSingletonLifetime());

            var inst = container.Resolve<ITest>("t1");
            var inst2 = container.Resolve<ITest>();

            Assert.NotNull(inst);
            Assert.NotNull(inst2);
            Assert.Equal(inst, inst2);
        }

        [Fact]
        public void Ensures_UnNamed_Dependency_Selected_When_Convention_Enabled_But_Named_Preferred()
        {
            var container = new StashboxContainer(c => c
                    .TreatParameterAndMemberNameAsDependencyName()
                    .WithNamedDependencyResolutionForUnNamedRequests())
                .Register<Test3>()
                .Register<ITest, Test1>("t1")
                .Register<ITest, Test2>();

            var inst = container.Resolve<Test3>();

            Assert.IsType<Test1>(inst.T1);
            Assert.IsType<Test2>(inst.T2);
        }

        [Fact]
        public void Ensures_Dont_Resolve_Named_When_Not_Enabled()
        {
            var container = new StashboxContainer(c => c
                    .TreatParameterAndMemberNameAsDependencyName())
                .Register<Test3>()
                .Register<ITest, Test1>("t1");

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<Test3>());
        }

        [Fact]
        public void Ensures_UnNamed_Dependency_Selected_When_Named_Not_Available_With_Treating_Param_Names_As_Dependency_Names()
        {
            var container = new StashboxContainer(c => c
                    .TreatParameterAndMemberNameAsDependencyName())
                .Register<Test3>()
                .Register<ITest, Test1>("t1")
                .Register<ITest, Test2>();

            var inst = container.Resolve<Test3>();

            Assert.IsType<Test1>(inst.T1);
            Assert.IsType<Test2>(inst.T2);
        }

        interface ITest { }

        class Test1 : ITest { }

        class Test2 : ITest { }

        class Test3
        {
            public ITest T1 { get; }
            public ITest T2 { get; }

            public Test3(ITest t1, ITest t2)
            {
                T1 = t1;
                T2 = t2;
            }
        }

        class Test4
        {
            public ITest T1 { get; set; }
            public ITest T2 { get; set; }
        }

        class Test5
        {
            public ITest T1 { get; set; }
            public ITest T2 { get; set; }

            [InjectionMethod]
            public void Init(ITest t1, ITest t2)
            {
                T1 = t1;
                T2 = t2;
            }
        }
    }
}
