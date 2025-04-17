# Result API Reference

## Overview

The `Result<T>` class is the core type in TinyResult that represents the outcome of an operation that can either succeed or fail. It provides a type-safe way to handle success and failure cases in your applications.

## Properties

### IsSuccess
```csharp
public bool IsSuccess { get; }
```
Indicates whether the result represents a successful operation.

### IsFailure
```csharp
public bool IsFailure { get; }
```
Indicates whether the result represents a failed operation.

### Value
```csharp
public T? Value { get; }
```
The value held by the result if it is successful. This property is null if the result is a failure.

### Error
```csharp
public Error? Error { get; }
```
The error held by the result if it is a failure. This property is null if the result is successful.

## Static Methods

### Success
```csharp
public static Result<T> Success(T value)
```
Creates a successful result with the specified value.

**Parameters:**
- `value`: The value to hold in the result.

**Returns:**
A new successful `Result<T>` instance.

**Example:**
```csharp
var success = Result<int>.Success(42);
```

### Failure
```csharp
public static Result<T> Failure(Error error)
public static Result<T> Failure(ErrorCode code, string message)
```
Creates a failed result with the specified error.

**Parameters:**
- `error`: The error to hold in the result.
- `code`: The error code.
- `message`: The error message.

**Returns:**
A new failed `Result<T>` instance.

**Example:**
```csharp
var failure = Result<int>.Failure(ErrorCode.NotFound, "Value not found");
```

### FromTry
```csharp
public static Result<T> FromTry(Func<T> func, Func<Exception, Error> errorFactory)
```
Creates a result from a function that might throw an exception.

**Parameters:**
- `func`: The function to execute.
- `errorFactory`: The function to create an error from an exception.

**Returns:**
A new `Result<T>` instance.

**Example:**
```csharp
var result = Result<int>.FromTry(
    () => int.Parse("42"),
    ex => Error.Create(ErrorCode.InvalidOperation, ex.Message)
);
```

## Instance Methods

### Map
```csharp
public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
```
Transforms the value of a successful result using the specified function.

**Parameters:**
- `mapper`: The function to transform the value.

**Returns:**
A new result with the transformed value or the original error.

**Example:**
```csharp
var result = Result<int>.Success(42)
    .Map(x => x * 2);
```

### Bind
```csharp
public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> binder)
```
Binds the result to another result using the specified function.

**Parameters:**
- `binder`: The function to bind the result.

**Returns:**
A new result with the bound value or the original error.

**Example:**
```csharp
var result = Result<int>.Success(42)
    .Bind(x => Result<string>.Success(x.ToString()));
```

### Match
```csharp
public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
```
Handles both success and failure cases using the specified functions.

**Parameters:**
- `onSuccess`: The function to handle success.
- `onFailure`: The function to handle failure.

**Returns:**
The result of the appropriate handler function.

**Example:**
```csharp
var message = result.Match(
    value => $"Success: {value}",
    error => $"Error: {error.Message}"
);
```

### OnSuccess
```csharp
public Result<T> OnSuccess(Action<T> action)
```
Executes an action if the result is successful.

**Parameters:**
- `action`: The action to execute.

**Returns:**
The original result.

**Example:**
```csharp
result.OnSuccess(value => Console.WriteLine($"Value: {value}"));
```

### OnFailure
```csharp
public Result<T> OnFailure(Action<Error> action)
```
Executes an action if the result has failed.

**Parameters:**
- `action`: The action to execute.

**Returns:**
The original result.

**Example:**
```csharp
result.OnFailure(error => Console.WriteLine($"Error: {error.Message}"));
```

### Validate
```csharp
public Result<T> Validate(Func<T, bool> predicate, ErrorCode errorCode, string errorMessage)
```
Validates the result using the specified predicate.

**Parameters:**
- `predicate`: The predicate to validate the value.
- `errorCode`: The error code to use if validation fails.
- `errorMessage`: The error message to use if validation fails.

**Returns:**
A new result with the validated value or a failed result if validation fails.

**Example:**
```csharp
var result = Result<int>.Success(42)
    .Validate(x => x > 0, ErrorCode.ValidationError, "Value must be positive");
```

### Catch
```csharp
public Result<T> Catch(Func<Error, Result<T>> handler)
```
Handles errors by transforming them into a new result.

**Parameters:**
- `handler`: The function to handle the error.

**Returns:**
A new result with the handled value or the original error.

**Example:**
```csharp
var result = Result<int>.Failure(ErrorCode.NotFound, "Not found")
    .Catch(error => Result<int>.Success(0));
```

### GetValueOr
```csharp
public T GetValueOr(T defaultValue)
public T GetValueOr(Func<T> func)
```
Gets the value of the result if it is successful, or the specified default value if it has failed.

**Parameters:**
- `defaultValue`: The default value to return if the result has failed.
- `func`: The function to get the default value if the result has failed.

**Returns:**
The value of the result if it is successful, or the default value if it has failed.

**Example:**
```csharp
var value = result.GetValueOr(0);
var value = result.GetValueOr(() => GetDefaultValue());
```

### GetValueOrThrow
```csharp
public T GetValueOrThrow()
public T GetValueOrThrow(Func<Error, Exception> exceptionFactory)
```
Gets the value of the result if it is successful, or throws an exception if it has failed.

**Parameters:**
- `exceptionFactory`: The function to create an exception from an error.

**Returns:**
The value of the result if it is successful.

**Throws:**
An exception if the result has failed.

**Example:**
```csharp
try
{
    var value = result.GetValueOrThrow();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Static Utility Methods

### Combine
```csharp
public static Result<(T, T2)> Combine<T2>(Result<T> result1, Result<T2> result2)
public static Result<IEnumerable<T>> Combine(IEnumerable<Result<T>> results)
```
Combines multiple results into a single result.

**Parameters:**
- `result1`: The first result.
- `result2`: The second result.
- `results`: The results to combine.

**Returns:**
A new result with all values or the first error encountered.

**Example:**
```csharp
var result1 = Result<int>.Success(42);
var result2 = Result<string>.Success("Hello");
var combined = Result.Combine(result1, result2);

var results = new[] { result1, result2 };
var allResults = Result.Combine(results);
```

## Next Steps

- [Error API Reference](error.md)
- [ValidationResult API Reference](validation-result.md)
- [Extensions API Reference](extensions.md) 