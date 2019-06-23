using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stashbox.Tests.IssueTests
{
    [TestClass]
    public class StaticFactoryFails
    {
        [TestMethod]
        public void Ensure_Static_Factory_Registration_Works()
        {
            var inst = new StashboxContainer().Register<T>(c => c.WithFactory(Factory)).Resolve<T>();

            Assert.IsNotNull(inst);
        }

        [TestMethod]
        public void Ensure_Static_Factory_Registration_With_Resolver_Works()
        {
            var inst = new StashboxContainer().Register<T>(c => c.WithFactory(ResolverFactory)).Resolve<T>();

            Assert.IsNotNull(inst);
        }

        private static T Factory() => new T();

        private static T ResolverFactory(IDependencyResolver resolver) => new T();

        private class T { }
    }
}