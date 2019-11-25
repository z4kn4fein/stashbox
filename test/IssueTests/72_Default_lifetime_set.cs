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
            var lifeTime = new ScopedLifetime();
            var mappings = new StashboxContainer(c => c.WithDefaultLifetime(lifeTime))
                .Register<Test>().GetRegistrationMappings();

            var reg = mappings.First();

            Assert.Equal(typeof(Test), reg.Key);
            Assert.IsType<ScopedLifetime>(reg.Value.RegistrationContext.Lifetime);
            Assert.NotSame(lifeTime, reg.Value.RegistrationContext.Lifetime);
        }

        [Fact]
        public void Ensure_Custom_Lifetime_Used_When_Both_Set()
        {
            var mappings = new StashboxContainer(c => c.WithDefaultLifetime(new ScopedLifetime()))
                .Register<Test>(c => c.WithSingletonLifetime()).GetRegistrationMappings();

            var reg = mappings.First();

            Assert.Equal(typeof(Test), reg.Key);
            Assert.IsType<SingletonLifetime>(reg.Value.RegistrationContext.Lifetime);
        }

        class Test { }
    }
}
