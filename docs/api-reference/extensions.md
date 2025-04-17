# Extensions API Reference

## Overview

The `ResultExtensions` class provides extension methods for working with `Result<T>` and `ValidationResult` types. These methods enable functional programming patterns and make working with results more convenient.

## Result<T> Extensions

### ToResult
```csharp
public static Result<T> ToResult<T>(this T value)
```
Converts a value to a successful result.

### Tap
```csharp
public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
```
Executes an action on a successful result's value and returns the original result.

### TapError
```csharp
public static Result<T> TapError<T>(this Result<T> result, Action<Error> action)
```
Executes an action on a failed result's error and returns the original result.

### Select
```csharp
public static Result<TResult> Select<T, TResult>(this Result<T> result, Func<T, TResult> selector)
```
Transforms a successful result's value using the specified selector function.

### SelectMany
```csharp
public static Result<TResult> SelectMany<T, TResult>(this Result<T> result, Func<T, Result<TResult>> selector)
```
Transforms a successful result's value using the specified selector function that returns a new result.

### Where
```csharp
public static Result<T> Where<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
public static Result<T> Where<T>(this Result<T> result, Func<T, bool> predicate, Error error)
```
Filters a successful result's value using the specified predicate.

### Validate
```csharp
public static Result<T> Validate<T>(this Result<T> result, Func<T, ValidationResult> validator)
```
Validates a successful result's value using the specified validator function.

### TransformError
```csharp
public static Result<T> TransformError<T>(this Result<T> result, Func<Error, Error> transformer)
```
Transforms a failed result's error using the specified transformer function.

### GetValueOr
```csharp
public static T GetValueOr<T>(this Result<T> result, T defaultValue)
public static T GetValueOr<T>(this Result<T> result, Func<T> defaultValueFactory)
```
Gets the value of a successful result or returns the specified default value.

### GetValueOrAsync
```csharp
public static Task<T> GetValueOrAsync<T>(this Result<T> result, Func<Task<T>> defaultValueFactory)
```
Asynchronously gets the value of a successful result or returns the specified default value.

## ValidationResult Extensions

### ToResult
```csharp
public static Result<T> ToResult<T>(this ValidationResult validationResult, T value)
```
Converts a validation result to a result with the specified value.

### Combine
```csharp
public static ValidationResult Combine(this ValidationResult first, ValidationResult second)
```
Combines two validation results into a single result.

## Examples

### Working with Results
```csharp
// Convert value to result
var result = "Hello".ToResult();

// Tap into success
result = result.Tap(value => Console.WriteLine(value));

// Transform value
var lengthResult = result.Select(s => s.Length);

// Filter value
var nonEmptyResult = result.Where(s => s.Length > 0, "String cannot be empty");

// Validate value
var validatedResult = result.Validate(value => 
    ValidationResult.Create()
        .AddError("Name", "Name is required")
);

// Transform error
var transformedError = result.TransformError(error => 
    Error.Create(ErrorCode.InternalError, error.Message)
);

// Get value or default
var value = result.GetValueOr("Default");
```

### Working with Validation Results
```csharp
// Create validation results
var first = ValidationResult.Create()
    .AddError("Name", "Name is required");

var second = ValidationResult.Create()
    .AddError("Email", "Email is invalid");

// Combine validation results
var combined = first.Combine(second);

// Convert to result
var result = combined.ToResult(new User());
```

## Next Steps

- [Result API Reference](result.md)
- [Error API Reference](error.md)
- [ValidationResult API Reference](validation-result.md) 