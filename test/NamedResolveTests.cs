using Stashbox.Exceptions;
using System;
using System.Collections;
using System.Reflection;
using System.Text;
using Xunit;

namespace Stashbox.Tests
{
    public class NamedResolveTests
    {
        [Fact]
        public void NamedResolveTests_Resolve()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>("A")
                .Register<IA, B>("B");

            Assert.IsType<A>(container.Resolve<IA>("A"));
            Assert.IsType<B>(container.Resolve<IA>("B"));
        }

        [Fact]
        public void NamedResolveTests_Resolve_Throw_On_Same_Name()
        {
            Assert.Throws<ServiceAlreadyRegisteredException>(
                () => new StashboxContainer(c => c.WithRegistrationBehavior(Configuration.Rules.RegistrationBehavior.ThrowException))
                .Register<IA, A>("A")
                .Register<IA, A>("A"));
        }

        [Fact]
        public void NamedResolveTests_Resolve_Throw_On_Same_Name_Equality_By_Value()
        {
            var key = new StringBuilder("A");
            Assert.Throws<ServiceAlreadyRegisteredException>(
                () => new StashboxContainer(c => c.WithRegistrationBehavior(Configuration.Rules.RegistrationBehavior.ThrowException))
                .Register<IA, A>("A")
                .Register<IA, A>(key.ToString()));
        }

        [Fact]
        public void NamedResolveTests_Ensure_Delegate_Cache_Works_OnRoot()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>("A");

            container.Resolve<IA>("A");
            container.Resolve<IA>("A");

            var scopeType = container.ContainerContext.RootScope.GetType();
            var delegateCache = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(container.ContainerContext.RootScope);
            var serviceFactories = delegateCache.GetType().GetField("ServiceDelegates").GetValue(delegateCache);

            var enumerator = (IEnumerable)serviceFactories.GetType().GetMethod("Walk").Invoke(serviceFactories, Type.EmptyTypes);

            var length = 0;
            foreach (var item in enumerator)
                length++;

            Assert.Equal(1, length);
        }

        [Fact]
        public void NamedResolveTests_Ensure_Delegate_Cache_Works_By_Value_OnRoot()
        {
            var key = new StringBuilder("A");
            using var container = new StashboxContainer()
                .Register<IA, A>("A");

            container.Resolve<IA>("A");
            container.Resolve<IA>(key.ToString());

            var scopeType = container.ContainerContext.RootScope.GetType();
            var delegateCache = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(container.ContainerContext.RootScope);
            var serviceFactories = delegateCache.GetType().GetField("ServiceDelegates").GetValue(delegateCache);

            var enumerator = (IEnumerable)serviceFactories.GetType().GetMethod("Walk").Invoke(serviceFactories, Type.EmptyTypes);

            var length = 0;
            foreach (var item in enumerator)
                length++;

            Assert.Equal(1, length);
        }

        [Fact]
        public void NamedResolveTests_Ensure_Delegate_Cache_Shared_Between_UnNamed_Scopes()
        {
            var key = new StringBuilder("A");
            using var container = new StashboxContainer();

            var scope1 = container.BeginScope();
            var scope2 = container.BeginScope();
            var scope3 = scope2.BeginScope();

            var scopeType = scope1.GetType();
            var delegateCache1 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope1);
            var delegateCache2 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope2);
            var delegateCache3 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope3);

            Assert.Same(delegateCache1, delegateCache2);
            Assert.Same(delegateCache2, delegateCache3);
        }

        [Fact]
        public void NamedResolveTests_Ensure_Delegate_Cache_Not_Shared_Between_Root_And_UnNamed_Scopes()
        {
            var key = new StringBuilder("A");
            using var container = new StashboxContainer();

            var scope = container.BeginScope();

            var scopeType = scope.GetType();
            var delegateCache1 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(scope);
            var delegateCache2 = scopeType.GetField("DelegateCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(container.ContainerContext.RootScope);

            Assert.NotSame(delegateCache1, delegateCache2);
        }

        [Fact]
        public void NamedResolveTests_Named_Scope_Cache_Works()
        {
            using var container = new StashboxContainer();

            container.BeginScope("A");
            container.BeginScope("A");

            var scopeType = container.ContainerContext.RootScope.GetType();
            var cacheProvider = scopeType.GetField("delegateCacheProvider", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(container.ContainerContext.RootScope);
            var namedCache = cacheProvider.GetType().GetField("NamedCache").GetValue(cacheProvider);
            var enumerator = (IEnumerable)namedCache.GetType().GetMethod("Walk").Invoke(namedCache, Type.EmptyTypes);

            var length = 0;
            foreach (var item in enumerator)
                length++;

            Assert.Equal(1, length);
        }

        [Fact]
        public void NamedResolveTests_Named_Scope_Cache_Works_Equality_By_Value()
        {
            var key = new StringBuilder("A");
            using var container = new StashboxContainer();

            container.BeginScope("A");
            container.BeginScope(key.ToString());

            var scopeType = container.ContainerContext.RootScope.GetType();
            var cacheProvider = scopeType.GetField("delegateCacheProvider", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(container.ContainerContext.RootScope);
            var namedCache = cacheProvider.GetType().GetField("NamedCache").GetValue(cacheProvider);
            var enumerator = (IEnumerable)namedCache.GetType().GetMethod("Walk").Invoke(namedCache, Type.EmptyTypes);

            var length = 0;
            foreach (var item in enumerator)
                length++;

            Assert.Equal(1, length);
        }

        interface IA { }

        class A : IA { }

        class B : IA { }

        class C : IA { }

        class D : IA { }
    }
}
