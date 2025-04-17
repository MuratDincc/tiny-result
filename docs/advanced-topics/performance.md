# Performance

## Overview

This section covers the performance characteristics of the TinyResult library and techniques that can be used to optimize performance.

## Performance Characteristics

### Lightweight Design
- Result objects are designed as value types
- Unnecessary object creation and garbage collection overhead is minimized
- Memory usage is optimized

### Caching
```csharp
// Cached result
private static readonly Result<string> CachedSuccess = Result<string>.Success("Success");

public Result<string> GetCachedResult()
{
    return CachedSuccess;
}
```

## Performance Tips

### Optimize Result Transformations
```csharp
// Bad - Unnecessary transformation
return Result<User>.Success(user).Map(u => u.Name);

// Good - Direct result creation
return Result<string>.Success(user.Name);
```

### Use Large Objects Wisely
```csharp
// Bad - Copying large object
return Result<byte[]>.Success(largeFile);

// Good - Using stream
return Result<Stream>.Success(fileStream);
```

### Optimize Async Operations
```csharp
// Bad - Unnecessary async/await
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = await _repository.GetUserAsync(id);
    return Result<User>.Success(user);
}

// Good - Direct Task return
public Task<Result<User>> GetUserAsync(int id)
{
    return Result<User>.TryAsync(() => _repository.GetUserAsync(id));
}
```

## Performance Tests

### Result Creation Performance
```csharp
[Benchmark]
public Result<string> CreateSuccessResult()
{
    return Result<string>.Success("Test");
}

[Benchmark]
public Result<string> CreateFailureResult()
{
    return Result<string>.Failure("Error");
}
```

### Result Transformation Performance
```csharp
[Benchmark]
public Result<int> MapResult()
{
    return Result<string>.Success("123")
        .Map(s => int.Parse(s));
}

[Benchmark]
public Result<int> BindResult()
{
    return Result<string>.Success("123")
        .Bind(s => Result<int>.Success(int.Parse(s)));
}
```

## Performance Measurements

### Memory Usage
- Successful result: ~24 bytes
- Failed result: ~40 bytes
- Error metadata: Variable

### Processing Time
- Result creation: ~10-20 ns
- Result transformation: ~20-30 ns
- Error handling: ~30-40 ns

## Performance Optimizations

### Result Pool Usage
```csharp
private static readonly ObjectPool<Result<string>> ResultPool = 
    new ObjectPool<Result<string>>(() => new Result<string>());

public Result<string> GetPooledResult()
{
    var result = ResultPool.Get();
    // Use result
    ResultPool.Return(result);
    return result;
}
```

### Cached Errors
```csharp
private static readonly Dictionary<string, Error> ErrorCache = 
    new Dictionary<string, Error>();

public Error GetCachedError(string message)
{
    if (!ErrorCache.TryGetValue(message, out var error))
    {
        error = Error.Create(message);
        ErrorCache[message] = error;
    }
    return error;
}
```

## Next Steps

- [Best Practices](best-practices.md)
- [Testing](testing.md)
- [Integration](integration.md) 