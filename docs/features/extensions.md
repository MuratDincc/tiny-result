# Extensions

## Overview

TinyResult provides a rich set of extension methods that enhance the functionality of the Result Pattern. These extensions make it easier to work with results in various scenarios and provide additional utility methods for common operations.

## Basic Extensions

### 1. Value Access

```csharp
// Get value or default
var value = result.GetValueOr(defaultValue);

// Get value or throw
var value = result.GetValueOrThrow();

// Get value or handle error
var value = result.GetValueOr(error => HandleError(error));
```

### 2. Error Handling

```csharp
// Transform error
var transformedResult = result.TransformError(error => 
    new Error(error.Code, $"Modified: {error.Message}")
);

// Recover from error
var recoveredResult = result.Recover(error => 
    Result<T>.Success(fallbackValue)
);

// Handle specific error
var handledResult = result.Catch(
    error => error.Code == ErrorCode.NotFound,
    error => Result<T>.Success(fallbackValue)
);
```

### 3. Validation

```csharp
// Validate with predicate
var validatedResult = result.Validate(
    value => value != null,
    "Value cannot be null"
);

// Validate with multiple rules
var validatedResult = result.Validate(
    value => new[]
    {
        (value != null, "Value cannot be null"),
        (value.Length > 0, "Value cannot be empty")
    }
);
```

## Advanced Extensions

### 1. Pipeline Operations

```csharp
// Chain operations
var finalResult = result
    .Then(value => ProcessValue(value))
    .Then(processed => TransformValue(processed))
    .Then(transformed => SaveValue(transformed));

// Conditional operations
var conditionalResult = result
    .When(
        value => value.IsValid,
        value => ProcessValidValue(value),
        value => HandleInvalidValue(value)
    );
```

### 2. Async Operations

```csharp
// Async value access
var value = await result.GetValueOrAsync(async () => 
    await GetFallbackValueAsync()
);

// Async error handling
var handledResult = await result.CatchAsync(async error => 
    await HandleErrorAsync(error)
);

// Async validation
var validatedResult = await result.ValidateAsync(async value => 
    await IsValidAsync(value)
);
```

### 3. Collection Operations

```csharp
// Combine results
var combinedResult = results.Combine();

// Partition results
var (successes, failures) = results.Partition();

// Aggregate results
var aggregatedResult = results.Aggregate(
    (acc, result) => acc.Bind(value => 
        result.Map(newValue => CombineValues(value, newValue))
    )
);
```

## Best Practices

### 1. Use Appropriate Extensions

```csharp
// Avoid
if (result.IsSuccess)
{
    var value = result.Value;
    // Process value
}
else
{
    // Handle error
}

// Prefer
result.Match(
    value => ProcessValue(value),
    error => HandleError(error)
);
```

### 2. Chain Operations Effectively

```csharp
// Avoid
var result1 = ProcessValue(value);
if (result1.IsSuccess)
{
    var result2 = TransformValue(result1.Value);
    if (result2.IsSuccess)
    {
        return SaveValue(result2.Value);
    }
    return result2;
}
return result1;

// Prefer
return ProcessValue(value)
    .Bind(processed => TransformValue(processed))
    .Bind(transformed => SaveValue(transformed));
```

### 3. Handle Errors Gracefully

```csharp
// Avoid
try
{
    var value = result.GetValueOrThrow();
    // Process value
}
catch (Exception ex)
{
    // Handle exception
}

// Prefer
result.Match(
    value => ProcessValue(value),
    error => HandleError(error)
);
```

## Common Use Cases

### 1. API Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        return await _userService.GetUserAsync(id)
            .Match(
                user => Ok(user),
                error => StatusCode(GetStatusCode(error.Code), error.Message)
            );
    }
}
```

### 2. Service Layer

```csharp
public class UserService
{
    public async Task<Result<User>> UpdateUserAsync(int id, UserUpdate update)
    {
        return await GetUserAsync(id)
            .Validate(user => user.IsActive, "User is not active")
            .Bind(user => ValidateUpdateAsync(update))
            .Bind(validUpdate => ApplyUpdateAsync(id, validUpdate))
            .Catch(error => error.Code == ErrorCode.NotFound,
                error => CreateUserAsync(update));
    }
}
```

### 3. Data Access

```csharp
public class UserRepository
{
    public async Task<Result<User>> GetUserAsync(int id)
    {
        return await _context.Users
            .FindAsync(id)
            .ToResult()
            .Validate(user => user != null, $"User {id} not found")
            .Catch(error => error.Code == ErrorCode.NotFound,
                error => GetUserFromCacheAsync(id));
    }
}
```

## Next Steps

- [Examples](examples/basic-examples.md)
- [API Reference](api-reference/result.md)
- [Advanced Topics](advanced-topics/performance.md) 