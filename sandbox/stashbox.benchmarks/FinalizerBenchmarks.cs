extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class FinalizerBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer();

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer();

        [GlobalSetup]
        public void Setup()
        {
            this.oldContainer.Register<A>(c => c.WithFinalizer(a => a.Finalize()).WithScopedLifetime());

            this.newContainer.Register<A>(c => c.WithFinalizer(a => a.Finalize()).WithScopedLifetime());
        }

        [Benchmark(Baseline = true)]
        public object Old()
        {
            using var scope = this.oldContainer.BeginScope();
            scope.Resolve(typeof(A));
            scope.Resolve(typeof(A));
            scope.Resolve(typeof(A));
            scope.Resolve(typeof(A));

            using var scope2 = this.oldContainer.BeginScope();
            scope2.Resolve(typeof(A));
            scope2.Resolve(typeof(A));
            scope2.Resolve(typeof(A));
            return scope2.Resolve(typeof(A));
        }

        [Benchmark]
        public object New()
        {
            using var scope = this.newContainer.BeginScope();
            scope.Resolve(typeof(A));
            scope.Resolve(typeof(A));
            scope.Resolve(typeof(A));
            scope.Resolve(typeof(A));

            using var scope2 = this.newContainer.BeginScope();
            scope2.Resolve(typeof(A));
            scope2.Resolve(typeof(A));
            scope2.Resolve(typeof(A));
            return scope2.Resolve(typeof(A));
        }

        class A
        {
            private bool isFinalized;

            public void Finalize()
            {
                this.isFinalized = true;
            }
        }
    }
}
