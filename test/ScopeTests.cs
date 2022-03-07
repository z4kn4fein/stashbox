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

            var scopeType = scope.GetType();
            var cacheField = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var delegateCache = cacheField.GetValue(scope);
            var delegatesField = delegateCache.GetType().GetField("ServiceDelegates");

            scope.PutInstanceInScope(new A());
            scope.PutInstanceInScope(new B());

            scope.Resolve<A>();
            scope.Resolve<B>();

            var cache1 = cacheField.GetValue(scope);
            var serviceFactories1 = delegatesField.GetValue(cache1);
            var enumerator1 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)serviceFactories1
                .GetType().GetMethod("Walk").Invoke(serviceFactories1, Type.EmptyTypes);

            Assert.NotEqual(delegateCache, cache1);
            Assert.Equal(2, enumerator1.ToArray().Length);

            scope.PutInstanceInScope(new C());

            var cache2 = cacheField.GetValue(scope);
            var serviceFactories2 = delegatesField.GetValue(cache2);
            var enumerator2 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)serviceFactories2
                .GetType().GetMethod("Walk").Invoke(serviceFactories2, Type.EmptyTypes);

            Assert.NotEqual(cache1, cache2);
            Assert.Empty(enumerator2.ToArray());

            scope.Resolve<A>();
            scope.Resolve<A>();
            scope.Resolve<B>();
            scope.Resolve<B>();
            scope.Resolve<C>();
            scope.Resolve<C>();

            var cache3 = cacheField.GetValue(scope);
            var serviceFactories3 = delegatesField.GetValue(cache3);
            var enumerator3 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)serviceFactories3
                .GetType().GetMethod("Walk").Invoke(serviceFactories3, Type.EmptyTypes);

            Assert.Equal(cache2, cache3);
            Assert.Equal(3, enumerator3.ToArray().Length);
        }

        [Fact]
        public void Enusre_Put_Instance_Creates_New_Cache_ResolveOrDefault()
        {
            using var container = new StashboxContainer();

            using var scope = container.BeginScope();

            var scopeType = scope.GetType();
            var cacheField = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var delegateCache = cacheField.GetValue(scope);
            var delegatesField = delegateCache.GetType().GetField("ServiceDelegates");

            scope.PutInstanceInScope(new A());
            scope.PutInstanceInScope(new B());

            scope.ResolveOrDefault<A>();
            scope.ResolveOrDefault<B>();

            var cache1 = cacheField.GetValue(scope);
            var serviceFactories1 = delegatesField.GetValue(cache1);
            var enumerator1 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)serviceFactories1
                .GetType().GetMethod("Walk").Invoke(serviceFactories1, Type.EmptyTypes);

            Assert.NotEqual(delegateCache, cache1);
            Assert.Equal(2, enumerator1.ToArray().Length);

            scope.PutInstanceInScope(new C());

            var cache2 = cacheField.GetValue(scope);
            var serviceFactories2 = delegatesField.GetValue(cache2);
            var enumerator2 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)serviceFactories2
                .GetType().GetMethod("Walk").Invoke(serviceFactories2, Type.EmptyTypes);

            Assert.NotEqual(cache1, cache2);
            Assert.Empty(enumerator2.ToArray());

            scope.ResolveOrDefault<A>();
            scope.ResolveOrDefault<A>();
            scope.ResolveOrDefault<B>();
            scope.ResolveOrDefault<B>();
            scope.ResolveOrDefault<C>();
            scope.ResolveOrDefault<C>();

            var cache3 = cacheField.GetValue(scope);
            var serviceFactories3 = delegatesField.GetValue(cache3);
            var enumerator3 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)serviceFactories3
                .GetType().GetMethod("Walk").Invoke(serviceFactories3, Type.EmptyTypes);

            Assert.Equal(cache2, cache3);
            Assert.Equal(3, enumerator3.ToArray().Length);
        }

        [Fact]
        public void Enusre_Dependency_Overrides_Disables_Cache()
        {
            using var container = new StashboxContainer().Register<A>();

            using var scope = container.BeginScope();

            var scopeType = scope.GetType();
            var cacheField = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var delegateCache = cacheField.GetValue(scope);
            var delegatesField = delegateCache.GetType().GetField("ServiceDelegates");

            scope.Resolve<A>(dependencyOverrides: new[] { new A() });

            var serviceFactories1 = delegatesField.GetValue(delegateCache);
            var walkMethod = serviceFactories1.GetType().GetMethod("Walk");
            var enumerator1 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)walkMethod
                .Invoke(serviceFactories1, Type.EmptyTypes);

            Assert.Empty(enumerator1.ToArray());

            scope.Resolve<A>();

            var cache = cacheField.GetValue(scope);
            var serviceFactories2 = delegatesField.GetValue(cache);
            var enumerator2 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)walkMethod
                .Invoke(serviceFactories2, Type.EmptyTypes);

            Assert.Single(enumerator2.ToArray());
        }

        [Fact]
        public void Enusre_Dependency_Overrides_Disables_Cache_ResolveOrDefault()
        {
            using var container = new StashboxContainer().Register<A>();

            using var scope = container.BeginScope();

            var scopeType = scope.GetType();
            var cacheField = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var delegateCache = cacheField.GetValue(scope);
            var delegatesField = delegateCache.GetType().GetField("ServiceDelegates");

            scope.ResolveOrDefault<A>(dependencyOverrides: new[] { new A() });

            var serviceFactories1 = delegatesField.GetValue(delegateCache);
            var walkMethod = serviceFactories1.GetType().GetMethod("Walk");
            var enumerator1 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)walkMethod
                .Invoke(serviceFactories1, Type.EmptyTypes);

            Assert.Empty(enumerator1.ToArray());

            scope.ResolveOrDefault<A>();

            var cache = cacheField.GetValue(scope);
            var serviceFactories2 = delegatesField.GetValue(cache);
            var enumerator2 = (IEnumerable<ReadOnlyKeyValue<object, Func<IResolutionScope, IRequestContext, object>>>)walkMethod
                .Invoke(serviceFactories2, Type.EmptyTypes);

            Assert.Single(enumerator2.ToArray());
        }

        private class A { }
        private class B { }
        private class C { }
    }
}
