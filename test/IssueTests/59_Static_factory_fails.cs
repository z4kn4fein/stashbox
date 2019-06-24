using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Reflection;

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

        [TestMethod]
        public void Ensure_Static_Factory_Registration_WithProperty_Works()
        {
            var prop = typeof(St)
                .GetProperty("Tp");
            var inst = new StashboxContainer().Register<T>(c => c
                .WithFactory(prop
                    .Access(null)
                    .AsLambda<Func<T>>()
                    .Compile()))
                .Resolve<T>();

            Assert.IsNotNull(inst);
        }

        private static T Factory() => new T();

        private static T ResolverFactory(IDependencyResolver resolver) => new T();

        private class T { }

        private static class St
        {
            public static T Tp => new T();
        }
    }
}