using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            using (var container = new StashboxContainer(config => config.WithOptionalAndDefaultValueInjection()))
            {
                container.RegisterType<Test1>();
                var inst = container.Resolve<Test1>();

                Assert.AreEqual(inst.I, 5);
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
            public int I { get; set; }

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

        public class RefDep
        { }
    }
}
