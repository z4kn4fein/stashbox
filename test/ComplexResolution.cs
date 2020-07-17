using Stashbox.Configuration;
using Stashbox.Tests.Utils;
using Xunit;

namespace Stashbox.Tests
{
    public class ComplexResolution
    {
        [Theory]
        [ClassData(typeof(CompilerTypeTestData))]
        public void Ensure_Complex_Resolution_Works(CompilerType compilerType)
        {
            using var container = new StashboxContainer(c => c.WithDisposableTransientTracking().WithCompiler(compilerType))
                .Register<T1>()
                .Register<T2>()
                .Register<T3>()
                .Register<T4>()
                .RegisterScoped<Scoped1>()
                .RegisterScoped<Scoped2>()
                .RegisterScoped<Scoped3>()
                .RegisterSingleton<Single1>()
                .RegisterSingleton<Single2>()
                .RegisterSingleton<Single3>()
                .Register<Func1>(c => c.WithFactory(r => new Func1(r.Resolve<T4>(), r.Resolve<Scoped3>(), r.Resolve<Single3>())))
                .Register<Func2>(c => c.WithFactory(r => new Func2(r.Resolve<T4>(), r.Resolve<Single1>(), r.Resolve<Single2>())));

            {
                using var scope = container.BeginScope();
                var t1 = scope.Resolve<T1>();

                Assert.NotNull(t1);
                Assert.NotNull(t1.T2);
                Assert.NotNull(t1.Single1);
                Assert.NotNull(t1.Scoped1);
                Assert.NotNull(t1.Scoped2);
                Assert.NotNull(t1.T2.T3);
                Assert.NotNull(t1.T2.T4);
                Assert.NotNull(t1.T2.Single1);
                Assert.NotNull(t1.T2.Single2);
                Assert.NotNull(t1.T2.Scoped1);
                Assert.NotNull(t1.T2.Scoped3);
                Assert.NotNull(t1.Single1.Single2);
                Assert.NotNull(t1.Single1.Single3);
                Assert.NotNull(t1.Scoped1.T4);
                Assert.NotNull(t1.Scoped1.Single1);
                Assert.NotNull(t1.Scoped1.Scoped2);
                Assert.NotNull(t1.Scoped1.Func1);
                Assert.NotNull(t1.Scoped2.T4);
                Assert.NotNull(t1.Scoped2.Single2);
                Assert.NotNull(t1.Scoped2.Single3);
                Assert.NotNull(t1.Scoped2.Scoped3);
                Assert.NotNull(t1.Scoped2.Func2);
                Assert.NotNull(t1.T2.T3.T4);
                Assert.NotNull(t1.T2.T3.Scoped1);
                Assert.NotNull(t1.T2.T3.Scoped3);
                Assert.NotNull(t1.T2.T3.Single3);
                Assert.NotNull(t1.Scoped1.Func1.T4);
                Assert.NotNull(t1.Scoped1.Func1.Scoped3);
                Assert.NotNull(t1.Scoped1.Func1.Single3);
            }

            {
                using var scope1 = container.BeginScope();
                var t2 = scope1.Resolve<T2>();

                Assert.NotNull(t2);
                Assert.NotNull(t2.T3);
                Assert.NotNull(t2.T4);
                Assert.NotNull(t2.Single1);
                Assert.NotNull(t2.Single2);
                Assert.NotNull(t2.Scoped1);
                Assert.NotNull(t2.Scoped3);
                Assert.NotNull(t2.Single1.Single2);
                Assert.NotNull(t2.Single1.Single3);
                Assert.NotNull(t2.Scoped1.T4);
                Assert.NotNull(t2.Scoped1.Single1);
                Assert.NotNull(t2.Scoped1.Scoped2);
                Assert.NotNull(t2.Scoped1.Func1);
                Assert.NotNull(t2.T3.T4);
                Assert.NotNull(t2.T3.Scoped1);
                Assert.NotNull(t2.T3.Scoped3);
                Assert.NotNull(t2.T3.Single3);
                Assert.NotNull(t2.Scoped1.Func1.T4);
                Assert.NotNull(t2.Scoped1.Func1.Scoped3);
                Assert.NotNull(t2.Scoped1.Func1.Single3);
            }
        }

        class T1
        {
            public T1(T2 t2, Scoped1 scoped1, Scoped2 scoped2, Single1 single1)
            {
                T2 = t2;
                Scoped1 = scoped1;
                Scoped2 = scoped2;
                Single1 = single1;
            }

            public T2 T2 { get; }
            public Scoped1 Scoped1 { get; }
            public Scoped2 Scoped2 { get; }
            public Single1 Single1 { get; }
        }

        class T2
        {
            public T2(T3 t3, T4 t4, Scoped1 scoped1, Scoped3 scoped3, Single1 single1, Single2 single2)
            {
                T3 = t3;
                T4 = t4;
                Scoped1 = scoped1;
                Scoped3 = scoped3;
                Single1 = single1;
                Single2 = single2;
            }

            public T3 T3 { get; }
            public T4 T4 { get; }
            public Scoped1 Scoped1 { get; }
            public Scoped3 Scoped3 { get; }
            public Single1 Single1 { get; }
            public Single2 Single2 { get; }
        }

        class T3
        {
            public T3(T4 t4, Scoped1 scoped1, Scoped3 scoped3, Single3 single3)
            {
                T4 = t4;
                Scoped1 = scoped1;
                Scoped3 = scoped3;
                Single3 = single3;
            }

            public T4 T4 { get; }
            public Scoped1 Scoped1 { get; }
            public Scoped3 Scoped3 { get; }
            public Single3 Single3 { get; }
        }

        class T4 { }

        class Scoped1
        {
            public Scoped1(T4 t4, Scoped2 scoped2, Func1 func1, Single1 single1)
            {
                T4 = t4;
                Scoped2 = scoped2;
                Func1 = func1;
                Single1 = single1;
            }

            public T4 T4 { get; }
            public Scoped2 Scoped2 { get; }
            public Func1 Func1 { get; }
            public Single1 Single1 { get; }
        }

        class Scoped2
        {
            public Scoped2(T4 t4, Scoped3 scoped3, Func2 func2, Single2 single2, Single3 single3)
            {
                T4 = t4;
                Scoped3 = scoped3;
                Func2 = func2;
                Single2 = single2;
                Single3 = single3;
            }

            public T4 T4 { get; }
            public Scoped3 Scoped3 { get; }
            public Func2 Func2 { get; }
            public Single2 Single2 { get; }
            public Single3 Single3 { get; }
        }

        class Scoped3 { }

        class Single1
        {
            public Single1(Single2 single2, Single3 single3)
            {
                Single2 = single2;
                Single3 = single3;
            }

            public Single2 Single2 { get; }
            public Single3 Single3 { get; }
        }

        class Single2
        {
            public Single2(Single3 single3)
            {
                Single3 = single3;
            }

            public Single3 Single3 { get; }
        }

        class Single3 { }

        class Func1
        {
            public Func1(T4 t4, Scoped3 scoped3, Single3 single3)
            {
                T4 = t4;
                Scoped3 = scoped3;
                Single3 = single3;
            }

            public T4 T4 { get; }
            public Scoped3 Scoped3 { get; }
            public Single3 Single3 { get; }
        }

        class Func2
        {
            public Func2(T4 t4, Single1 single1, Single2 single2)
            {
                T4 = t4;
                Single1 = single1;
                Single2 = single2;
            }
            public T4 T4 { get; }
            public Single1 Single1 { get; }
            public Single2 Single2 { get; }
        }
    }
}