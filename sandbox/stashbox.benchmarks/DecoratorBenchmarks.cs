extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class DecoratorBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer();

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer();

        [GlobalSetup]
        public void Setup()
        {
            this.oldContainer.Register<IA, A>()
                .RegisterDecorator<IA, ADec>();

            this.newContainer.Register<IA, A>()
                .RegisterDecorator<IA, ADec>();
        }

        [Benchmark(Baseline = true)]
        public object Old()
        {
            return this.oldContainer.Resolve(typeof(IA));
        }

        [Benchmark]
        public object New()
        {
            return this.newContainer.Resolve(typeof(IA));
        }

        interface IA { }

        class A : IA { }

        class ADec : IA
        {
            public ADec(IA a) { }
        }
    }
}
