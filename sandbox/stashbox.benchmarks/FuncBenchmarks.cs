extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;
using System;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class FuncBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer();

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer();

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
            var factory = (Func<B, A>)this.oldContainer.Resolve(typeof(Func<B, A>));
            return factory((B)this.oldContainer.Resolve(typeof(B)));
        }

        [Benchmark]
        public object New()
        {
            var factory = (Func<B, A>)this.newContainer.Resolve(typeof(Func<B, A>));
            return factory((B)this.newContainer.Resolve(typeof(B)));
        }

        class A
        {
            public A(B b)
            { }
        }

        class B { }
    }
}
