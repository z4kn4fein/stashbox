using Stashbox.Configuration;
using System;
using System.Threading;
using Xunit;

namespace Stashbox.Tests.IssueTests
{
    public class ExceptionWhenBuildingExpressions
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ensure_expression_built_correctly_scoped(bool useMicrosoftCompiler)
        {
            A.Counter = 0;
            B.Counter = 0;
            C.Counter = 0;
            D.Counter = 0;
            E.Counter = 0;
            F.Counter = 0;

            A inst = null;
            {
                using var container = new StashboxContainer(c =>
                {
                    c.WithRuntimeCircularDependencyTracking()
                        .WithDisposableTransientTracking();

                    if (useMicrosoftCompiler)
                        c.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                })
                .Register<A>(c => c.WithScopedLifetime())
                .Register<B>(c => c.WithScopedLifetime())
                .Register<D>(c => c.WithScopedLifetime())
                .Register<E>(c => c.WithScopedLifetime())
                .Register<F>(c => c.WithScopedLifetime())
                .Register<C>(c => c.WithFactory(r => new C()).WithScopedLifetime().WithoutDisposalTracking());

                using var scope = container.BeginScope();
                inst = scope.Resolve<A>();

                Assert.NotNull(inst);
                Assert.NotNull(inst.B);
                Assert.NotNull(inst.C);
                Assert.NotNull(inst.B.D);
                Assert.NotNull(inst.B.C);
                Assert.NotNull(inst.B.D.C);
                Assert.NotNull(inst.B.D.E);
                Assert.NotNull(inst.B.D.F);
                Assert.NotNull(inst.B.D.E.C);
                Assert.NotNull(inst.B.D.F.C);

                Assert.Same(inst.C, inst.B.C);
                Assert.Same(inst.B.C, inst.B.D.C);
                Assert.Same(inst.B.D.C, inst.B.D.E.C);
                Assert.Same(inst.B.D.E.C, inst.B.D.F.C);
            }

            Assert.True(inst.Disposed);
            Assert.True(inst.B.Disposed);
            Assert.False(inst.C.Disposed);
            Assert.True(inst.B.D.Disposed);
            Assert.False(inst.B.C.Disposed);
            Assert.False(inst.B.D.C.Disposed);
            Assert.True(inst.B.D.E.Disposed);
            Assert.True(inst.B.D.F.Disposed);
            Assert.False(inst.B.D.E.C.Disposed);
            Assert.False(inst.B.D.F.C.Disposed);

            Assert.Equal(1, A.Counter);
            Assert.Equal(1, B.Counter);
            Assert.Equal(1, C.Counter);
            Assert.Equal(1, D.Counter);
            Assert.Equal(1, E.Counter);
            Assert.Equal(1, F.Counter);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ensure_expression_built_correctly_scoped_one_singleton(bool useMicrosoftCompiler)
        {
            A.Counter = 0;
            B.Counter = 0;
            C.Counter = 0;
            D.Counter = 0;
            E.Counter = 0;
            F.Counter = 0;

            A inst = null;
            {
                using var container = new StashboxContainer(c =>
                {
                    c.WithRuntimeCircularDependencyTracking()
                        .WithDisposableTransientTracking();

                    if (useMicrosoftCompiler)
                        c.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                })
                .Register<A>(c => c.WithScopedLifetime())
                .Register<B>(c => c.WithSingletonLifetime())
                .Register<D>(c => c.WithScopedLifetime())
                .Register<E>(c => c.WithScopedLifetime())
                .Register<F>(c => c.WithScopedLifetime())
                .Register<C>(c => c.WithFactory(r => new C()).WithScopedLifetime().WithoutDisposalTracking());

                using var scope = container.BeginScope();
                inst = scope.Resolve<A>();

                Assert.NotNull(inst);
                Assert.NotNull(inst.B);
                Assert.NotNull(inst.C);
                Assert.NotNull(inst.B.D);
                Assert.NotNull(inst.B.C);
                Assert.NotNull(inst.B.D.C);
                Assert.NotNull(inst.B.D.E);
                Assert.NotNull(inst.B.D.F);
                Assert.NotNull(inst.B.D.E.C);
                Assert.NotNull(inst.B.D.F.C);

                Assert.NotSame(inst.C, inst.B.C);
                Assert.Same(inst.B.C, inst.B.D.C);
                Assert.Same(inst.B.D.C, inst.B.D.E.C);
                Assert.Same(inst.B.D.E.C, inst.B.D.F.C);
            }

            Assert.True(inst.Disposed);
            Assert.True(inst.B.Disposed);
            Assert.False(inst.C.Disposed);
            Assert.True(inst.B.D.Disposed);
            Assert.False(inst.B.C.Disposed);
            Assert.False(inst.B.D.C.Disposed);
            Assert.True(inst.B.D.E.Disposed);
            Assert.True(inst.B.D.F.Disposed);
            Assert.False(inst.B.D.E.C.Disposed);
            Assert.False(inst.B.D.F.C.Disposed);

            Assert.Equal(1, A.Counter);
            Assert.Equal(1, B.Counter);
            Assert.Equal(2, C.Counter);
            Assert.Equal(1, D.Counter);
            Assert.Equal(1, E.Counter);
            Assert.Equal(1, F.Counter);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ensure_expression_built_correctly_singleton(bool useMicrosoftCompiler)
        {
            A.Counter = 0;
            B.Counter = 0;
            C.Counter = 0;
            D.Counter = 0;
            E.Counter = 0;
            F.Counter = 0;

            A inst = null;
            {
                using var container = new StashboxContainer(c =>
                {
                    c.WithDisposableTransientTracking();

                    if (useMicrosoftCompiler)
                        c.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                })
                    .Register<A>(c => c.DefinesScope("A"))
                    .Register<B>(c => c.WithSingletonLifetime().DefinesScope("B"))
                    .Register<D>(c => c.DefinesScope("D").WithScopedLifetime())
                    .Register<E>(c => c.DefinesScope("E"))
                    .Register<F>(c => c.DefinesScope("F"))
                    .Register<C>(c => c.WithFactory(r => new C()).WithoutDisposalTracking().WithScopedLifetime());

                {
                    using var scope = container.BeginScope();
                    inst = scope.Resolve<A>();

                    Assert.NotNull(inst);
                    Assert.NotNull(inst.B);
                    Assert.NotNull(inst.C);
                    Assert.NotNull(inst.B.D);
                    Assert.NotNull(inst.B.C);
                    Assert.NotNull(inst.B.D.C);
                    Assert.NotNull(inst.B.D.E);
                    Assert.NotNull(inst.B.D.F);
                    Assert.NotNull(inst.B.D.E.C);
                    Assert.NotNull(inst.B.D.F.C);

                    Assert.NotSame(inst.C, inst.B.C);
                    Assert.NotSame(inst.B.C, inst.B.D.C);
                    Assert.NotSame(inst.B.D.C, inst.B.D.E.C);
                    Assert.NotSame(inst.B.D.E.C, inst.B.D.F.C);
                }

                Assert.True(inst.Disposed);
                Assert.False(inst.B.Disposed);
                Assert.False(inst.C.Disposed);
                Assert.False(inst.B.D.Disposed);
                Assert.False(inst.B.C.Disposed);
                Assert.False(inst.B.D.C.Disposed);
                Assert.False(inst.B.D.E.Disposed);
                Assert.False(inst.B.D.F.Disposed);
                Assert.False(inst.B.D.E.C.Disposed);
                Assert.False(inst.B.D.F.C.Disposed);
            }

            Assert.True(inst.Disposed);
            Assert.True(inst.B.Disposed);
            Assert.False(inst.C.Disposed);
            Assert.True(inst.B.D.Disposed);
            Assert.False(inst.B.C.Disposed);
            Assert.False(inst.B.D.C.Disposed);
            Assert.True(inst.B.D.E.Disposed);
            Assert.True(inst.B.D.F.Disposed);
            Assert.False(inst.B.D.E.C.Disposed);
            Assert.False(inst.B.D.F.C.Disposed);

            Assert.Equal(1, A.Counter);
            Assert.Equal(1, B.Counter);
            Assert.Equal(5, C.Counter);
            Assert.Equal(1, D.Counter);
            Assert.Equal(1, E.Counter);
            Assert.Equal(1, F.Counter);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ensure_expression_built_correctly_mixed(bool useMicrosoftCompiler)
        {
            A.Counter = 0;
            B.Counter = 0;
            C.Counter = 0;
            D.Counter = 0;
            E.Counter = 0;
            F.Counter = 0;

            A inst = null;
            {
                using var container = new StashboxContainer(c =>
                {
                    c.WithDisposableTransientTracking();

                    if (useMicrosoftCompiler)
                        c.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                })
                    .Register<A>()
                    .Register<B>()
                    .Register<D>(c => c.WithScopedLifetime())
                    .Register<E>(c => c.WithScopedLifetime())
                    .Register<F>(c => c.WithScopedLifetime())
                    .Register<C>(c => c.WithSingletonLifetime());

                {
                    for (int i = 0; i < 5; i++)
                    {
                        {
                            using var scope = container.BeginScope();
                            for (int j = 0; j < 5; j++)
                            {
                                inst = scope.Resolve<A>();

                                Assert.NotNull(inst);
                                Assert.NotNull(inst.B);
                                Assert.NotNull(inst.C);
                                Assert.NotNull(inst.B.D);
                                Assert.NotNull(inst.B.C);
                                Assert.NotNull(inst.B.D.C);
                                Assert.NotNull(inst.B.D.E);
                                Assert.NotNull(inst.B.D.F);
                                Assert.NotNull(inst.B.D.E.C);
                                Assert.NotNull(inst.B.D.F.C);

                                Assert.Same(inst.C, inst.B.C);
                                Assert.Same(inst.B.C, inst.B.D.C);
                                Assert.Same(inst.B.D.C, inst.B.D.E.C);
                                Assert.Same(inst.B.D.E.C, inst.B.D.F.C);
                            }
                        }

                        Assert.True(inst.Disposed);
                        Assert.True(inst.B.Disposed);
                        Assert.False(inst.C.Disposed);
                        Assert.True(inst.B.D.Disposed);
                        Assert.False(inst.B.C.Disposed);
                        Assert.False(inst.B.D.C.Disposed);
                        Assert.True(inst.B.D.E.Disposed);
                        Assert.True(inst.B.D.F.Disposed);
                        Assert.False(inst.B.D.E.C.Disposed);
                        Assert.False(inst.B.D.F.C.Disposed);
                    }
                }
            }

            Assert.True(inst.Disposed);
            Assert.True(inst.B.Disposed);
            Assert.True(inst.C.Disposed);
            Assert.True(inst.B.D.Disposed);
            Assert.True(inst.B.C.Disposed);
            Assert.True(inst.B.D.C.Disposed);
            Assert.True(inst.B.D.E.Disposed);
            Assert.True(inst.B.D.F.Disposed);
            Assert.True(inst.B.D.E.C.Disposed);
            Assert.True(inst.B.D.F.C.Disposed);

            Assert.Equal(25, A.Counter);
            Assert.Equal(25, B.Counter);
            Assert.Equal(1, C.Counter);
            Assert.Equal(5, D.Counter);
            Assert.Equal(5, E.Counter);
            Assert.Equal(5, F.Counter);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ensure_expression_built_correctly_singleton_dispose(bool useMicrosoftCompiler)
        {
            C.Counter = 0;
            D inst = null;
            {
                using var container = new StashboxContainer(c =>
                {
                    c.WithDisposableTransientTracking();

                    if (useMicrosoftCompiler)
                        c.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                })
                    .Register<D>(c => c.WithScopedLifetime().DefinesScope())
                    .Register<E>(c => c.WithSingletonLifetime().DefinesScope())
                    .Register<F>(c => c.WithScopedLifetime().DefinesScope())
                    .Register<C>(c => c.WithScopedLifetime());


                {
                    using var scope = container.BeginScope();
                    inst = scope.Resolve<D>();
                }

                Assert.False(inst.E.Disposed);
            }

            Assert.True(inst.E.Disposed);

            Assert.Equal(3, C.Counter);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ensure_expression_built_correctly_singleton_dispose_simple(bool useMicrosoftCompiler)
        {
            C.Counter = 0;
            F inst = null;
            {
                using var container = new StashboxContainer(c =>
                {
                    c.WithDisposableTransientTracking();

                    if (useMicrosoftCompiler)
                        c.WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler);
                })
                    .Register<F>(c => c.WithScopedLifetime())
                    .Register<C>(c => c.WithSingletonLifetime());


                {
                    using var scope = container.BeginScope();
                    inst = scope.Resolve<F>();
                }

                Assert.True(inst.Disposed);
                Assert.False(inst.C.Disposed);
            }

            Assert.True(inst.C.Disposed);

            Assert.Equal(1, C.Counter);
        }

        class A : IDisposable
        {
            public static int Counter = 0;

            public A(B b, C c)
            {
                Interlocked.Increment(ref Counter);
                B = b;
                C = c;
            }

            public B B { get; }
            public C C { get; }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(A));

                this.Disposed = true;
            }
        }

        class B : IDisposable
        {
            public static int Counter = 0;

            public B(C c, D d)
            {
                Interlocked.Increment(ref Counter);
                C = c;
                D = d;
            }

            public C C { get; }
            public D D { get; }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(B));

                this.Disposed = true;
            }
        }

        class C : IDisposable
        {
            public static int Counter = 0;

            public C()
            {
                Interlocked.Increment(ref Counter);
            }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(C));

                this.Disposed = true;
            }
        }

        class D : IDisposable
        {
            public static int Counter = 0;

            public D(C c, E e, F f)
            {
                Interlocked.Increment(ref Counter);
                C = c;
                E = e;
                F = f;
            }

            public C C { get; }
            public E E { get; }
            public F F { get; }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(D));

                this.Disposed = true;
            }
        }

        class E : IDisposable
        {
            public static int Counter = 0;

            public E(C c)
            {
                Interlocked.Increment(ref Counter);
                C = c;
            }

            public C C { get; }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(E));

                this.Disposed = true;
            }
        }

        class F : IDisposable
        {
            public static int Counter = 0;

            public F(C c)
            {
                Interlocked.Increment(ref Counter);
                C = c;
            }

            public C C { get; }

            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                    throw new ObjectDisposedException(nameof(F));

                this.Disposed = true;
            }
        }
    }
}
