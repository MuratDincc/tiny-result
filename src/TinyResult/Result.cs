using TinyResult.Enums;

namespace TinyResult;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// This is a generic class that can hold a value of type T on success or an Error on failure.
/// </summary>
/// <typeparam name="T">The type of the value that the result can hold on success.</typeparam>
public sealed class Result<T>
{
    public Result(T? value, Error? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        _isSuccess = isSuccess;
    }
    
    public T? Value { get; }
    
    public Error? Error { get; }
    
    private readonly bool _isSuccess;

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    private Result(T value)
    {
        Value = value;
        Error = null;
        _isSuccess = true;
    }

    private Result(Error error)
    {
        Value = default;
        Error = error;
        _isSuccess = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="value">The value to hold on success.</param>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="error">The error to hold on failure.</param>
    public static Result<T> Failure(Error error) => new(error);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new failed <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Failure(ErrorCode code, string message) => new(Error.Create(code, message));

    /// <summary>
    /// Creates a result from a function that might throw an exception.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <param name="errorFactory">The function to create an error message from an exception.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromTry(Func<T> func, Func<Exception, Error> errorFactory)
    {
        try
        {
            return Success(func());
        }
        catch (Exception ex)
        {
            return Failure(errorFactory(ex));
        }
    }

    /// <summary>
    /// Creates a result from an async function that might throw an exception.
    /// </summary>
    /// <param name="func">The async function to execute.</param>
    /// <param name="errorFactory">The function to create an error message from an exception.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static async Task<Result<T>> FromTryAsync(Func<Task<T>> func, Func<Exception, Error> errorFactory)
    {
        try
        {
            return Success(await func());
        }
        catch (Exception ex)
        {
            return Failure(errorFactory(ex));
        }
    }

    /// <summary>
    /// Creates a successful result asynchronously.
    /// </summary>
    /// <param name="value">The value to hold.</param>
    /// <returns>A new successful <see cref="Result{T}"/> instance.</returns>
    public static async Task<Result<T>> SuccessAsync(T value) => await Task.FromResult(Success(value));

    /// <summary>
    /// Creates a failed result asynchronously with the specified error message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new failed <see cref="Result{T}"/> instance.</returns>
    public static async Task<Result<T>> FailureAsync(ErrorCode code, string message) => await Task.FromResult(Failure(code, message));

    /// <summary>
    /// Matches the result to one of two functions based on whether it is successful or not.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the match.</typeparam>
    /// <param name="success">The function to execute if the result is successful.</param>
    /// <param name="failure">The function to execute if the result has failed.</param>
    /// <returns>The result of the matched function.</returns>
    public TResult Match<TResult>(Func<T, TResult> success, Func<Error, TResult> failure)
    {
        return _isSuccess 
            ? success(Value!) 
            : failure(Error!);
    }

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    public Result<T> OnSuccess(Action<T> action)
    {
        if (_isSuccess)
        {
            action(Value!);
        }
        return this;
    }

    /// <summary>
    /// Executes an action if the result has failed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    public Result<T> OnFailure(Action<Error> action)
    {
        if (!_isSuccess)
        {
            action(Error!);
        }
        return this;
    }

    /// <summary>
    /// Transforms the value of a successful result using the specified function.
    /// </summary>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="mapper">The function to transform the value.</param>
    /// <returns>A new result with the transformed value or the original error.</returns>
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return _isSuccess 
            ? Result<TResult>.Success(mapper(Value!)) 
            : Result<TResult>.Failure(Error!);
    }

    /// <summary>
    /// Transforms the value of a successful result using the specified async function.
    /// </summary>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="mapper">The async function to transform the value.</param>
    /// <returns>A new result with the transformed value or the original error.</returns>
    public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> mapper)
    {
        return _isSuccess 
            ? Result<TResult>.Success(await mapper(Value!)) 
            : Result<TResult>.Failure(Error!);
    }

    /// <summary>
    /// Binds the result to another result using the specified function.
    /// </summary>
    /// <typeparam name="TResult">The type of the bound result.</typeparam>
    /// <param name="binder">The function to bind the result.</param>
    /// <returns>The bound result or the original error.</returns>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        return _isSuccess 
            ? binder(Value!) 
            : Result<TResult>.Failure(Error!);
    }

    /// <summary>
    /// Binds the result to another result using the specified async function.
    /// </summary>
    /// <typeparam name="TResult">The type of the bound result.</typeparam>
    /// <param name="binder">The async function to bind the result.</param>
    /// <returns>The bound result or the original error.</returns>
    public async Task<Result<TResult>> BindAsync<TResult>(Func<T, Task<Result<TResult>>> binder)
    {
        return _isSuccess 
            ? await binder(Value!) 
            : Result<TResult>.Failure(Error!);
    }

    /// <summary>
    /// Filters the result using the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the value.</param>
    /// <param name="errorCode">The error code to use if the predicate fails.</param>
    /// <param name="errorMessage">The error message to use if the predicate fails.</param>
    /// <returns>The original result if the predicate passes, or a failed result if it fails.</returns>
    public Result<T> Validate(Func<T, bool> predicate, ErrorCode errorCode, string errorMessage)
    {
        return _isSuccess && !predicate(Value!) 
            ? Failure(errorCode, errorMessage) 
            : this;
    }

    /// <summary>
    /// Filters the result using the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the value.</param>
    /// <param name="errorCode">The error code to use if the predicate fails.</param>
    /// <param name="errorMessage">The error message to use if the predicate fails.</param>
    /// <returns>The original result if the predicate passes, or a failed result if it fails.</returns>
    public Result<T> Where(Func<T, bool> predicate, ErrorCode errorCode, string errorMessage)
    {
        return _isSuccess && !predicate(Value!) 
            ? Failure(errorCode, errorMessage) 
            : this;
    }

    /// <summary>
    /// Catches exceptions and returns a failed result.
    /// </summary>
    /// <param name="handler">The function to handle the exception.</param>
    /// <returns>A new failed <see cref="Result{T}"/> instance.</returns>
    public Result<T> Catch(Func<Error, Result<T>> handler)
    {
        return !_isSuccess 
            ? handler(Error!) 
            : this;
    }

    /// <summary>
    /// Combines two results into a single result.
    /// </summary>
    /// <typeparam name="T2">The type of the second result.</typeparam>
    /// <param name="result1">The first result.</param>
    /// <param name="result2">The second result.</param>
    /// <returns>A new result containing both values as a tuple if both results are successful, or the first error encountered.</returns>
    public static Result<(T, T2)> Combine<T2>(Result<T> result1, Result<T2> result2)
    {
        if (!result1._isSuccess) return Result<(T, T2)>.Failure(result1.Error!);
        if (!result2._isSuccess) return Result<(T, T2)>.Failure(result2.Error!);
        return Result<(T, T2)>.Success((result1.Value!, result2.Value!));
    }

    /// <summary>
    /// Combines multiple results into a single result.
    /// </summary>
    /// <param name="results">The results to combine.</param>
    /// <returns>A new result with all values or the first error encountered.</returns>
    public static Result<IEnumerable<T>> Combine(IEnumerable<Result<T>> results)
    {
        var resultList = results.ToList();
        var failures = resultList.Where(r => !r._isSuccess).ToList();

        if (failures.Any())
        {
            return Result<IEnumerable<T>>.Failure(failures.First().Error!);
        }

        return Result<IEnumerable<T>>.Success(resultList.Select(r => r.Value!));
    }

    /// <summary>
    /// Gets the value of the result if it is successful, or the default value if it has failed.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the default value if it has failed.</returns>
    public T GetValueOrDefault(T defaultValue = default!)
        => _isSuccess ? Value! : defaultValue;

    /// <summary>
    /// Gets the value of the result if it is successful, or throws an exception if it has failed.
    /// </summary>
    /// <returns>The value of the result if it is successful.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the result has failed.</exception>
    public T GetValueOrThrow()
        => _isSuccess ? Value! : throw new InvalidOperationException($"Result is not successful: {Error}");

    /// <summary>
    /// Gets the value of the result if it is successful, or throws the specified exception if it has failed.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="exceptionFactory">The factory to create the exception.</param>
    /// <returns>The value of the result if it is successful.</returns>
    /// <exception cref="System.Exception">The base type for all exceptions.</exception>
    public T GetValueOrThrow<TException>(Func<Error, TException> exceptionFactory) where TException : Exception
        => _isSuccess ? Value! : throw exceptionFactory(Error!);

    /// <summary>
    /// Gets the value of the result if it is successful, or null if it has failed.
    /// </summary>
    /// <returns>The value of the result if it is successful, or null if it has failed.</returns>
    public T? GetValueOrNull()
        => _isSuccess ? Value : default;

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified function if it has failed.
    /// </summary>
    /// <param name="func">The function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified function if it has failed.</returns>
    public T GetValueOr(Func<Error, T> func)
        => _isSuccess ? Value! : func(Error!);

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified async function if it has failed.
    /// </summary>
    /// <param name="func">The async function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified async function if it has failed.</returns>
    public async Task<T> GetValueOrAsync(Func<Error, Task<T>> func)
        => _isSuccess ? Value! : await func(Error!);

    /// <summary>
    /// Gets the value of the result if it is successful, or the specified value if it has failed.
    /// </summary>
    /// <param name="value">The value to return if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the specified value if it has failed.</returns>
    public T GetValueOr(T value)
        => _isSuccess ? Value! : value;

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified function if it has failed.
    /// </summary>
    /// <param name="func">The function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified function if it has failed.</returns>
    public T GetValueOr(Func<T> func)
        => _isSuccess ? Value! : func();

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified async function if it has failed.
    /// </summary>
    /// <param name="func">The async function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified async function if it has failed.</returns>
    public async Task<T> GetValueOrAsync(Func<Task<T>> func)
        => _isSuccess ? Value! : await func();

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string representation of the result.</returns>
    public override string ToString() => _isSuccess ? $"Success: {Value}" : $"Failure: {Error}";

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure(error);
} 