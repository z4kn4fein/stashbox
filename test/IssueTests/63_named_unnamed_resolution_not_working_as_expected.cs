using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Exceptions;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class NamedUnnamedResolutionNotWorkingAsExpected
    {
        [TestMethod]
        public void Ensures_Named_Dependency_Selected_When_Convention_Enabled()
        {
            var inst = new StashboxContainer(c => c.TreatParameterAndMemberNameAsDependencyName())
                .Register<ITest, Test1>(c => c.WithName("t1"))
                .Register<ITest, Test2>(c => c.WithName("t2"))
                .Register<Test3>()
                .Resolve<Test3>();

            Assert.IsInstanceOfType(inst.T1, typeof(Test1));
            Assert.IsInstanceOfType(inst.T2, typeof(Test2));
        }

        [TestMethod]
        public void Ensures_Named_Dependency_Selected_When_Convention_Enabled_InjectionMembers()
        {
            var inst = new StashboxContainer(c => c.TreatParameterAndMemberNameAsDependencyName())
                .Register<ITest, Test1>(c => c.WithName("T1"))
                .Register<ITest, Test2>(c => c.WithName("T2"))
                .Register<Test4>(c => c.WithAutoMemberInjection())
                .Resolve<Test4>();

            Assert.IsInstanceOfType(inst.T1, typeof(Test1));
            Assert.IsInstanceOfType(inst.T2, typeof(Test2));
        }

        [TestMethod]
        public void Ensures_Named_Dependency_Selected_When_Convention_Enabled_InjectionMethod()
        {
            var inst = new StashboxContainer(c => c.TreatParameterAndMemberNameAsDependencyName())
                .Register<ITest, Test1>(c => c.WithName("t1"))
                .Register<ITest, Test2>(c => c.WithName("t2"))
                .Register<Test5>()
                .Resolve<Test5>();

            Assert.IsInstanceOfType(inst.T1, typeof(Test1));
            Assert.IsInstanceOfType(inst.T2, typeof(Test2));
        }

        [TestMethod]
        public void Ensures_UnNamed_Dependency_Selected_When_Named_Not_Available()
        {
            var container = new StashboxContainer(c => c.WithNamedDependencyResolutionForUnNamedRequests())
                .Register<ITest, Test1>(c => c.WithName("t1").WithSingletonLifetime());

            var inst = container.Resolve<ITest>("t1");
            var inst2 = container.Resolve<ITest>();

            Assert.IsNotNull(inst);
            Assert.IsNotNull(inst2);
            Assert.AreEqual(inst, inst2);
        }

        [TestMethod]
        public void Ensures_UnNamed_Dependency_Selected_When_Convention_Enabled_But_Named_Preferred()
        {
            var container = new StashboxContainer(c => c
                    .TreatParameterAndMemberNameAsDependencyName()
                    .WithNamedDependencyResolutionForUnNamedRequests())
                .Register<Test3>()
                .Register<ITest, Test1>("t1");

            var inst = container.Resolve<Test3>();

            Assert.IsInstanceOfType(inst.T1, typeof(Test1));
            Assert.IsInstanceOfType(inst.T2, typeof(Test1));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void Ensures_Dont_Resolve_Named_When_Not_Enabled()
        {
            var container = new StashboxContainer(c => c
                    .TreatParameterAndMemberNameAsDependencyName())
                .Register<Test3>()
                .Register<ITest, Test1>("t1");

            container.Resolve<Test3>();
        }

        [TestMethod]
        public void Ensures_UnNamed_Dependency_Selected_When_Named_Not_Available_With_Treating_Param_Names_As_Dependency_Names()
        {
            var container = new StashboxContainer(c => c
                    .TreatParameterAndMemberNameAsDependencyName())
                .Register<Test3>()
                .Register<ITest, Test1>("t1")
                .Register<ITest, Test2>();

            var inst = container.Resolve<Test3>();

            Assert.IsInstanceOfType(inst.T1, typeof(Test1));
            Assert.IsInstanceOfType(inst.T2, typeof(Test2));
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
