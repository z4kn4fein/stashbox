using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Tests
{
    public class ScopeTests
    {
        [Fact]
        public void GetOrAdd_Ensure_Evaluator_DoesNotThrow()
        {
            for (int i = 0; i < 5000; i++)
            {
                using var scope = (IResolutionScope)new StashboxContainer().BeginScope();
                Parallel.For(0, 50, i =>
                {
                    var inst = scope.GetOrAddScopedObject(1, (_, _) => new object(), null, typeof(object));
                    Assert.NotNull(inst);
                });
            }
        }

        [Fact]
        public void Enusre_Put_Instance_Creates_New_Cache()
        {
            using var container = new StashboxContainer();
            using var scope = container.BeginScope();

            scope.PutInstanceInScope(new A());
            scope.PutInstanceInScope(new B());

            var cache = scope.GetDelegateCacheEntries();
            Assert.Empty(cache);

            scope.Resolve<A>();
            scope.Resolve<B>();

            cache = scope.GetDelegateCacheEntries();
            Assert.Equal(2, cache.ToArray().Length);

            scope.Resolve<A>();
            scope.Resolve<B>();

            cache = scope.GetDelegateCacheEntries();
            Assert.Equal(2, cache.ToArray().Length);

            scope.PutInstanceInScope(new C());

            cache = scope.GetDelegateCacheEntries();
            Assert.Empty(cache);

            scope.Resolve<A>();
            scope.Resolve<A>();
            scope.Resolve<B>();
            scope.Resolve<B>();
            scope.Resolve<C>();
            scope.Resolve<C>();

            cache = scope.GetDelegateCacheEntries();
            Assert.Equal(3, cache.ToArray().Length);
        }

        [Fact]
        public void Enusre_Put_Instance_Creates_New_Cache_ResolveOrDefault()
        {
            using var container = new StashboxContainer();
            using var scope = container.BeginScope();

            scope.PutInstanceInScope(new A());
            scope.PutInstanceInScope(new B());

            var cache = scope.GetDelegateCacheEntries();
            Assert.Empty(cache);

            scope.ResolveOrDefault<A>();
            scope.ResolveOrDefault<B>();

            cache = scope.GetDelegateCacheEntries();
            Assert.Equal(2, cache.ToArray().Length);

            scope.ResolveOrDefault<A>();
            scope.ResolveOrDefault<B>();

            cache = scope.GetDelegateCacheEntries();
            Assert.Equal(2, cache.ToArray().Length);

            scope.PutInstanceInScope(new C());

            cache = scope.GetDelegateCacheEntries();
            Assert.Empty(cache);

            scope.ResolveOrDefault<A>();
            scope.ResolveOrDefault<A>();
            scope.ResolveOrDefault<B>();
            scope.ResolveOrDefault<B>();
            scope.ResolveOrDefault<C>();
            scope.ResolveOrDefault<C>();

            cache = scope.GetDelegateCacheEntries();
            Assert.Equal(3, cache.ToArray().Length);
        }

        [Fact]
        public void Enusre_Dependency_Overrides_Disables_Cache()
        {
            using var container = new StashboxContainer().Register<A>();
            using var scope = container.BeginScope();
            var cache = scope.GetDelegateCacheEntries();

            Assert.Empty(cache);

            scope.Resolve<A>();
            cache = scope.GetDelegateCacheEntries();

            Assert.Single(cache);
        }

        [Fact]
        public void Enusre_Dependency_Overrides_Disables_Cache_ResolveOrDefault()
        {
            using var container = new StashboxContainer().Register<A>();
            using var scope = container.BeginScope();
            scope.ResolveOrDefault<A>(dependencyOverrides: new[] { new A() });

            var cache = scope.GetDelegateCacheEntries();
            Assert.Empty(cache);

            scope.ResolveOrDefault<A>();
            cache = scope.GetDelegateCacheEntries();

            Assert.Single(cache);
        }

        private class A { }
        private class B { }
        private class C { }
    }
}
