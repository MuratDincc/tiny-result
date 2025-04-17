# Pipeline

Pipeline provides a fluent way to chain operations and handle results in a sequential manner.

## Basic Usage

```csharp
// Create a pipeline
var pipeline = ResultPipeline.Start(new { Id = 1 });

// Chain operations
var result = pipeline
    .Map(x => x.Id)
    .Validate(x => x > 0, ErrorCode.InvalidInput, "Id must be positive")
    .Bind(x => Result<int>.Success(x * 2))
    .OnSuccess(x => Console.WriteLine($"Result: {x}"))
    .OnFailure(error => Console.WriteLine($"Error: {error}"))
    .Catch(ex => Result<int>.Failure(ErrorCode.InternalError, ex.Message))
    .Result;
```

## Advanced Usage

### Custom Pipeline

```csharp
// Implement IPipeline
public class CustomPipeline<T> : IPipeline<T>
{
    public IPipeline<T> Map<TResult>(Func<T, TResult> mapper)
    {
        // Custom implementation
    }

    public IPipeline<T> Bind<TResult>(Func<T, Result<TResult>> binder)
    {
        // Custom implementation
    }

    public IPipeline<T> Validate(Func<T, bool> predicate, ErrorCode code, string message)
    {
        // Custom implementation
    }

    public IPipeline<T> OnSuccess(Action<T> action)
    {
        // Custom implementation
    }

    public IPipeline<T> OnFailure(Action<Error> action)
    {
        // Custom implementation
    }

    public IPipeline<T> Catch(Func<Exception, Result<T>> handler)
    {
        // Custom implementation
    }

    public Result<T> Result { get; }
}

// Use custom pipeline
var customPipeline = new CustomPipeline<int>();
var result = customPipeline
    .Map(x => x * 2)
    .Result;
```

### Pipeline Events

```csharp
// Subscribe to pipeline events
pipeline.OnSuccess += (value) => 
    Console.WriteLine($"Pipeline success: {value}");
pipeline.OnFailure += (error) => 
    Console.WriteLine($"Pipeline failure: {error}");
pipeline.OnException += (exception) => 
    Console.WriteLine($"Pipeline exception: {exception.Message}");
```

## Best Practices

1. Keep pipelines short and focused
2. Use meaningful operation names
3. Handle errors appropriately
4. Implement proper logging
5. Use async operations when needed
6. Consider performance implications
7. Test pipeline behavior
8. Document pipeline flow 