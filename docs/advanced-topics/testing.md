# Testing

## Overview

This section covers testing techniques and best practices for working with the TinyResult library.

## Test Strategies

### Success Result Test
```csharp
[Fact]
public void GetUser_WhenUserExists_ReturnsSuccess()
{
    // Arrange
    var user = new User { Id = 1, Name = "Test" };
    _repository.Setup(r => r.GetUser(1)).Returns(user);

    // Act
    var result = _service.GetUser(1);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeEquivalentTo(user);
}
```

### Failure Result Test
```csharp
[Fact]
public void GetUser_WhenUserNotFound_ReturnsFailure()
{
    // Arrange
    _repository.Setup(r => r.GetUser(1)).Returns((User)null);

    // Act
    var result = _service.GetUser(1);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Message.Should().Be("User not found");
}
```

## Test Helpers

### Result Assertions
```csharp
public static class ResultAssertions
{
    public static void ShouldBeSuccess<T>(this Result<T> result, T expectedValue)
    {
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedValue);
    }

    public static void ShouldBeFailure<T>(this Result<T> result, string expectedMessage)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedMessage);
    }
}
```

### Test Data Creation
```csharp
public static class ResultTestData
{
    public static Result<User> CreateSuccessResult()
    {
        return Result<User>.Success(new User { Id = 1, Name = "Test" });
    }

    public static Result<User> CreateFailureResult()
    {
        return Result<User>.Failure("Test error");
    }
}
```

## Async Tests

### Async Success Test
```csharp
[Fact]
public async Task GetUserAsync_WhenUserExists_ReturnsSuccess()
{
    // Arrange
    var user = new User { Id = 1, Name = "Test" };
    _repository.Setup(r => r.GetUserAsync(1)).ReturnsAsync(user);

    // Act
    var result = await _service.GetUserAsync(1);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeEquivalentTo(user);
}
```

### Async Failure Test
```csharp
[Fact]
public async Task GetUserAsync_WhenRepositoryThrows_ReturnsFailure()
{
    // Arrange
    _repository.Setup(r => r.GetUserAsync(1))
        .ThrowsAsync(new Exception("Database error"));

    // Act
    var result = await _service.GetUserAsync(1);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Message.Should().Be("Database error");
}
```

## Validation Tests

### Validation Success Test
```csharp
[Fact]
public void ValidateUser_WhenValid_ReturnsSuccess()
{
    // Arrange
    var user = new User { Name = "Test", Email = "test@example.com" };

    // Act
    var result = _validator.Validate(user);

    // Assert
    result.IsValid.Should().BeTrue();
    result.Errors.Should().BeEmpty();
}
```

### Validation Failure Test
```csharp
[Fact]
public void ValidateUser_WhenInvalid_ReturnsErrors()
{
    // Arrange
    var user = new User { Name = "", Email = "invalid" };

    // Act
    var result = _validator.Validate(user);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().ContainKey("Name")
        .WhoseValue.Should().Be("Name is required");
    result.Errors.Should().ContainKey("Email")
        .WhoseValue.Should().Be("Email is invalid");
}
```

## Mock and Stub Usage

### Repository Mock
```csharp
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repository;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repository = new Mock<IUserRepository>();
        _service = new UserService(_repository.Object);
    }

    [Fact]
    public void GetUser_WhenUserExists_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Test" };
        _repository.Setup(r => r.GetUser(1)).Returns(user);

        // Act
        var result = _service.GetUser(1);

        // Assert
        result.ShouldBeSuccess(user);
        _repository.Verify(r => r.GetUser(1), Times.Once);
    }
}
```

### Validator Stub
```csharp
public class UserRegistrationServiceTests
{
    private readonly Mock<IUserValidator> _validator;
    private readonly UserRegistrationService _service;

    public UserRegistrationServiceTests()
    {
        _validator = new Mock<IUserValidator>();
        _service = new UserRegistrationService(_validator.Object);
    }

    [Fact]
    public void RegisterUser_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Name = "Test" };
        _validator.Setup(v => v.Validate(user))
            .Returns(ValidationResult.Create());

        // Act
        var result = _service.RegisterUser(user);

        // Assert
        result.ShouldBeSuccess();
    }
}
```

## Test Coverage

### Unit Test Coverage
- Result creation
- Result transformation
- Error handling
- Validation
- Async operations

### Integration Test Coverage
- Repository integration
- Service integration
- API integration
- Database integration

## Next Steps

- [Best Practices](best-practices.md)
- [Performance](performance.md)
- [Integration](integration.md) 