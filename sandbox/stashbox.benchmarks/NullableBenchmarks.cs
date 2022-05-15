extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class NullableBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer();

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer();

        [Benchmark(Baseline = true)]
        public object Old()
        {
            return this.oldContainer.ResolveOrDefault(typeof(object));
        }

        [Benchmark]
        public object New()
        {
            return this.newContainer.ResolveOrDefault(typeof(object));
        }
    }
}
