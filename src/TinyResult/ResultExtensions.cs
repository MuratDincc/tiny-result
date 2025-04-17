using TinyResult.Enums;

namespace TinyResult;

/// <summary>
/// Provides extension methods for the <see cref="Result{T}"/> class.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a value to a successful result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>A successful result containing the value.</returns>
    public static Result<T> ToResult<T>(this T value) => Result<T>.Success(value);

    /// <summary>
    /// Converts a nullable value to a result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The nullable value to convert.</param>
    /// <param name="errorMessage">The error message to use if the value is null.</param>
    /// <returns>A successful result containing the value, or a failed result if the value is null.</returns>
    public static Result<T> ToResult<T>(this T? value, string errorMessage) where T : class
        => value is null ? Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, errorMessage)) : Result<T>.Success(value);

    /// <summary>
    /// Converts a nullable value to a result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The nullable value to convert.</param>
    /// <param name="errorMessage">The error message to use if the value is null.</param>
    /// <returns>A successful result containing the value, or a failed result if the value is null.</returns>
    public static Result<T> ToResult<T>(this T? value, string errorMessage) where T : struct
        => value is null ? Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, errorMessage)) : Result<T>.Success(value.Value);

    /// <summary>
    /// Executes an action on the value of a successful result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
        => result.OnSuccess(action);

    /// <summary>
    /// Executes an action on the error of a failed result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    public static Result<T> TapError<T>(this Result<T> result, Action<Error> action)
        => result.OnFailure(action);

    /// <summary>
    /// Transforms the value of a successful result using the specified function.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to transform the value.</param>
    /// <returns>A new result with the transformed value or the original error.</returns>
    public static Result<TResult> Select<T, TResult>(this Result<T> result, Func<T, TResult> func)
        => result.Map(func);

    /// <summary>
    /// Binds the result to another result using the specified function.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the bound result.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to bind the result.</param>
    /// <returns>The bound result or the original error.</returns>
    public static Result<TResult> SelectMany<T, TResult>(this Result<T> result, Func<T, Result<TResult>> func)
        => result.Bind(func);

    /// <summary>
    /// Combines two results using the specified function.
    /// </summary>
    /// <typeparam name="T1">The type of the first result.</typeparam>
    /// <typeparam name="T2">The type of the second result.</typeparam>
    /// <typeparam name="TResult">The type of the combined result.</typeparam>
    /// <param name="result1">The first result.</param>
    /// <param name="result2">The second result.</param>
    /// <param name="func">The function to combine the values.</param>
    /// <returns>A new result with the combined value or the first error encountered.</returns>
    public static Result<TResult> SelectMany<T1, T2, TResult>(
        this Result<T1> result1,
        Func<T1, Result<T2>> result2,
        Func<T1, T2, TResult> func)
    {
        return result1.Bind(value1 => result2(value1).Map(value2 => func(value1, value2)));
    }

    /// <summary>
    /// Filters the result using the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="predicate">The predicate to filter the value.</param>
    /// <param name="errorMessage">The error message to use if the predicate fails.</param>
    /// <returns>The original result if the predicate passes, or a failed result if it fails.</returns>
    public static Result<T> Where<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
        => result.Filter(predicate, Error.Create(ErrorCode.InvalidOperation, errorMessage));

    /// <summary>
    /// Filters the result using the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="predicate">The predicate to filter the value.</param>
    /// <param name="error">The error to use if the predicate fails.</param>
    /// <returns>The original result if the predicate passes, or a failed result if it fails.</returns>
    public static Result<T> Where<T>(this Result<T> result, Func<T, bool> predicate, Error error)
        => result.Filter(predicate, error);

    /// <summary>
    /// Validates the result using the specified validator.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="validator">The validator to use.</param>
    /// <param name="error">The error to use if validation fails.</param>
    /// <returns>The original result if validation passes, or a failed result if it fails.</returns>
    public static Result<T> Validate<T>(this Result<T> result, Func<T, bool> validator, Error error)
    {
        if (result.IsSuccess && !validator(result.Value!))
        {
            return Result<T>.Failure(error);
        }
        return result;
    }

    /// <summary>
    /// Validates the result using the specified validator.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="validator">The validator to use.</param>
    /// <param name="errorMessage">The error message to use if validation fails.</param>
    /// <returns>The original result if validation passes, or a failed result if it fails.</returns>
    public static Result<T> Validate<T>(this Result<T> result, Func<T, bool> validator, string errorMessage)
    {
        if (result.IsSuccess && !validator(result.Value!))
        {
            return Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, errorMessage));
        }
        return result;
    }

    /// <summary>
    /// Transforms the error of a failed result using the specified function.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="errorTransformer">The function to transform the error.</param>
    /// <returns>A new result with the transformed error or the original value.</returns>
    public static Result<T> TransformError<T>(this Result<T> result, Func<Error, string> errorTransformer)
        => result.IsSuccess ? result : Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, errorTransformer(result.Error!)));

    /// <summary>
    /// Gets the value of the result if it is successful, or the default value if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="defaultValue">The default value to return if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the default value if it has failed.</returns>
    public static T GetValueOrDefault<T>(this Result<T> result, T defaultValue = default!)
        => result.IsSuccess ? result.Value! : defaultValue;

    /// <summary>
    /// Gets the value of the result if it is successful, or throws an exception if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The value of the result if it is successful.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the result has failed.</exception>
    public static T GetValueOrThrow<T>(this Result<T> result)
        => result.IsSuccess ? result.Value! : throw new InvalidOperationException(result.Error!.ToString());

    /// <summary>
    /// Gets the value of the result if it is successful, or throws the specified exception if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="exceptionFactory">The factory to create the exception.</param>
    /// <returns>The value of the result if it is successful.</returns>
    /// <exception cref="System.Exception">The base type for all exceptions.</exception>
    public static T GetValueOrThrow<T, TException>(this Result<T> result, Func<Error, TException> exceptionFactory)
        where TException : Exception
        => result.IsSuccess ? result.Value! : throw exceptionFactory(result.Error!);

    /// <summary>
    /// Gets the value of the result if it is successful, or null if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>The value of the result if it is successful, or null if it has failed.</returns>
    public static T? GetValueOrNull<T>(this Result<T> result)
        => result.IsSuccess ? result.Value! : default;

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified function if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified function if it has failed.</returns>
    public static T GetValueOr<T>(this Result<T> result, Func<Error, T> func)
        => result.IsSuccess ? result.Value! : func(result.Error!);

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified async function if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The async function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified async function if it has failed.</returns>
    public static async Task<T> GetValueOrAsync<T>(this Result<T> result, Func<Error, Task<T>> func)
        => result.IsSuccess ? result.Value! : await func(result.Error!);

    /// <summary>
    /// Gets the value of the result if it is successful, or the specified value if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="value">The value to return if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the specified value if it has failed.</returns>
    public static T GetValueOr<T>(this Result<T> result, T value)
        => result.IsSuccess ? result.Value! : value;

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified function if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified function if it has failed.</returns>
    public static T GetValueOr<T>(this Result<T> result, Func<T> func)
        => result.IsSuccess ? result.Value! : func();

    /// <summary>
    /// Gets the value of the result if it is successful, or the result of the specified async function if it has failed.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="func">The async function to execute if the result has failed.</param>
    /// <returns>The value of the result if it is successful, or the result of the specified async function if it has failed.</returns>
    public static async Task<T> GetValueOrAsync<T>(this Result<T> result, Func<Task<T>> func)
        => result.IsSuccess ? result.Value! : await func();

    public static Result<T> ToResult<T>(this T value, Func<T, bool> predicate, string errorMessage)
    {
        return predicate(value) 
            ? Result<T>.Success(value) 
            : Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, errorMessage));
    }

    public static Result<T> ToResult<T>(this T value, Func<T, bool> predicate, ErrorCode errorCode, string errorMessage)
    {
        return predicate(value) 
            ? Result<T>.Success(value) 
            : Result<T>.Failure(Error.Create(errorCode, errorMessage));
    }

    public static Result<T> ToResult<T>(this T value, Func<T, bool> predicate, Error error)
    {
        return predicate(value) 
            ? Result<T>.Success(value) 
            : Result<T>.Failure(error);
    }

    public static async Task<Result<T>> ToResultAsync<T>(this Task<T> task)
    {
        try
        {
            return Result<T>.Success(await task);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(Error.FromException(ex));
        }
    }

    public static async Task<Result<T>> ToResultAsync<T>(this Task<T> task, Func<Exception, Error> errorFactory)
    {
        try
        {
            return Result<T>.Success(await task);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(errorFactory(ex));
        }
    }

    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
    {
        return result.IsSuccess && !predicate(result.Value!) 
            ? Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, errorMessage)) 
            : result;
    }

    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, ErrorCode errorCode, string errorMessage)
    {
        return result.IsSuccess && !predicate(result.Value!) 
            ? Result<T>.Failure(Error.Create(errorCode, errorMessage)) 
            : result;
    }

    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        return result.IsSuccess && !predicate(result.Value!) 
            ? Result<T>.Failure(error) 
            : result;
    }

    public static Result<T> Filter<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
    {
        return result.IsSuccess && !predicate(result.Value!) 
            ? Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, errorMessage)) 
            : result;
    }

    public static Result<T> Filter<T>(this Result<T> result, Func<T, bool> predicate, ErrorCode errorCode, string errorMessage)
    {
        return result.IsSuccess && !predicate(result.Value!) 
            ? Result<T>.Failure(Error.Create(errorCode, errorMessage)) 
            : result;
    }

    public static Result<T> Filter<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        return result.IsSuccess && !predicate(result.Value!) 
            ? Result<T>.Failure(error) 
            : result;
    }

    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value!);
        }
        return result;
    }

    public static Result<T> OnFailure<T>(this Result<T> result, Action<Error> action)
    {
        if (result.IsFailure)
        {
            action(result.Error!);
        }
        return result;
    }

    public static Result<T> Catch<T>(this Result<T> result, Func<Error, Result<T>> handler)
    {
        return result.IsFailure 
            ? handler(result.Error!) 
            : result;
    }
} 