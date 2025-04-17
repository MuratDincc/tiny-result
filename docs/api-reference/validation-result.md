# ValidationResult API Reference

## Overview

The `ValidationResult` class represents the result of a validation operation. It contains information about validation errors and whether the validation was successful.

## Properties

### IsValid
```csharp
public bool IsValid { get; }
```
Gets whether the validation was successful.

### Errors
```csharp
public IReadOnlyDictionary<string, string> Errors { get; }
```
Gets the validation errors as a read-only dictionary.

## Static Factory Methods

### Create
```csharp
public static ValidationResult Create()
```
Creates a new validation result.

## Instance Methods

### AddError
```csharp
public ValidationResult AddError(string field, string message)
```
Adds a validation error for the specified field.

### AddErrors
```csharp
public ValidationResult AddErrors(IEnumerable<KeyValuePair<string, string>> errors)
```
Adds multiple validation errors.

### Clear
```csharp
public ValidationResult Clear()
```
Clears all validation errors.

## Examples

### Creating Validation Results
```csharp
// Create a new validation result
var result = ValidationResult.Create();

// Add a single error
result = result.AddError("Name", "Name is required");

// Add multiple errors
result = result.AddErrors(new Dictionary<string, string>
{
    { "Email", "Email is invalid" },
    { "Password", "Password must be at least 8 characters" }
});

// Check validation status
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Key}: {error.Value}");
    }
}

// Clear errors
result = result.Clear();
```

## Next Steps

- [Result API Reference](result.md)
- [Error API Reference](error.md)
- [Extensions API Reference](extensions.md) 