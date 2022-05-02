extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class RegisterBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer();

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer();

        [Benchmark(Baseline = true)]
        public object Old()
        {
            return this.oldContainer.Register<A>().Register<B>();
        }

        [Benchmark]
        public object New()
        {
            return this.newContainer.Register<A>().Register<B>();
        }

        class A { }

        class B { }
    }
}
