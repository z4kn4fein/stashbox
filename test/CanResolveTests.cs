using System;
using System.Collections.Generic;
using Xunit;

namespace Stashbox.Tests
{
    public class CanResolveTests
    {
        [Fact]
        public void CanResolveTests_ExplicitService()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>();

            Assert.True(container.CanResolve<IA>());
            Assert.False(container.CanResolve<A>());
        }

        [Fact]
        public void CanResolveTests_OpenGeneric()
        {
            using var container = new StashboxContainer()
                .Register(typeof(IB<>), typeof(B<>));

            Assert.False(container.CanResolve(typeof(IB<>)));
            Assert.True(container.CanResolve<IB<IA>>());
        }

        [Fact]
        public void CanResolveTests_ClosedGeneric()
        {
            using var container = new StashboxContainer()
                .Register(typeof(IB<IA>), typeof(B<IA>));

            Assert.True(container.CanResolve<IB<IA>>());
        }

        [Fact]
        public void CanResolveTests_Wrapper()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>();

            Assert.True(container.CanResolve<Func<IA>>());
            Assert.True(container.CanResolve<Lazy<IA>>());
            Assert.True(container.CanResolve<Tuple<IA>>());
        }

        [Fact]
        public void CanResolveTests_Enumerable()
        {
            using var container = new StashboxContainer()
                .Register<IA, A>();

            Assert.True(container.CanResolve<IEnumerable<IA>>());
            Assert.True(container.CanResolve<IEnumerable<IB<IA>>>());
        }

        interface IA { }

        class A : IA { }

        interface IB<T> { }

        class B<T> : IB<T> { }
    }
}
