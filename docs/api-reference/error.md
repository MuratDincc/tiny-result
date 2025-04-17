# Error API Reference

## Overview

The `Error` class represents an error that occurred during an operation. It contains information about the error, including a code, message, and optional metadata.

## Properties

### Code
```csharp
public ErrorCode Code { get; }
```
Gets the error code that categorizes the error.

### Message
```csharp
public string Message { get; }
```
Gets the error message that describes what went wrong.

### Metadata
```csharp
public IReadOnlyDictionary<string, object> Metadata { get; }
```
Gets additional metadata about the error.

## Static Factory Methods

### Create
```csharp
public static Error Create(string message)
public static Error Create(ErrorCode code, string message)
public static Error Create(ErrorCode code, string message, Dictionary<string, object> metadata)
```
Creates a new error with the specified information.

### FromException
```csharp
public static Error FromException(Exception exception)
public static Error FromException(Exception exception, ErrorCode code)
public static Error FromException(Exception exception, ErrorCode code, Dictionary<string, object> metadata)
```
Creates a new error from an exception.

## Instance Methods

### WithMetadata
```csharp
public Error WithMetadata(string key, object value)
public Error WithMetadata(Dictionary<string, object> metadata)
```
Creates a new error with additional metadata.

### ToString
```csharp
public override string ToString()
```
Returns a string representation of the error.

## Examples

### Creating Errors
```csharp
// Simple error
var error = Error.Create("Operation failed");

// Coded error
var notFoundError = Error.Create(ErrorCode.NotFound, "Resource not found");

// Error with metadata
var validationError = Error.Create(
    ErrorCode.ValidationError,
    "Invalid input",
    new Dictionary<string, object> { { "Field", "Name" } }
);

// Error from exception
try
{
    // Some operation that might throw
}
catch (Exception ex)
{
    var error = Error.FromException(ex, ErrorCode.InternalError);
}
```

### Working with Errors
```csharp
// Adding metadata
var detailedError = error.WithMetadata("Timestamp", DateTime.UtcNow);

// Combining metadata
var combinedError = error.WithMetadata(new Dictionary<string, object>
{
    { "UserId", 123 },
    { "Action", "Update" }
});

// String representation
Console.WriteLine(error.ToString());
// Output: Error[NotFound]: Resource not found
```

## Next Steps

- [Result API Reference](result.md)
- [ValidationResult API Reference](validation-result.md)
- [Extensions API Reference](extensions.md) 