# Performance Guide

## Overview

This guide provides best practices and recommendations for optimizing performance when using TinyResult in your applications.

## Memory Usage

### 1. Value Types vs Reference Types

```csharp
// Prefer value types for small data
public Result<int> GetUserId() { ... }

// Use reference types for larger data
public Result<User> GetUser() { ... }
```

### 2. Avoid Unnecessary Allocations

```csharp
// Avoid: Creates new Error instance for each call
public Result<T> Validate(T value)
{
    return value != null 
        ? Result<T>.Success(value)
        : Result<T>.Failure(new Error(ErrorCode.ValidationError, "Value cannot be null"));
}

// Prefer: Reuse Error instances
private static readonly Error NullValueError = Error.Create(ErrorCode.ValidationError, "Value cannot be null");

public Result<T> Validate(T value)
{
    return value != null 
        ? Result<T>.Success(value)
        : Result<T>.Failure(NullValueError);
}
```

## Async Operations

### 1. ConfigureAwait

```csharp
public async Task<Result<T>> GetDataAsync()
{
    return await ResultPipeline<T>
        .Start(initialValue)
        .ThenAsync(async value => await ProcessAsync(value).ConfigureAwait(false))
        .BuildAsync();
}
```

### 2. Parallel Operations

```csharp
public async Task<Result<List<T>>> ProcessInParallelAsync(List<int> ids)
{
    var tasks = ids.Select(id => ProcessAsync(id));
    var results = await Task.WhenAll(tasks);
    
    return Result.Combine(results);
}
```

## Pipeline Optimization

### 1. Chain Length

```csharp
// Avoid: Too many chained operations
var result = pipeline
    .Map(x => x + 1)
    .Map(x => x * 2)
    .Map(x => x.ToString())
    .Map(x => x.ToUpper())
    .Map(x => x.Length);

// Prefer: Combine operations
var result = pipeline
    .Map(x => (x + 1) * 2)
    .Map(x => x.ToString().ToUpper().Length);
```

### 2. Early Validation

```csharp
// Avoid: Validating after expensive operations
var result = await pipeline
    .ThenAsync(async x => await ExpensiveOperationAsync(x))
    .Validate(x => x.IsValid, "Invalid result");

// Prefer: Validate early
var result = await pipeline
    .Validate(x => x.IsValid, "Invalid input")
    .ThenAsync(async x => await ExpensiveOperationAsync(x));
```

## Error Handling

### 1. Error Creation

```csharp
// Avoid: Creating detailed errors for every failure
public Result<T> Process(T value)
{
    try
    {
        // Operation
    }
    catch (Exception ex)
    {
        return Result<T>.Failure(new Error(
            ErrorCode.InternalError,
            ex.Message,
            new Dictionary<string, object>
            {
                { "StackTrace", ex.StackTrace },
                { "Source", ex.Source },
                { "Timestamp", DateTime.UtcNow }
            }
        ));
    }
}

// Prefer: Create detailed errors only when needed
public Result<T> Process(T value)
{
    try
    {
        // Operation
    }
    catch (Exception ex)
    {
        return Result<T>.Failure(ErrorCode.InternalError, "Operation failed");
    }
}
```

### 2. Error Recovery

```csharp
// Avoid: Multiple recovery attempts
var result = await pipeline
    .Catch(error => Recovery1(error))
    .Catch(error => Recovery2(error))
    .Catch(error => Recovery3(error));

// Prefer: Single recovery with fallback
var result = await pipeline
    .Catch(error => 
        error.Code == ErrorCode.Timeout 
            ? Recovery1(error)
            : Recovery2(error)
    );
```

## Validation

### 1. Batch Validation

```csharp
// Avoid: Multiple validation calls
var result = value
    .Validate(x => x != null, "Value cannot be null")
    .Validate(x => x.Length > 0, "Value cannot be empty")
    .Validate(x => x.Length < 100, "Value too long");

// Prefer: Single validation with multiple rules
var result = value.Validate(x => new[]
{
    (x != null, "Value cannot be null"),
    (x.Length > 0, "Value cannot be empty"),
    (x.Length < 100, "Value too long")
});
```

### 2. Lazy Validation

```csharp
// Avoid: Validating all fields immediately
public Result<User> ValidateUser(User user)
{
    var validationResult = ValidationResult.Create()
        .AddErrorIf(user.Name == null, "Name", "Name is required")
        .AddErrorIf(user.Email == null, "Email", "Email is required")
        .AddErrorIf(user.Password == null, "Password", "Password is required");

    return validationResult.IsValid
        ? Result<User>.Success(user)
        : Result<User>.Failure(validationResult.Errors);
}

// Prefer: Validate fields only when needed
public Result<User> ValidateUser(User user)
{
    return ResultPipeline<User>
        .Start(user)
        .Then(u => ValidateName(u))
        .Then(u => ValidateEmail(u))
        .Then(u => ValidatePassword(u))
        .Build();
}
```

## Best Practices

1. **Use Value Types for Small Data**
   - Reduces memory allocations
   - Improves cache locality

2. **Reuse Error Instances**
   - Cache common error messages
   - Use static readonly fields

3. **Optimize Async Operations**
   - Use ConfigureAwait(false)
   - Process in parallel when possible

4. **Minimize Pipeline Length**
   - Combine operations
   - Validate early

5. **Optimize Error Handling**
   - Create detailed errors only when needed
   - Use efficient recovery strategies

6. **Efficient Validation**
   - Use batch validation
   - Implement lazy validation

7. **Memory Management**
   - Avoid unnecessary allocations
   - Use object pooling for frequently created objects

8. **Exception Handling**
   - Catch specific exceptions
   - Avoid catching general Exception

9. **Logging**
   - Log errors at appropriate levels
   - Include relevant context

10. **Testing**
    - Write performance tests
    - Monitor memory usage
    - Profile critical paths 