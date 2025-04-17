# TinyResult

A lightweight and powerful Result Pattern implementation for .NET 9. This library is designed to make error handling and validation operations cleaner and more readable.

## Features

- ✅ Result Pattern Implementation
- ✅ Comprehensive error handling
- ✅ Built-in validation support
- ✅ Full async/await support
- ✅ LINQ-style operations
- ✅ Fluent API for chaining operations
- ✅ Extensible structure
- ✅ Lightweight design
- ✅ .NET 9 Support

## Requirements

- .NET 9.0 or later

## Installation

```bash
dotnet add package TinyResult
```

## Quick Start

### Basic Usage

```csharp
// Creating a successful result
var success = Result<int>.Success(42);

// Creating a failed result
var failure = Result<int>.Failure(ErrorCode.NotFound, "Item not found");

// Pattern matching
var message = success.Match(
    value => $"Success: {value}",
    error => $"Error: {error.Message}"
);

// Chaining operations
var result = ResultPipeline<int>
    .Start(10)
    .Map(x => x * 2)
    .Validate(x => x > 0, ErrorCode.ValidationError, "Value must be positive")
    .OnSuccess(x => Console.WriteLine($"Value: {x}"))
    .OnFailure(error => Console.WriteLine($"Error: {error.Message}"))
    .Build();
```

### Error Handling

```csharp
// Creating detailed errors
var error = Error.Create(
    ErrorCode.ValidationError,
    "Invalid input",
    new Dictionary<string, object>
    {
        { "Field", "Email" },
        { "Value", "invalid-email" }
    }
);

// Exception handling
var result = Result.FromTry(
    () => int.Parse("not-a-number"),
    ex => Error.Create(ErrorCode.InternalError, "Failed to parse number", ex)
);
```

### Validation

```csharp
// Creating validation result
var validationResult = ValidationResult.Create()
    .AddError("Email", "Invalid email format")
    .AddError("Password", "Password too short");

// Using validation with Result
var result = Result<int>.Success(42)
    .Validate(value => value > 0, ErrorCode.ValidationError, "Value must be positive")
    .Validate(value => value < 100, ErrorCode.ValidationError, "Value must be less than 100");
```

### Async Operations

```csharp
// Async result creation
var asyncResult = await Result.FromTryAsync(
    async () => await GetDataAsync(),
    ex => Error.Create(ErrorCode.InternalError, "Failed to get data", ex)
);

// Async pipeline
var pipelineResult = await ResultPipeline<int>
    .Start(10)
    .ThenAsync(async x => await ProcessAsync(x))
    .MapAsync(async x => await TransformAsync(x))
    .BuildAsync();
```

### Result Combination

```csharp
// Combining two results
var result1 = Result<int>.Success(10);
var result2 = Result<int>.Success(20);

var combinedResult = Result.Combine(
    result1,
    result2,
    (value1, value2) => value1 + value2
);

// Combining multiple results
var results = new[]
{
    Result<int>.Success(1),
    Result<int>.Success(2),
    Result<int>.Success(3)
};

var combinedResults = Result.Combine(results);
```

## Documentation

For more detailed information, please visit our [documentation](https://muratdincc.github.io/tiny-result/).

## Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.