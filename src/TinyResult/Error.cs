using System.Text.Json.Serialization;
using TinyResult.Enums;

namespace TinyResult;

/// <summary>
/// Represents an error that occurred during an operation.
/// Contains detailed information about the error including code, message, and metadata.
/// </summary>
public sealed class Error
{
    public Error(ErrorCode code, string message, IDictionary<string, object>? metadata = null)
    {
        Code = code;
        Message = message;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    [JsonPropertyName("code")]
    public ErrorCode Code { get; }

    [JsonPropertyName("message")]
    public string Message { get; }

    [JsonPropertyName("metadata")]
    public IDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Creates a new error with the specified error code and message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="metadata">Additional metadata about the error.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    public static Error Create(ErrorCode code, string message, IDictionary<string, object>? metadata = null)
    {
        return new Error(code, message, metadata);
    }

    /// <summary>
    /// Creates a new error from an exception.
    /// </summary>
    /// <param name="exception">The exception to create the error from.</param>
    /// <param name="code">The error code.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    public static Error FromException(Exception exception, ErrorCode? code = null)
    {
        var metadata = new Dictionary<string, object>
        {
            { "ExceptionType", exception.GetType().Name },
            { "StackTrace", exception.StackTrace ?? string.Empty }
        };

        if (exception.InnerException != null)
        {
            metadata["InnerException"] = exception.InnerException.Message;
        }

        return Create(
            code ?? ErrorCode.Exception,
            exception.Message,
            metadata
        );
    }

    /// <summary>
    /// Implicitly converts a string to an Error instance.
    /// </summary>
    /// <param name="message">The error message.</param>
    public static implicit operator Error(string message)
    {
        return Create(ErrorCode.InvalidOperation, message);
    }

    /// <summary>
    /// Adds a new metadata entry to the error.
    /// </summary>
    /// <param name="key">The key of the metadata entry.</param>
    /// <param name="value">The value of the metadata entry.</param>
    /// <returns>The updated <see cref="Error"/> instance.</returns>
    public Error WithMetadata(string key, object value)
    {
        Metadata[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple metadata entries to the error.
    /// </summary>
    /// <param name="metadata">The metadata entries to add.</param>
    /// <returns>The updated <see cref="Error"/> instance.</returns>
    public Error WithMetadata(IDictionary<string, object> metadata)
    {
        foreach (var kvp in metadata)
        {
            Metadata[kvp.Key] = kvp.Value;
        }
        return this;
    }

    public override string ToString() => $"{Code}: {Message}";
} 