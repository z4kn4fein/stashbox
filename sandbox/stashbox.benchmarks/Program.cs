using System;
using BenchmarkDotNet.Running;

namespace Stashbox.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DisposeBenchmarks>();
            Console.ReadKey();
        }
    }
}
