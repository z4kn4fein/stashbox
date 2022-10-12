extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class ChildContainerBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer();

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer();

        [GlobalSetup]
        public void Setup()
        {
            this.oldContainer.Register<A>()
                .Register<B>()
                .Register<C>();

            this.newContainer.Register<A>()
                .Register<B>()
                .Register<C>();
        }

        [Benchmark(Baseline = true)]
        public object Old()
        {
            var child = this.oldContainer.CreateChildContainer();
            child.Register<B>();
            return this.oldContainer.Resolve(typeof(A));
        }

        [Benchmark]
        public object New()
        {
            var child = this.newContainer.CreateChildContainer();
            child.Register<B>();
            return this.newContainer.Resolve(typeof(A));
        }

        class A
        {
            public A(B b, C c)
            { }
        }

        class B
        {
            public B(C c)
            { }
        }

        class C { }
    }
}
