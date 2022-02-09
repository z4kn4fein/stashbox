using System;
using BenchmarkDotNet.Running;

namespace Stashbox.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<DisposeBenchmarks>();
            //BenchmarkRunner.Run<EnumerableBenchmarks>();
            //BenchmarkRunner.Run<PropertyBenchmarks>();
            //BenchmarkRunner.Run<RegisterBenchmarks>();
            //BenchmarkRunner.Run<ResolveBenchmarks>();
            BenchmarkRunner.Run<ScopedBenchmarks>();
            //BenchmarkRunner.Run<FuncBenchmarks>();
            //BenchmarkRunner.Run<FinalizerBenchmarks>();
            //BenchmarkRunner.Run<NullableBenchmarks>();
            //BenchmarkRunner.Run<BeginScopeBenchmarks>();
            //BenchmarkRunner.Run<SingletonBenchmarks>();
            Console.ReadKey();
        }
    }
}
