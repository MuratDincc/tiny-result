using TinyResult.Enums;

namespace TinyResult;

/// <summary>
/// Provides a fluent interface for chaining operations on a Result.
/// </summary>
/// <typeparam name="T">The type of the value that the result can hold on success.</typeparam>
public class ResultPipeline<T>
{
    private readonly Result<T> _result;
    private readonly TimeSpan? _timeout;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultPipeline{T}"/> class.
    /// </summary>
    /// <param name="result">The initial result.</param>
    /// <param name="timeout">The timeout for the operation.</param>
    /// <param name="retryCount">The number of retries for failed operations.</param>
    /// <param name="retryDelay">The delay between retries.</param>
    private ResultPipeline(Result<T> result, TimeSpan? timeout = null, int retryCount = 0, TimeSpan retryDelay = default)
    {
        _result = result;
        _timeout = timeout;
        _retryCount = retryCount;
        _retryDelay = retryDelay;
    }

    /// <summary>
    /// Creates a new pipeline with a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value to start the pipeline with.</param>
    /// <returns>A new pipeline instance.</returns>
    public static ResultPipeline<T> Start(T value)
    {
        return new ResultPipeline<T>(Result<T>.Success(value));
    }

    /// <summary>
    /// Creates a new pipeline with the specified result.
    /// </summary>
    /// <param name="result">The result to start the pipeline with.</param>
    /// <returns>A new pipeline instance.</returns>
    public static ResultPipeline<T> Start(Result<T> result)
    {
        return new ResultPipeline<T>(result);
    }

    /// <summary>
    /// Transforms the value of a successful result using the specified function.
    /// </summary>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="mapper">The function to transform the value.</param>
    /// <returns>A new pipeline with the transformed value or the original error.</returns>
    public ResultPipeline<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return new ResultPipeline<TResult>(_result.Map(mapper), _timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Binds the result to another result using the specified function.
    /// </summary>
    /// <typeparam name="TResult">The type of the bound result.</typeparam>
    /// <param name="binder">The function to bind the result.</param>
    /// <returns>A new pipeline with the bound result or the original error.</returns>
    public ResultPipeline<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        return new ResultPipeline<TResult>(_result.Bind(binder), _timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Binds the result to another result of the same type using the specified function.
    /// </summary>
    /// <param name="binder">The function to bind the result.</param>
    /// <returns>A new pipeline with the bound result or the original error.</returns>
    public ResultPipeline<T> Then(Func<T, Result<T>> binder)
    {
        return new ResultPipeline<T>(_result.Bind(binder), _timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Validates the result using the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to validate the value.</param>
    /// <param name="code">The error code to use if validation fails.</param>
    /// <param name="message">The error message to use if validation fails.</param>
    /// <returns>A new pipeline with the validated result or a failed result if validation fails.</returns>
    public ResultPipeline<T> Validate(Func<T, bool> predicate, ErrorCode code, string message)
    {
        return new ResultPipeline<T>(_result.Validate(predicate, code, message), _timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original pipeline.</returns>
    public ResultPipeline<T> OnSuccess(Action<T> action)
    {
        return new ResultPipeline<T>(_result.OnSuccess(action), _timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Executes an action if the result has failed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original pipeline.</returns>
    public ResultPipeline<T> OnFailure(Action<Error> action)
    {
        return new ResultPipeline<T>(_result.OnFailure(action), _timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Handles errors by transforming them into a new result.
    /// </summary>
    /// <param name="handler">The function to handle the error.</param>
    /// <returns>A new pipeline with the handled result.</returns>
    public ResultPipeline<T> Catch(Func<Error, Result<T>> handler)
    {
        return new ResultPipeline<T>(_result.Catch(handler), _timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Sets a timeout for the operation.
    /// </summary>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>A new pipeline with the timeout set.</returns>
    public ResultPipeline<T> WithTimeout(TimeSpan timeout)
    {
        return new ResultPipeline<T>(_result, timeout, _retryCount, _retryDelay);
    }

    /// <summary>
    /// Sets retry parameters for failed operations.
    /// </summary>
    /// <param name="retryCount">The number of retries.</param>
    /// <param name="retryDelay">The delay between retries.</param>
    /// <returns>A new pipeline with the retry parameters set.</returns>
    public ResultPipeline<T> WithRetry(int retryCount, TimeSpan retryDelay)
    {
        return new ResultPipeline<T>(_result, _timeout, retryCount, retryDelay);
    }

    /// <summary>
    /// Builds the pipeline, applying timeout and retry logic if specified.
    /// </summary>
    /// <returns>The final result after applying all operations.</returns>
    public Result<T> Build()
    {
        if (_timeout.HasValue)
        {
            var task = Task.Run(() => _result);
            if (!task.Wait(_timeout.Value))
            {
                return Result<T>.Failure(Error.Create(ErrorCode.Timeout, "Operation timed out"));
            }
            return task.Result;
        }

        if (_retryCount > 0)
        {
            var currentRetry = 0;
            while (currentRetry < _retryCount)
            {
                if (_result.IsSuccess)
                {
                    return _result;
                }

                currentRetry++;
                if (currentRetry < _retryCount)
                {
                    Thread.Sleep(_retryDelay);
                }
            }
        }

        return _result;
    }

    /// <summary>
    /// Ends the pipeline and returns the current result.
    /// </summary>
    /// <returns>The current result.</returns>
    public Result<T> End()
    {
        return _result;
    }
} 