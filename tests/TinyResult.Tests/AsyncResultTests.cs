using Xunit;
using TinyResult.Enums;

namespace TinyResult.Tests;

public class AsyncResultTests
{
    [Fact]
    public async Task FromTryAsync_WithSuccess_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = await Result<int>.FromTryAsync(
            async () =>
            {
                await Task.Delay(1);
                return 42;
            },
            ex => new Error(ErrorCode.InvalidOperation, ex.Message)
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value!);
    }

    [Fact]
    public async Task FromTryAsync_WithException_ShouldReturnFailureResult()
    {
        // Arrange & Act
        var result = await Result<int>.FromTryAsync(
            async () =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("Test error");
            },
            ex => new Error(ErrorCode.InvalidOperation, ex.Message)
        );

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Test error", result.Error!.Message);
    }

    [Fact]
    public async Task MapAsync_WithSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result<int>.Success(42);

        // Act
        var mapped = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x.ToString();
        });

        // Assert
        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value!);
    }

    [Fact]
    public async Task MapAsync_WithFailure_ShouldPreserveError()
    {
        // Arrange
        var error = new Error(ErrorCode.InvalidOperation, "Test error");
        var result = Result<int>.Failure(error);

        // Act
        var mapped = await result.MapAsync(async x =>
        {
            await Task.Delay(1);
            return x.ToString();
        });

        // Assert
        Assert.True(mapped.IsFailure);
        Assert.Equal(error, mapped.Error!);
    }
} 