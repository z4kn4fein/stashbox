using Stashbox.Lifetime;
using System.Linq;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class DefaultLifetimeSet
    {
        [Fact]
        public void Ensure_Default_Lifetime_Used_When_Custom_Not_Set()
        {
            var mappings = new StashboxContainer(c => c.WithDefaultLifetime(Lifetimes.Scoped))
                .Register<Test>().GetRegistrationMappings();

            var reg = mappings.First();

            Assert.Equal(typeof(Test), reg.Key);
            Assert.Same(Lifetimes.Scoped, reg.Value.RegistrationContext.Lifetime);
        }

        [Fact]
        public void Ensure_Custom_Lifetime_Used_When_Both_Set()
        {
            var mappings = new StashboxContainer(c => c.WithDefaultLifetime(Lifetimes.Scoped))
                .Register<Test>(c => c.WithSingletonLifetime()).GetRegistrationMappings();

            var reg = mappings.First();

            Assert.Equal(typeof(Test), reg.Key);
            Assert.Same(Lifetimes.Singleton, reg.Value.RegistrationContext.Lifetime);
        }

        [Fact]
        public void Ensure_Transient_Lifetime_Used_By_Default()
        {
            var mappings = new StashboxContainer()
                .Register<Test>().GetRegistrationMappings();

            var reg = mappings.First();

            Assert.Equal(typeof(Test), reg.Key);
            Assert.Same(Lifetimes.Transient, reg.Value.RegistrationContext.Lifetime);
        }

        class Test { }
    }
}
