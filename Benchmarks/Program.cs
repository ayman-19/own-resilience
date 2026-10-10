using BenchmarkDotNet.Running;

namespace Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<RetryBenchmarks>();

        Console.ReadLine();
    }
}
