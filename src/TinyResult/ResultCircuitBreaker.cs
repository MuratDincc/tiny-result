using TinyResult.Configurations;
using TinyResult.Enums;

namespace TinyResult;

public class ResultCircuitBreaker
{
    private readonly string _operationName;
    private readonly CircuitBreakerSettings _settings;
    private int _failureCount;
    private int _halfOpenSuccessCount;
    private DateTime _lastFailureTime;
    private CircuitState _state;
    private readonly object _lock = new();

    public ResultCircuitBreaker(string operationName, CircuitBreakerSettings? settings = null)
    {
        _operationName = operationName;
        _settings = settings ?? new CircuitBreakerSettings();
        _state = CircuitState.Closed;
    }

    public Result<T> Execute<T>(Func<Result<T>> operation)
    {
        lock (_lock)
        {
            if (_state == CircuitState.Open)
            {
                if (DateTime.UtcNow - _lastFailureTime >= _settings.ResetTimeout)
                {
                    _state = CircuitState.HalfOpen;
                    _halfOpenSuccessCount = 0;
                }
                else
                {
                    return Result<T>.Failure(
                        Error.Create(
                            ErrorCode.CircuitBreakerOpen,
                            $"Circuit breaker is open for operation: {_operationName}"
                        )
                    );
                }
            }
        }

        try
        {
            var result = operation();

            lock (_lock)
            {
                if (result.IsSuccess)
                {
                    if (_state == CircuitState.HalfOpen)
                    {
                        _halfOpenSuccessCount++;
                        if (_halfOpenSuccessCount >= _settings.HalfOpenSuccessThreshold)
                        {
                            _state = CircuitState.Closed;
                            _failureCount = 0;
                        }
                    }
                }
                else
                {
                    HandleFailure();
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            HandleFailure();
            return Result<T>.Failure(
                Error.Create(
                    ErrorCode.CircuitBreakerError,
                    $"Operation failed: {ex.Message}"
                )
            );
        }
    }

    public async Task<Result<T>> ExecuteAsync<T>(Func<Task<Result<T>>> operation)
    {
        lock (_lock)
        {
            if (_state == CircuitState.Open)
            {
                if (DateTime.UtcNow - _lastFailureTime >= _settings.ResetTimeout)
                {
                    _state = CircuitState.HalfOpen;
                    _halfOpenSuccessCount = 0;
                }
                else
                {
                    return Result<T>.Failure(
                        Error.Create(
                            ErrorCode.CircuitBreakerOpen,
                            $"Circuit breaker is open for operation: {_operationName}"
                        )
                    );
                }
            }
        }

        try
        {
            var result = await operation();

            lock (_lock)
            {
                if (result.IsSuccess)
                {
                    if (_state == CircuitState.HalfOpen)
                    {
                        _halfOpenSuccessCount++;
                        if (_halfOpenSuccessCount >= _settings.HalfOpenSuccessThreshold)
                        {
                            _state = CircuitState.Closed;
                            _failureCount = 0;
                        }
                    }
                }
                else
                {
                    HandleFailure();
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            HandleFailure();
            return Result<T>.Failure(
                Error.Create(
                    ErrorCode.CircuitBreakerError,
                    $"Operation failed: {ex.Message}"
                )
            );
        }
    }

    private void HandleFailure()
    {
        _failureCount++;
        _lastFailureTime = DateTime.UtcNow;

        if (_failureCount >= _settings.FailureThreshold)
        {
            _state = CircuitState.Open;
        }
    }

    public CircuitState GetState() => _state;
    public int GetFailureCount() => _failureCount;
    public DateTime GetLastFailureTime() => _lastFailureTime;
}