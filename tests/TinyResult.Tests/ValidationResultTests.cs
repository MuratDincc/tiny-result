using Xunit;
using TinyResult.Enums;

namespace TinyResult.Tests;

public class ValidationResultTests
{
    [Fact]
    public void Create_ShouldReturnEmptyValidationResult()
    {
        // Act
        var result = ValidationResult.Create();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void AddError_ShouldAddErrorToResult()
    {
        // Arrange
        var result = ValidationResult.Create();

        // Act
        result.AddError("Field", "Error message");

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Error message", result.Errors["Field"]);
    }

    [Fact]
    public void AddErrors_ShouldAddMultipleErrors()
    {
        // Arrange
        var result = ValidationResult.Create();
        var errors = new Dictionary<string, string>
        {
            { "Field1", "Error 1" },
            { "Field2", "Error 2" }
        };

        // Act
        result.AddErrors(errors);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal("Error 1", result.Errors["Field1"]);
        Assert.Equal("Error 2", result.Errors["Field2"]);
    }

    [Fact]
    public void Clear_ShouldRemoveAllErrors()
    {
        // Arrange
        var result = ValidationResult.Create()
            .AddError("Field1", "Error 1")
            .AddError("Field2", "Error 2");

        // Act
        result.Clear();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Combine_WithMultipleResults_ShouldCombineErrors()
    {
        // Arrange
        var result1 = ValidationResult.Create().AddError("Field1", "Error 1");
        var result2 = ValidationResult.Create().AddError("Field2", "Error 2");

        // Act
        var combined = ValidationResult.Combine(result1, result2);

        // Assert
        Assert.False(combined.IsValid);
        Assert.Equal(2, combined.Errors.Count);
        Assert.Equal("Error 1", combined.Errors["Field1"]);
        Assert.Equal("Error 2", combined.Errors["Field2"]);
    }

    [Fact]
    public void Combine_WithEmptyResults_ShouldReturnValidResult()
    {
        // Arrange
        var result1 = ValidationResult.Create();
        var result2 = ValidationResult.Create();

        // Act
        var combined = ValidationResult.Combine(result1, result2);

        // Assert
        Assert.True(combined.IsValid);
        Assert.Empty(combined.Errors);
    }
} 