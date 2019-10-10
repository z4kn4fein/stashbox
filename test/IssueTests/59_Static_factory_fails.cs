using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class StaticFactoryFails
    {
        [Fact]
        public void Ensure_Static_Factory_Registration_Works()
        {
            var inst = new StashboxContainer().Register<T>(c => c.WithFactory(Factory)).Resolve<T>();

            Assert.NotNull(inst);
        }

        [Fact]
        public void Ensure_Static_Factory_Registration_With_Resolver_Works()
        {
            var inst = new StashboxContainer().Register<T>(c => c.WithFactory(ResolverFactory)).Resolve<T>();

            Assert.NotNull(inst);
        }

        [Fact]
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

            Assert.NotNull(inst);
        }

        [Fact]
        public void Ensure_Static_Factory_Registration_CompiledLambda_Works()
        {
            var param = typeof(IDependencyResolver).AsParameter();
            var inst = new StashboxContainer().Register<T>(c => c
                .WithFactory(this
                    .GetType()
                    .GetMethod("ResolverFactory", BindingFlags.Static | BindingFlags.NonPublic)
                    .CallStaticMethod(param)
                    .AsLambda<Func<IDependencyResolver, T>>(param)
                    .Compile()))
                .Resolve<T>();

            Assert.NotNull(inst);
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