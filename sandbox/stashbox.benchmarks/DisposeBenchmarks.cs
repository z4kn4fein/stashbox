extern alias from_nuget;
extern alias from_project;
using BenchmarkDotNet.Attributes;
using System;

namespace Stashbox.Benchmarks
{
    [MemoryDiagnoser]
    public class DisposeBenchmarks
    {
        private readonly from_nuget::Stashbox.IStashboxContainer oldContainer =
            new from_nuget::Stashbox.StashboxContainer(c => c.WithDisposableTransientTracking()
            .WithDefaultLifetime(from_nuget::Stashbox.Lifetime.Lifetimes.Scoped));

        private readonly from_project::Stashbox.IStashboxContainer newContainer =
            new from_project::Stashbox.StashboxContainer(c => c.WithDisposableTransientTracking()
            .WithDefaultLifetime(from_project::Stashbox.Lifetime.Lifetimes.Scoped));

        [GlobalSetup]
        public void Setup()
        {
            this.oldContainer.Register<DisposableObj2>()
                .Register<DisposableObj1>()
                .Register<DisposableObj3>()
                .Register<DisposableObj4>();

            this.newContainer.Register<DisposableObj2>()
                .Register<DisposableObj1>()
                .Register<DisposableObj3>()
                .Register<DisposableObj4>();
        }

        [Benchmark(Baseline = true)]
        public object Old()
        {
            using var scope1 = this.oldContainer.BeginScope();
            scope1.Resolve(typeof(DisposableObj2));

            using var scope2 = this.oldContainer.BeginScope();
            scope2.Resolve(typeof(DisposableObj2));

            using var scope5 = this.oldContainer.BeginScope();
            return scope5.Resolve(typeof(DisposableObj2));
        }

        [Benchmark]
        public object New()
        {
            using var scope1 = this.newContainer.BeginScope();
            scope1.Resolve(typeof(DisposableObj2));

            using var scope2 = this.newContainer.BeginScope();
            scope2.Resolve(typeof(DisposableObj2));

            using var scope5 = this.newContainer.BeginScope();
            return scope5.Resolve(typeof(DisposableObj2));
        }

        private class DisposableObj1 : IDisposable
        {
            public void Dispose()
            { }
        }

        private class DisposableObj3 : IDisposable
        {
            private readonly DisposableObj4 obj4;
            private readonly DisposableObj1 obj1;

            public DisposableObj3(DisposableObj4 obj4, DisposableObj1 obj1)
            {
                this.obj4 = obj4;
                this.obj1 = obj1;
            }

            public void Dispose()
            { }
        }

        private class DisposableObj4 : IDisposable
        {
            private readonly DisposableObj1 obj1;

            public DisposableObj4(DisposableObj1 obj1)
            {
                this.obj1 = obj1;
            }

            public void Dispose()
            { }
        }

        private class DisposableObj2 : IDisposable
        {
            private readonly DisposableObj1 obj1;
            private readonly DisposableObj3 obj3;

            public DisposableObj2(DisposableObj1 obj1, DisposableObj3 obj3)
            {
                this.obj1 = obj1;
                this.obj3 = obj3;
            }

            public void Dispose()
            { }
        }
    }
}
