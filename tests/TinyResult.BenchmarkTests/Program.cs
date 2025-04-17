using BenchmarkDotNet.Running;

namespace TinyResult.BenchmarkTests;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<ResultBenchmarks>();
    }
}
