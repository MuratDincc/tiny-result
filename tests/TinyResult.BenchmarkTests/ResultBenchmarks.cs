using BenchmarkDotNet.Attributes;

namespace TinyResult.BenchmarkTests;

[MemoryDiagnoser]
public class ResultBenchmarks
{
    private const int Iterations = 1000;

    [Benchmark]
    public void CreateSuccessResult()
    {
        for (int i = 0; i < Iterations; i++)
        {
            var result = Result<int>.Success(42);
        }
    }

    [Benchmark]
    public void CreateFailureResult()
    {
        for (int i = 0; i < Iterations; i++)
        {
            var result = Result<int>.Failure("Error");
        }
    }

    [Benchmark]
    public void MapSuccessResult()
    {
        var result = Result<int>.Success(42);
        for (int i = 0; i < Iterations; i++)
        {
            var mapped = result.Map(x => x * 2);
        }
    }

    [Benchmark]
    public void MapFailureResult()
    {
        var result = Result<int>.Failure("Error");
        for (int i = 0; i < Iterations; i++)
        {
            var mapped = result.Map(x => x * 2);
        }
    }

    [Benchmark]
    public void BindSuccessResult()
    {
        var result = Result<int>.Success(42);
        for (int i = 0; i < Iterations; i++)
        {
            var bound = result.Bind(x => Result<int>.Success(x * 2));
        }
    }

    [Benchmark]
    public void BindFailureResult()
    {
        var result = Result<int>.Failure("Error");
        for (int i = 0; i < Iterations; i++)
        {
            var bound = result.Bind(x => Result<int>.Success(x * 2));
        }
    }
} 