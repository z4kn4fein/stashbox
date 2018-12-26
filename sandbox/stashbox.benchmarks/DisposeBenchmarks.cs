extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;
using System;

namespace Stashbox.Benchmarks
{
    [ClrJob, CoreJob, MemoryDiagnoser]
    public class DisposeBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer(c => c.WithDisposableTransientTracking());

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer(c => c.WithDisposableTransientTracking());

        [GlobalSetup]
        public void Setup()
        {
            this.oldContainer.Register<DisposableObj2>().Register<DisposableObj1>();
            this.newContainer.Register<DisposableObj2>().Register<DisposableObj1>();
        }

        [Benchmark(Baseline = true)]
        public object Old()
        {
            using (var scope = this.oldContainer.BeginScope())
            {
                return scope.Resolve(typeof(DisposableObj2));
            }
        }

        [Benchmark]
        public object New()
        {
            using (var scope = this.newContainer.BeginScope())
            {
                return scope.Resolve(typeof(DisposableObj2));
            }
        }

        private class DisposableObj1 : IDisposable
        {
            public void Dispose()
            { }
        }

        private class DisposableObj2 : IDisposable
        {
            private readonly DisposableObj1 obj;

            public DisposableObj2(DisposableObj1 obj)
            {
                this.obj = obj;
            }

            public void Dispose()
            { }
        }
    }
}
