using Xunit;
using TinyResult.Enums;

namespace TinyResult.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");

        // Act
        var result = Result<int>.Failure(error);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Map_WithSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var mapped = result.Map(x => x.ToString());

        // Assert
        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public void Map_WithFailure_ShouldPreserveError()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");
        var result = Result<int>.Failure(error);

        // Act
        var mapped = result.Map(x => x.ToString());

        // Assert
        Assert.True(mapped.IsFailure);
        Assert.Equal(error, mapped.Error);
    }

    [Fact]
    public void Match_WithSuccess_ShouldExecuteSuccessFunc()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var matched = result.Match(
            success => success * 2,
            error => 0
        );

        // Assert
        Assert.Equal(84, matched);
    }

    [Fact]
    public void Match_WithFailure_ShouldExecuteFailureFunc()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");
        var result = Result<int>.Failure(error);

        // Act
        var matched = result.Match(
            success => success * 2,
            err => 0
        );

        // Assert
        Assert.Equal(0, matched);
    }

    [Fact]
    public void OnSuccess_WithSuccess_ShouldExecuteAction()
    {
        // Arrange
        var result = Result<int>.Success(42);
        var wasExecuted = false;

        // Act
        result.OnSuccess(_ => wasExecuted = true);

        // Assert
        Assert.True(wasExecuted);
    }

    [Fact]
    public void OnFailure_WithFailure_ShouldExecuteAction()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");
        var result = Result<int>.Failure(error);
        var wasExecuted = false;

        // Act
        result.OnFailure(_ => wasExecuted = true);

        // Assert
        Assert.True(wasExecuted);
    }

    [Fact]
    public async Task MapAsync_WithSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var mapped = await result.MapAsync(x => Task.FromResult(x.ToString()));

        // Assert
        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public void Combine_WithTwoSuccessResults_ShouldCombineValues()
    {
        // Arrange
        var result1 = Result<int>.Success(42);
        var result2 = Result<string>.Success("test");

        // Act
        var combined = Result<int>.Combine(result1, result2);

        // Assert
        Assert.True(combined.IsSuccess);
        Assert.Equal((42, "test"), combined.Value);
    }
} 