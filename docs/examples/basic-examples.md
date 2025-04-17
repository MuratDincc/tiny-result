# Basic Examples

## 1. Basic Result Operations

### Creating Successful Results

```csharp
// Simple successful result
var successResult = Result<string>.Success("Operation successful");

// Valued successful result
var userResult = Result<User>.Success(new User { Id = 1, Name = "John" });

// Nullable value to result
string? name = "John";
var nameResult = name.ToResult("Name cannot be null");

// Value with validation
var ageResult = 25.ToResult(
    age => age >= 18,
    "Age must be at least 18"
);
```

### Creating Failed Results

```csharp
// Simple error
var errorResult = Result<string>.Failure("An error occurred");

// Coded error
var notFoundResult = Result<User>.Failure(ErrorCode.NotFound, "User not found");

// Detailed error with metadata
var validationError = Result<User>.Failure(
    ErrorCode.ValidationError,
    "Invalid user information",
    new Dictionary<string, object> 
    { 
        { "Field", "Email" },
        { "Value", "invalid-email" },
        { "Timestamp", DateTime.UtcNow }
    }
);

// From exception
try
{
    // Some operation that might throw
}
catch (Exception ex)
{
    var errorResult = Result<int>.Failure(
        Error.FromException(ex, ErrorCode.InvalidOperation)
    );
}
```

## 2. Result Checking

### Basic Checks

```csharp
// Success check
if (result.IsSuccess)
{
    var value = result.Value;
    // Operations
}

// Error check
if (result.IsFailure)
{
    var error = result.Error;
    // Error handling
}

// Pattern matching
switch (result)
{
    case { IsSuccess: true } success:
        Console.WriteLine($"Success: {success.Value}");
        break;
    case { IsFailure: true } failure:
        Console.WriteLine($"Error: {failure.Error.Message}");
        break;
}
```

### Using Match

```csharp
// Processing with Match
var message = result.Match(
    value => $"Success: {value}",
    error => $"Error: {error.Message}"
);

// Performing operations with Match
result.Match(
    value => ProcessValue(value),
    error => HandleError(error)
);

// Async Match
await result.MatchAsync(
    async value => await ProcessValueAsync(value),
    async error => await HandleErrorAsync(error)
);
```

## 3. Value Transformations

### Using Map

```csharp
// Simple transformation
var lengthResult = result.Map(value => value.Length);

// Complex transformation
var userDtoResult = result.Map(user => new UserDto
{
    Id = user.Id,
    FullName = $"{user.FirstName} {user.LastName}"
});

// Async Map
var asyncResult = await result.MapAsync(async value => 
    await ProcessValueAsync(value)
);
```

### Using Bind

```csharp
// Chaining operations
var finalResult = result
    .Bind(value => ProcessValue(value))
    .Bind(processed => SaveValue(processed));

// Conditional operations
var conditionalResult = result
    .Bind(value => value.IsValid 
        ? ProcessValidValue(value)
        : Result<T>.Failure("Invalid value"));

// Async Bind
var asyncResult = await result.BindAsync(async value => 
    await ProcessValueAsync(value)
);
```

## 4. Error Handling

### Try-Catch Like Usage

```csharp
// Try-Catch like
var safeResult = Result.Try(() => RiskyOperation());

// Async Try-Catch
var asyncSafeResult = await Result.TryAsync(
    async () => await RiskyOperationAsync()
);

// With custom error handler
var result = Result.Try(
    () => RiskyOperation(),
    ex => new Error(ErrorCode.InvalidOperation, ex.Message)
);
```

### Error Recovery

```csharp
// Using Catch
var recoveredResult = result.Catch(error =>
{
    if (error.Code == ErrorCode.NotFound)
    {
        return Result<T>.Success(GetDefaultValue());
    }
    return Result<T>.Failure(error);
});

// Using Catch with predicate
var specificRecovery = result.Catch(
    error => error.Code == ErrorCode.ValidationError,
    error => Result<T>.Success(GetDefaultValue())
);
```

## 5. Value Extraction

### Safe Value Access

```csharp
// Get value or default
var value = result.GetValueOrDefault("default");

// Get value or execute function
var value = result.GetValueOr(() => GetDefaultValue());

// Get value or throw
try
{
    var value = result.GetValueOrThrow();
}
catch (Exception ex)
{
    // Handle exception
}

// Get value or throw custom exception
var value = result.GetValueOrThrow(
    error => new CustomException(error.Message)
);
```

### Async Value Access

```csharp
// Get value or default async
var value = await result.GetValueOrAsync(
    async () => await GetDefaultValueAsync()
);

// Get value or execute async function
var value = await result.GetValueOrAsync(
    async error => await HandleErrorAsync(error)
);
```

## 6. Validation Operations

### Basic Validation

```csharp
// Single condition
var validatedResult = result.Validate(
    value => value != null,
    "Value cannot be null");

// Multiple conditions
var multiValidatedResult = result.Validate(
    value => new[]
    {
        (value != null, "Value cannot be null"),
        (value.Length > 0, "Value length must be greater than 0")
    });
```

### Async Validation

```csharp
// Async validation
var asyncValidatedResult = await result.ValidateAsync(
    async value => await IsValidAsync(value));
```

## 7. Collection Operations

### Processing Result Lists

```csharp
// Filtering successful results
var successes = results.Where(r => r.IsSuccess);

// Extracting values
var values = results
    .Where(r => r.IsSuccess)
    .Select(r => r.Value);

// Combining results
var combinedResult = results.Combine();
```

## 8. API Integration

### Controller Usage

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        return _userService.GetUser(id)
            .Match(
                user => Ok(user),
                error => StatusCode(GetStatusCode(error.Code), error.Message)
            );
    }
}
```

## Next Steps

- [Advanced Examples](advanced-examples.md)
- [Real World Scenarios](real-world-scenarios.md)
- [API Reference](api-reference/result.md) 