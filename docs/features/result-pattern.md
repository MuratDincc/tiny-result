# Result Pattern

## Overview

The Result Pattern is a functional programming concept that provides a clean and type-safe way to handle success and failure cases in your applications. TinyResult implements this pattern in a way that is both powerful and easy to use.

## Key Concepts

### 1. Result Type

The `Result<T>` type represents the outcome of an operation that can either succeed or fail:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public Error Error { get; }
}
```

### 2. Error Type

The `Error` type represents detailed information about a failure:

```csharp
public class Error
{
    public ErrorCode Code { get; }
    public string Message { get; }
    public IReadOnlyDictionary<string, object> Metadata { get; }
}
```

### 3. Error Codes

Predefined error codes for common failure scenarios:

```csharp
public enum ErrorCode
{
    Unknown,
    NotFound,
    ValidationError,
    Unauthorized,
    InvalidOperation,
    NetworkError,
    Timeout,
    ConfigurationError,
    DatabaseError
}
```

## Core Operations

### 1. Creating Results

```csharp
// Success
var success = Result<int>.Success(42);

// Failure with message
var failure = Result<int>.Failure("Something went wrong");

// Failure with error code
var failure = Result<int>.Failure(ErrorCode.NotFound, "User not found");

// Failure with custom error
var failure = Result<int>.Failure(new Error(ErrorCode.ValidationError, "Invalid input"));
```

### 2. Transforming Results

```csharp
// Map: Transform success value
var result = success.Map(x => x * 2);

// Bind: Chain operations that return results
var result = success.Bind(x => GetUser(x));

// Filter: Validate success value
var result = success.Filter(x => x > 0, "Value must be positive");
```

### 3. Handling Results

```csharp
// Match: Handle both success and failure
result.Match(
    value => Console.WriteLine($"Success: {value}"),
    error => Console.WriteLine($"Error: {error.Message}")
);

// OnSuccess: Handle success only
result.OnSuccess(value => Console.WriteLine($"Success: {value}"));

// OnFailure: Handle failure only
result.OnFailure(error => Console.WriteLine($"Error: {error.Message}"));
```

## Advanced Features

### 1. Result Pipelines

Chain multiple operations together:

```csharp
var result = GetUser(1)
    .Map(user => user.Name)
    .Map(name => name.ToUpper())
    .Filter(name => name.Length > 0, "Name cannot be empty")
    .OnSuccess(name => Console.WriteLine($"Name: {name}"))
    .OnFailure(error => Console.WriteLine($"Error: {error.Message}"));
```

### 2. Error Handling

Handle errors in a type-safe way:

```csharp
var result = GetUser(1);

if (result.IsFailure)
{
    switch (result.Error.Code)
    {
        case ErrorCode.NotFound:
            return Result<User>.Failure("User not found. Please try again.");
        case ErrorCode.ValidationError:
            return Result<User>.Failure("Invalid user data.");
        default:
            return Result<User>.Failure("An unexpected error occurred.");
    }
}
```

### 3. Result Aggregation

Combine multiple results:

```csharp
var results = new[]
{
    GetUser(1),
    GetUser(2),
    GetUser(3)
};

var combinedResult = Result.Combine(results);

combinedResult.Match(
    users => Console.WriteLine($"Found {users.Count()} users"),
    error => Console.WriteLine($"Error: {error.Message}")
);
```

## Best Practices

### 1. Use Results Instead of Exceptions

```csharp
// Avoid
public User GetUser(int id)
{
    var user = _repository.GetById(id);
    if (user == null)
    {
        throw new UserNotFoundException($"User {id} not found");
    }
    return user;
}

// Prefer
public Result<User> GetUser(int id)
{
    var user = _repository.GetById(id);
    return user != null
        ? Result<User>.Success(user)
        : Result<User>.Failure(ErrorCode.NotFound, $"User {id} not found");
}
```

### 2. Chain Operations

```csharp
// Avoid
var user = GetUser(1);
if (user.IsSuccess)
{
    var address = GetAddress(user.Value.Id);
    if (address.IsSuccess)
    {
        return address.Value;
    }
    return Result<Address>.Failure(address.Error);
}
return Result<Address>.Failure(user.Error);

// Prefer
return GetUser(1)
    .Bind(user => GetAddress(user.Id));
```

### 3. Use Descriptive Error Messages

```csharp
// Avoid
return Result<User>.Failure("Error");

// Prefer
return Result<User>.Failure(
    ErrorCode.ValidationError,
    "User name must be between 3 and 50 characters",
    new Dictionary<string, object>
    {
        { "Field", "Name" },
        { "MinLength", 3 },
        { "MaxLength", 50 }
    }
);
```

## Common Use Cases

### 1. API Responses

```csharp
public async Task<IActionResult> GetUser(int id)
{
    var result = await _userService.GetUser(id);
    return result.Match(
        user => Ok(user),
        error => StatusCode(GetStatusCode(error.Code), error.Message)
    );
}
```

### 2. Validation

```csharp
public Result<User> ValidateUser(User user)
{
    var validationResult = ValidationResult.Create();

    if (string.IsNullOrEmpty(user.Name))
    {
        validationResult.AddError("Name", "Name is required");
    }

    if (user.Age < 18)
    {
        validationResult.AddError("Age", "User must be at least 18 years old");
    }

    return validationResult.IsValid
        ? Result<User>.Success(user)
        : Result<User>.Failure(validationResult);
}
```

### 3. Database Operations

```csharp
public async Task<Result<User>> CreateUser(User user)
{
    try
    {
        var createdUser = await _repository.CreateAsync(user);
        return Result<User>.Success(createdUser);
    }
    catch (Exception ex)
    {
        return Result<User>.Failure(
            ErrorCode.DatabaseError,
            "Failed to create user",
            new Dictionary<string, object> { { "Exception", ex } }
        );
    }
}
```

## Next Steps

- [Error Handling](error-handling.md)
- [Validation](validation.md)
- [Async Support](async-support.md)
- [Examples](examples/basic-examples.md) 