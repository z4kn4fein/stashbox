extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class ScopedBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer(c => c
            .WithDefaultLifetime(from_nuget::Stashbox.Lifetime.Lifetimes.Scoped));

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer(c => c
            .WithDefaultLifetime(from_project::Stashbox.Lifetime.Lifetimes.Scoped));

        [GlobalSetup]
        public void Setup()
        {
            this.oldContainer.Register<A>()
                .Register<B>();

            this.newContainer.Register<A>()
                .Register<B>();
        }

        [Benchmark(Baseline = true)]
        public object Old()
        {
            using var scope = this.oldContainer.BeginScope();
            return scope.Resolve(typeof(A));
        }

        [Benchmark]
        public object New()
        {
            using var scope = this.newContainer.BeginScope();
            return scope.Resolve(typeof(A));
        }

        class A
        {
            public A(B b)
            { }
        }

        class B { }
    }
}
