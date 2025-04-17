# Basic Usage

## Creating Results

### Success Results

Create a successful result using the `Success` factory method:

```csharp
// Create a successful result with a value
var successResult = Result<int>.Success(42);

// Create a successful result with a string
var stringResult = Result<string>.Success("Hello, World!");

// Create a successful result with an object
var userResult = Result<User>.Success(new User { Id = 1, Name = "John" });
```

### Failure Results

Create a failed result using the `Failure` factory method:

```csharp
// Create a failed result with an error message
var errorResult = Result<int>.Failure("Something went wrong");

// Create a failed result with an error code and message
var errorResult = Result<int>.Failure(ErrorCode.NotFound, "User not found");

// Create a failed result with a custom error
var errorResult = Result<int>.Failure(new Error(ErrorCode.ValidationError, "Invalid input"));
```

## Working with Results

### Checking Result State

```csharp
var result = GetUser(1);

// Check if the result is successful
if (result.IsSuccess)
{
    Console.WriteLine($"User found: {result.Value.Name}");
}

// Check if the result has failed
if (result.IsFailure)
{
    Console.WriteLine($"Error: {result.Error.Message}");
}
```

### Accessing Values

```csharp
var result = GetUser(1);

// Get the value if successful, or throw an exception if failed
var user = result.Value;

// Get the value if successful, or a default value if failed
var user = result.GetValueOrDefault();

// Get the value if successful, or null if failed
var user = result.GetValueOrNull();

// Get the value if successful, or execute a function if failed
var user = result.GetValueOr(() => new User { Name = "Default" });
```

### Handling Results

```csharp
var result = GetUser(1);

// Match on success or failure
result.Match(
    user => Console.WriteLine($"User found: {user.Name}"),
    error => Console.WriteLine($"Error: {error.Message}")
);

// Execute an action on success
result.OnSuccess(user => Console.WriteLine($"User found: {user.Name}"));

// Execute an action on failure
result.OnFailure(error => Console.WriteLine($"Error: {error.Message}"));
```

## Transforming Results

### Map

Transform a successful result using the `Map` method:

```csharp
var result = GetUser(1)
    .Map(user => user.Name)
    .Map(name => name.ToUpper());

result.Match(
    name => Console.WriteLine($"User name: {name}"),
    error => Console.WriteLine($"Error: {error.Message}")
);
```

### Bind

Chain operations that return results using the `Bind` method:

```csharp
var result = GetUser(1)
    .Bind(user => GetUserAddress(user.Id))
    .Bind(address => GetAddressCoordinates(address));

result.Match(
    coordinates => Console.WriteLine($"Coordinates: {coordinates}"),
    error => Console.WriteLine($"Error: {error.Message}")
);
```

### Filter

Filter a successful result using the `Filter` method:

```csharp
var result = GetUser(1)
    .Filter(user => user.Age >= 18, "User must be at least 18 years old");

result.Match(
    user => Console.WriteLine($"User found: {user.Name}"),
    error => Console.WriteLine($"Error: {error.Message}")
);
```

## Working with Collections

### Combining Results

Combine multiple results into a single result:

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

### LINQ Operations

Use LINQ operations with results:

```csharp
var results = new[]
{
    GetUser(1),
    GetUser(2),
    GetUser(3)
};

var validUsers = results
    .Where(r => r.IsSuccess)
    .Select(r => r.Value)
    .ToList();

Console.WriteLine($"Found {validUsers.Count} valid users");
```

## Error Handling

### Creating Errors

```csharp
// Create a simple error
var error = Error.Create("Something went wrong");

// Create an error with a code
var error = Error.Create(ErrorCode.NotFound, "User not found");

// Create an error with metadata
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

### Handling Errors

```csharp
var result = GetUser(1);

// Handle specific error codes
if (result.IsFailure)
{
    switch (result.Error.Code)
    {
        case ErrorCode.NotFound:
            Console.WriteLine("User not found");
            break;
        case ErrorCode.ValidationError:
            Console.WriteLine("Validation failed");
            break;
        default:
            Console.WriteLine("Unknown error");
            break;
    }
}
```

## Next Steps

- [Features](features/result-pattern.md)
- [Examples](examples/basic-examples.md)
- [API Reference](api-reference/result.md) 