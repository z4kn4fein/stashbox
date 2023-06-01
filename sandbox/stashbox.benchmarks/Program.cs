using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Stashbox.Benchmarks
{
    class Program
    {
        static void Main()
        {
            var config = ManualConfig.Create(DefaultConfig.Instance)
             .WithOptions(ConfigOptions.JoinSummary)
             .WithOptions(ConfigOptions.DisableLogFile);

            BenchmarkRunner.Run(
                new[]
                {
                    // BenchmarkConverter.TypeToBenchmarks( typeof(DisposeBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(EnumerableBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(PropertyBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(RegisterBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(ResolveBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(ScopedBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(FuncBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(FinalizerBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(NullableBenchmarks), config),
                    BenchmarkConverter.TypeToBenchmarks( typeof(BeginScopeBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(SingletonBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(DecoratorBenchmarks), config),
                    // BenchmarkConverter.TypeToBenchmarks( typeof(ChildContainerBenchmarks), config),
                });

            Console.ReadKey();
        }
    }
}
