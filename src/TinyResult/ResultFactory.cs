using TinyResult.Enums;

namespace TinyResult;

/// <summary>
/// Provides factory methods for creating Result instances from various sources.
/// </summary>
public static class ResultFactory
{
    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to hold on success.</param>
    /// <returns>A new successful <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="error">The error to hold on failure.</param>
    /// <returns>A new failed <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);

    /// <summary>
    /// Creates a failed result with the specified error code and message.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A new failed <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Failure<T>(ErrorCode code, string message) => Result<T>.Failure(Error.Create(code, message));

    /// <summary>
    /// Creates a result from a function that might throw an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromTry<T>(Func<T> func)
    {
        try
        {
            return Success(func());
        }
        catch (Exception ex)
        {
            return Failure<T>(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Creates a result from an async function that might throw an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="func">The async function to execute.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static async Task<Result<T>> FromTryAsync<T>(Func<Task<T>> func)
    {
        try
        {
            return Success(await func());
        }
        catch (Exception ex)
        {
            return Failure<T>(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Creates a result from a function that might throw an exception, with a custom error factory.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="errorFactory">The function to create an error from an exception.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromTry<T>(Func<T> func, Func<Exception, Error> errorFactory)
    {
        try
        {
            return Success(func());
        }
        catch (Exception ex)
        {
            return Failure<T>(errorFactory(ex));
        }
    }

    /// <summary>
    /// Creates a result from an async function that might throw an exception, with a custom error factory.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="func">The async function to execute.</param>
    /// <param name="errorFactory">The function to create an error from an exception.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static async Task<Result<T>> FromTryAsync<T>(Func<Task<T>> func, Func<Exception, Error> errorFactory)
    {
        try
        {
            return Success(await func());
        }
        catch (Exception ex)
        {
            return Failure<T>(errorFactory(ex));
        }
    }

    /// <summary>
    /// Creates a failed result from an exception.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="exception">The exception to create the error from.</param>
    /// <returns>A new failed <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromException<T>(Exception exception)
    {
        return Failure<T>(Error.FromException(exception));
    }

    /// <summary>
    /// Creates a failed result from an exception with a specific error code.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="exception">The exception to create the error from.</param>
    /// <param name="code">The error code to use.</param>
    /// <returns>A new failed <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromException<T>(Exception exception, ErrorCode code)
    {
        return Failure<T>(Error.FromException(exception, code));
    }

    /// <summary>
    /// Creates a result from an HTTP response.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="response">The HTTP response.</param>
    /// <param name="deserializer">The function to deserialize the response content.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromHttpResponse<T>(HttpResponseMessage response, Func<string, T> deserializer)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            return Result<T>.Success(deserializer(content));
        }

        return Result<T>.Failure(
            Error.Create(
                ErrorCode.NetworkError,
                $"HTTP request failed with status code: {response.StatusCode}",
                new Dictionary<string, object> { { "StatusCode", response.StatusCode } }
            )
        );
    }

    /// <summary>
    /// Creates a result from a database operation.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="operation">The database operation to execute.</param>
    /// <param name="operationName">The name of the operation for error reporting.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromDatabaseOperation<T>(Func<T> operation, string operationName)
    {
        try
        {
            return Result<T>.Success(operation());
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(
                Error.Create(
                    ErrorCode.DatabaseError,
                    $"Database operation failed: {operationName}",
                    new Dictionary<string, object> { { "Exception", ex } }
                )
            );
        }
    }

    /// <summary>
    /// Creates a result from a file operation.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="reader">The function to read and parse the file content.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromFileOperation<T>(string filePath, Func<string, T> reader)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return Result<T>.Failure(
                    Error.Create(
                        ErrorCode.NotFound,
                        $"File not found: {filePath}"
                    )
                );
            }

            var content = File.ReadAllText(filePath);
            return Result<T>.Success(reader(content));
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(
                Error.Create(
                    ErrorCode.InvalidOperation,
                    $"File operation failed: {filePath}",
                    new Dictionary<string, object> { { "Exception", ex } }
                )
            );
        }
    }

    /// <summary>
    /// Creates a result from an API operation.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="url">The API URL.</param>
    /// <param name="deserializer">The function to deserialize the API response.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromApiOperation<T>(string url, Func<string, T> deserializer)
    {
        try
        {
            using var client = new HttpClient();
            var response = client.GetAsync(url).Result;
            return FromHttpResponse(response, deserializer);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(
                Error.Create(
                    ErrorCode.NetworkError,
                    $"API operation failed: {url}",
                    new Dictionary<string, object> { { "Exception", ex } }
                )
            );
        }
    }

    /// <summary>
    /// Creates a result from an XML string.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="xml">The XML string to deserialize.</param>
    /// <returns>A new <see cref="Result{T}"/> instance.</returns>
    public static Result<T> FromXml<T>(string xml)
    {
        try
        {
            using var reader = new StringReader(xml);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var value = (T?)serializer.Deserialize(reader);
            return value is null
                ? Result<T>.Failure(Error.Create(ErrorCode.InvalidOperation, "Failed to deserialize XML"))
                : Result<T>.Success(value);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(
                Error.Create(
                    ErrorCode.InvalidOperation,
                    "XML deserialization failed",
                    new Dictionary<string, object> { { "Exception", ex } }
                )
            );
        }
    }
} 