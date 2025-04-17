using Xunit;
using TinyResult.Enums;

namespace TinyResult.Tests;

public class ErrorTests
{
    [Fact]
    public void Create_WithCodeAndMessage_ShouldCreateError()
    {
        // Arrange & Act
        var error = new Error(ErrorCode.InvalidOperation, "Test error");

        // Assert
        Assert.Equal(ErrorCode.InvalidOperation, error.Code);
        Assert.Equal("Test error", error.Message);
        Assert.Empty(error.Metadata);
    }

    [Fact]
    public void Create_WithCodeMessageAndMetadata_ShouldCreateError()
    {
        // Arrange
        var metadata = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", 42 }
        };

        // Act
        var error = new Error(ErrorCode.InvalidOperation, "Test error", metadata);

        // Assert
        Assert.Equal(ErrorCode.InvalidOperation, error.Code);
        Assert.Equal("Test error", error.Message);
        Assert.Equal(metadata, error.Metadata);
    }

    [Fact]
    public void FromException_ShouldCreateErrorFromException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act
        var error = Error.FromException(exception);

        // Assert
        Assert.Equal(ErrorCode.Exception, error.Code);
        Assert.Equal("Test exception", error.Message);
        Assert.Contains("ExceptionType", error.Metadata.Keys);
        Assert.Contains("StackTrace", error.Metadata.Keys);
    }

    [Fact]
    public void WithMetadata_ShouldAddMetadata()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");
        var metadata = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", 42 }
        };

        // Act
        error = error.WithMetadata(metadata);

        // Assert
        Assert.Equal(metadata["key1"], error.Metadata["key1"]);
        Assert.Equal(metadata["key2"], error.Metadata["key2"]);
    }

    [Fact]
    public void WithMetadata_SingleEntry_ShouldAddMetadata()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");

        // Act
        error = error.WithMetadata("key", "value");

        // Assert
        Assert.Equal("value", error.Metadata["key"]);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");

        // Act
        var result = error.ToString();

        // Assert
        Assert.Equal("InvalidOperation: Test error", result);
    }
} 