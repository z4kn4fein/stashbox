extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class EnumerableBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer();

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer();

        [GlobalSetup]
        public void Setup()
        {
            this.oldContainer.Register<IA, A>()
                .Register<IA, AA>()
                .Register<IA, AAA>()
                .Register<B>();

            this.newContainer.Register<IA, A>()
                .Register<IA, AA>()
                .Register<IA, AAA>()
                .Register<B>();
        }

        [Benchmark(Baseline = true)]
        public object Old()
        {
            return this.oldContainer.ResolveAll(typeof(A));
        }

        [Benchmark]
        public object New()
        {
            return this.newContainer.ResolveAll(typeof(A));
        }

        interface IA { }

        class A : IA
        {
            public A(B b)
            { }
        }

        class AA : IA
        {
            public AA(B b)
            { }
        }

        class AAA : IA
        {
            public AAA(B b)
            { }
        }

        class B { }
    }
}
