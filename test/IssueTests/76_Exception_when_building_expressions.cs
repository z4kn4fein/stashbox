using System;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class ExceptionWhenBuildingExpressions
    {
        [Fact]
        public void Ensure_expression_built_correctly()
        {
            var inst = new StashboxContainer(c => c.WithCircularDependencyTracking(true).WithDisposableTransientTracking().WithMicrosoftExpressionCompiler())
                .Register<A>(c => c.WithScopedLifetime())
                .Register<B>(c => c.WithScopedLifetime())
                .Register<C>(c => c.WithFactory(r => new C()).WithScopedLifetime().WithoutDisposalTracking()).BeginScope().Resolve<A>();

            Assert.NotNull(inst);
            Assert.Same(inst.C, inst.B.C);
        }

        class A : IDisposable
        {
            public A(B b, C c)
            {
                B = b;
                C = c;
            }

            public B B { get; }
            public C C { get; }

            public void Dispose()
            { }
        }

        class B : IDisposable
        {
            public B(C c)
            {
                C = c;
            }

            public C C { get; }

            public void Dispose()
            { }
        }

        class C : IDisposable
        {
            public void Dispose()
            { }
        }
    }
}
