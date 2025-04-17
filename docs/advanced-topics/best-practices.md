# Best Practices

## Overview

This section contains recommended best practices for using the TinyResult library.

## Return Values

### Always Return Results
```csharp
// Bad
public User GetUser(int id)
{
    var user = _repository.GetUser(id);
    if (user == null)
        throw new UserNotFoundException();
    return user;
}

// Good
public Result<User> GetUser(int id)
{
    var user = _repository.GetUser(id);
    if (user == null)
        return Result<User>.Failure("User not found");
    return Result<User>.Success(user);
}
```

### Convert Exceptions to Results
```csharp
// Bad
public Result<User> GetUser(int id)
{
    try
    {
        var user = _repository.GetUser(id);
        return Result<User>.Success(user);
    }
    catch (Exception ex)
    {
        throw;
    }
}

// Good
public Result<User> GetUser(int id)
{
    return Result<User>.Try(() => _repository.GetUser(id));
}
```

## Error Handling

### Use Custom Error Codes
```csharp
// Bad
return Result<User>.Failure("User not found");

// Good
return Result<User>.Failure(ErrorCode.NotFound, "User not found");
```

### Use Error Metadata
```csharp
// Bad
return Result<User>.Failure("Invalid input");

// Good
return Result<User>.Failure(
    ErrorCode.ValidationError,
    "Invalid input",
    new Dictionary<string, object>
    {
        { "Field", "Email" },
        { "Value", email }
    }
);
```

## Validation

### Combine Validation Results
```csharp
public ValidationResult ValidateUser(User user)
{
    return ValidationResult.Create()
        .AddError("Name", "Name is required")
        .Combine(ValidateEmail(user.Email))
        .Combine(ValidatePassword(user.Password));
}
```

### Keep Validation Rules Separate
```csharp
private ValidationResult ValidateEmail(string email)
{
    var result = ValidationResult.Create();
    
    if (string.IsNullOrEmpty(email))
        result = result.AddError("Email", "Email is required");
    else if (!IsValidEmail(email))
        result = result.AddError("Email", "Email is invalid");
        
    return result;
}
```

## Async Operations

### Use Async Methods Correctly
```csharp
// Bad
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = await _repository.GetUserAsync(id);
    if (user == null)
        return Result<User>.Failure("User not found");
    return Result<User>.Success(user);
}

// Good
public Task<Result<User>> GetUserAsync(int id)
{
    return Result<User>.TryAsync(() => _repository.GetUserAsync(id));
}
```

### Use Async Chaining
```csharp
public async Task<Result<User>> ProcessUserAsync(int id)
{
    return await Result<User>.TryAsync(() => _repository.GetUserAsync(id))
        .BindAsync(user => UpdateUserAsync(user))
        .MapAsync(user => TransformUser(user));
}
```

## Performance

### Avoid Unnecessary Result Transformations
```csharp
// Bad
return Result<User>.Success(user).Map(u => u.Name);

// Good
return Result<string>.Success(user.Name);
```

### Use Large Objects Carefully
```csharp
// Bad
return Result<byte[]>.Success(largeFile);

// Good
return Result<Stream>.Success(fileStream);
```

## Testing

### Test Results Correctly
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

## Next Steps

- [Performance](performance.md)
- [Testing](testing.md)
- [Integration](integration.md) 