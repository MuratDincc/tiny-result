using System.Diagnostics;

namespace TinyResult;

public static class ResultMetrics
{
    private static readonly Dictionary<string, OperationMetrics> _metrics = new();
    private static readonly object _lock = new();

    public static void TrackOperation<T>(string operationName, Func<Result<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = operation();
        stopwatch.Stop();

        TrackOperation(operationName, result.IsSuccess, stopwatch.Elapsed);
    }

    public static async Task TrackOperationAsync<T>(string operationName, Func<Task<Result<T>>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await operation();
        stopwatch.Stop();

        TrackOperation(operationName, result.IsSuccess, stopwatch.Elapsed);
    }

    private static void TrackOperation(string operationName, bool isSuccess, TimeSpan duration)
    {
        lock (_lock)
        {
            if (!_metrics.ContainsKey(operationName))
            {
                _metrics[operationName] = new OperationMetrics();
            }

            var metrics = _metrics[operationName];
            metrics.TotalOperations++;
            metrics.TotalDuration += duration;

            if (isSuccess)
            {
                metrics.SuccessfulOperations++;
            }
            else
            {
                metrics.FailedOperations++;
            }

            if (duration > metrics.MaxDuration)
            {
                metrics.MaxDuration = duration;
            }

            if (duration < metrics.MinDuration || metrics.MinDuration == TimeSpan.Zero)
            {
                metrics.MinDuration = duration;
            }
        }
    }

    public static OperationMetrics GetMetrics(string operationName)
    {
        lock (_lock)
        {
            return _metrics.TryGetValue(operationName, out var metrics) 
                ? metrics 
                : new OperationMetrics();
        }
    }

    public static void ResetMetrics(string operationName)
    {
        lock (_lock)
        {
            if (_metrics.ContainsKey(operationName))
            {
                _metrics[operationName] = new OperationMetrics();
            }
        }
    }

    public static void ResetAllMetrics()
    {
        lock (_lock)
        {
            _metrics.Clear();
        }
    }
}

public class OperationMetrics
{
    public int TotalOperations { get; set; }
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public TimeSpan MaxDuration { get; set; }
    public TimeSpan MinDuration { get; set; }

    private double SuccessRate => TotalOperations > 0 
        ? (double)SuccessfulOperations / TotalOperations * 100 
        : 0;

    private double FailureRate => TotalOperations > 0 
        ? (double)FailedOperations / TotalOperations * 100 
        : 0;

    private TimeSpan AverageDuration => TotalOperations > 0 
        ? TimeSpan.FromTicks(TotalDuration.Ticks / TotalOperations) 
        : TimeSpan.Zero;

    public override string ToString()
    {
        return $"Total Operations: {TotalOperations}\n" +
               $"Successful: {SuccessfulOperations} ({SuccessRate:F2}%)\n" +
               $"Failed: {FailedOperations} ({FailureRate:F2}%)\n" +
               $"Average Duration: {AverageDuration}\n" +
               $"Min Duration: {MinDuration}\n" +
               $"Max Duration: {MaxDuration}";
    }
} 