# Error Handling

## Overview

TinyResult provides a clean and type-safe way to handle errors in your applications. The error handling features offer a comprehensive solution for catching, processing, and reporting errors.

## Key Concepts

### 1. Error Types

```csharp
// Simple error
var error = Error.Create("Something went wrong");

// Error with code
var error = Error.Create(ErrorCode.NotFound, "User not found");

// Error with metadata
var error = Error.Create(
    ErrorCode.ValidationError,
    "Invalid input",
    new Dictionary<string, object>
    {
        { "Field", "Name" },
        { "Value", "" }
    }
);
```

### 2. Error Codes

Predefined error codes:

```csharp
public enum ErrorCode
{
    Unknown,           // Unknown error
    NotFound,          // Resource not found
    ValidationError,   // Validation error
    Unauthorized,      // Unauthorized access
    InvalidOperation,  // Invalid operation
    NetworkError,      // Network error
    Timeout,           // Timeout
    ConfigurationError,// Configuration error
    DatabaseError      // Database error
}
```

## Error Handling Strategies

### 1. Basic Error Handling

```csharp
var result = GetUser(1);

// Error handling with Match
result.Match(
    user => Console.WriteLine($"User found: {user.Name}"),
    error => Console.WriteLine($"Error: {error.Message}")
);

// Error handling with OnFailure
result.OnFailure(error => Console.WriteLine($"Error: {error.Message}"));
```

### 2. Error Code Based Handling

```csharp
var result = GetUser(1);

if (result.IsFailure)
{
    switch (result.Error.Code)
    {
        case ErrorCode.NotFound:
            Console.WriteLine("User not found");
            break;
        case ErrorCode.ValidationError:
            Console.WriteLine("Validation error");
            break;
        default:
            Console.WriteLine("Unknown error");
            break;
    }
}
```

### 3. Error Transformation

```csharp
var result = GetUser(1)
    .TransformError(error => new Error(
        ErrorCode.Unknown,
        $"Unexpected error: {error.Message}"
    ));
```

## Advanced Features

### 1. Error Aggregation

```csharp
var results = new[]
{
    GetUser(1),
    GetUser(2),
    GetUser(3)
};

var combinedResult = Result.Combine(results);

if (combinedResult.IsFailure)
{
    foreach (var error in combinedResult.Error.Metadata["Errors"] as IEnumerable<Error>)
    {
        Console.WriteLine($"Error: {error.Message}");
    }
}
```

### 2. Error Logging

```csharp
// Error logging configuration
Result.OnError = error =>
{
    Console.WriteLine($"Error Code: {error.Code}");
    Console.WriteLine($"Error Message: {error.Message}");
    foreach (var metadata in error.Metadata)
    {
        Console.WriteLine($"{metadata.Key}: {metadata.Value}");
    }
};
```

### 3. Custom Error Handlers

```csharp
public class CustomErrorHandler
{
    public static Result<T> HandleError<T>(Error error)
    {
        switch (error.Code)
        {
            case ErrorCode.NotFound:
                return Result<T>.Failure("Resource not found. Please try again.");
            case ErrorCode.ValidationError:
                return Result<T>.Failure("Invalid data.");
            default:
                return Result<T>.Failure("An unexpected error occurred.");
        }
    }
}

var result = GetUser(1)
    .Catch(CustomErrorHandler.HandleError);
```

## Best Practices

### 1. Descriptive Error Messages

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

### 2. Error Chaining

```csharp
var result = GetUser(1)
    .Catch(error => CustomErrorHandler.HandleError(error))
    .Catch(error => FallbackErrorHandler.HandleError(error));
```

### 3. Error Tracking

```csharp
public static Result<T> WithErrorTracking<T>(Func<Result<T>> operation)
{
    try
    {
        return operation();
    }
    catch (Exception ex)
    {
        return Result<T>.Failure(
            ErrorCode.Unknown,
            "An unexpected error occurred",
            new Dictionary<string, object>
            {
                { "Exception", ex },
                { "StackTrace", ex.StackTrace }
            }
        );
    }
}
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

### 2. Database Operations

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

### 3. Validation

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

## Next Steps

- [Validation](validation.md)
- [Async Support](async-support.md)
- [Examples](examples/basic-examples.md) 