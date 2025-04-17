using Xunit;
using TinyResult.Enums;

namespace TinyResult.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void ToResult_ShouldConvertValueToSuccessResult()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = value.ToResult();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Tap_ShouldExecuteActionOnSuccess()
    {
        // Arrange
        var result = Result<int>.Success(42);
        var value = 0;

        // Act
        var newResult = result.Tap(x => value = x);

        // Assert
        Assert.True(newResult.IsSuccess);
        Assert.Equal(42, value);
        Assert.Equal(result, newResult);
    }

    [Fact]
    public void Tap_ShouldNotExecuteActionOnFailure()
    {
        // Arrange
        var result = Result<int>.Failure("Error");
        var value = 0;

        // Act
        var newResult = result.Tap(x => value = x);

        // Assert
        Assert.True(newResult.IsFailure);
        Assert.Equal(0, value);
        Assert.Equal(result, newResult);
    }

    [Fact]
    public void TapError_ShouldExecuteActionOnFailure()
    {
        // Arrange
        var result = Result<int>.Failure("Error");
        var error = string.Empty;

        // Act
        var newResult = result.TapError(e => error = e.Message);

        // Assert
        Assert.True(newResult.IsFailure);
        Assert.Equal("Error", error);
        Assert.Equal(result, newResult);
    }

    [Fact]
    public void TapError_ShouldNotExecuteActionOnSuccess()
    {
        // Arrange
        var result = Result<int>.Success(42);
        var error = string.Empty;

        // Act
        var newResult = result.TapError(e => error = e.Message);

        // Assert
        Assert.True(newResult.IsSuccess);
        Assert.Empty(error);
        Assert.Equal(result, newResult);
    }

    [Fact]
    public void Select_WithSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var selected = result.Select(x => x.ToString());

        // Assert
        Assert.True(selected.IsSuccess);
        Assert.NotNull(selected.Value);
        Assert.Equal("42", selected.Value);
    }

    [Fact]
    public void Select_WithFailure_ShouldReturnFailure()
    {
        // Arrange
        var result = Result<int>.Failure(ErrorCode.ValidationError, "Test error");

        // Act
        var selected = result.Select(x => x.ToString());

        // Assert
        Assert.False(selected.IsSuccess);
        Assert.NotNull(selected.Error);
        Assert.Equal(ErrorCode.ValidationError, selected.Error.Code);
        Assert.Equal("Test error", selected.Error.Message);
    }

    [Fact]
    public void SelectMany_ShouldTransformValueOnSuccess()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var newResult = result.SelectMany(x => Result<int>.Success(x * 2));

        // Assert
        Assert.True(newResult.IsSuccess);
        Assert.Equal(84, newResult.Value);
    }

    [Fact]
    public void SelectMany_ShouldPreserveErrorOnFailure()
    {
        // Arrange
        var result = Result<int>.Failure("Error");

        // Act
        var newResult = result.SelectMany(x => Result<int>.Success(x * 2));

        // Assert
        Assert.True(newResult.IsFailure);
        Assert.Equal("Error", newResult.Error?.Message);
    }

    [Fact]
    public void Where_WithSuccessAndValidPredicate_ShouldReturnSuccess()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var filtered = result.Where(x => x > 0, ErrorCode.ValidationError, "Value must be positive");

        // Assert
        Assert.True(filtered.IsSuccess);
        Assert.Equal(42, filtered.Value);
    }

    [Fact]
    public void Where_WithSuccessAndInvalidPredicate_ShouldReturnFailure()
    {
        // Arrange
        var result = Result<int>.Success(-1);

        // Act
        var filtered = result.Where(x => x > 0, ErrorCode.ValidationError, "Value must be positive");

        // Assert
        Assert.False(filtered.IsSuccess);
        Assert.NotNull(filtered.Error);
        Assert.Equal(ErrorCode.ValidationError, filtered.Error.Code);
        Assert.Equal("Value must be positive", filtered.Error.Message);
    }

    [Fact]
    public void Where_WithFailure_ShouldReturnFailure()
    {
        // Arrange
        var result = Result<int>.Failure(ErrorCode.ValidationError, "Test error");

        // Act
        var filtered = result.Where(x => x > 0, ErrorCode.ValidationError, "Value must be positive");

        // Assert
        Assert.False(filtered.IsSuccess);
        Assert.NotNull(filtered.Error);
        Assert.Equal(ErrorCode.ValidationError, filtered.Error.Code);
        Assert.Equal("Test error", filtered.Error.Message);
    }

    [Fact]
    public void Validate_WithValidValue_ShouldReturnSuccess()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var validated = result.Validate(x => x > 0, "Value must be positive");

        // Assert
        Assert.True(validated.IsSuccess);
        Assert.Equal(42, validated.Value);
    }

    [Fact]
    public void Validate_WithInvalidValue_ShouldReturnFailure()
    {
        // Arrange
        var result = Result<int>.Success(-1);

        // Act
        var validated = result.Validate(x => x > 0, "Value must be positive");

        // Assert
        Assert.True(validated.IsFailure);
        Assert.Equal("Value must be positive", validated.Error?.Message);
    }

    [Fact]
    public void Validate_WithFailure_ShouldPreserveError()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Original error");
        var result = Result<int>.Failure(error);

        // Act
        var validated = result.Validate(x => x > 0, "Value must be positive");

        // Assert
        Assert.True(validated.IsFailure);
        Assert.Equal(error, validated.Error);
    }

    [Fact]
    public void SelectMany_WithSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var transformed = result.SelectMany(x => Result<string>.Success(x.ToString()));

        // Assert
        Assert.True(transformed.IsSuccess);
        Assert.NotNull(transformed.Value);
        Assert.Equal("42", transformed.Value);
    }

    [Fact]
    public void SelectMany_WithFailure_ShouldReturnFailure()
    {
        // Arrange
        var result = Result<int>.Failure(ErrorCode.ValidationError, "Test error");

        // Act
        var transformed = result.SelectMany(x => Result<string>.Success(x.ToString()));

        // Assert
        Assert.False(transformed.IsSuccess);
        Assert.NotNull(transformed.Error);
        Assert.Equal(ErrorCode.ValidationError, transformed.Error.Code);
        Assert.Equal("Test error", transformed.Error.Message);
    }
} 