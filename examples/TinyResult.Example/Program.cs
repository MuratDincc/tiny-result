using TinyResult.Enums;
using System.Net;

namespace TinyResult.Example;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== Basic Usage ===");
        BasicUsageExample();

        Console.WriteLine("\n=== Error Handling ===");
        ErrorHandlingExample();

        Console.WriteLine("\n=== Async Operations ===");
        await AsyncOperationsExample();

        Console.WriteLine("\n=== Validation ===");
        ValidationExample();

        Console.WriteLine("\n=== Result Combinations ===");
        ResultCombinationsExample();

        Console.WriteLine("\n=== Extension Methods ===");
        ExtensionMethodsExample();

        Console.WriteLine("\n=== Factory and Pipeline ===");
        await FactoryAndPipelineExample();

        Console.WriteLine("\n=== Circuit Breaker ===");
        await CircuitBreakerExample();
    }

    private static void BasicUsageExample()
    {
        // Basic success and failure
        var successResult = Result<int>.Success(42);
        var failureResult = Result<int>.Failure(new Error(ErrorCode.InvalidOperation, "Operation failed"));

        // Using Match
        var result = successResult.Match(
            value => $"Success: {value}",
            error => $"Error: {error.Message}"
        );
        Console.WriteLine(result);

        // Using OnSuccess and OnFailure
        successResult
            .OnSuccess(value => Console.WriteLine($"Value: {value}"))
            .OnFailure(error => Console.WriteLine($"Error: {error.Message}"));
    }

    private static void ErrorHandlingExample()
    {
        // Using Error with metadata
        var error = Error.Create(
            ErrorCode.ValidationError,
            "Invalid input",
            new Dictionary<string, object>
            {
                { "Field", "Email" },
                { "Value", "invalid-email" }
            }
        );

        var result = Result<int>.Failure(error);
        result.OnFailure(e => Console.WriteLine($"Error: {e.Code} - {e.Message}"));

        // Using FromTry
        var tryResult = Result<int>.FromTry(
            () => int.Parse("not-a-number"),
            ex => new Error(ErrorCode.InvalidOperation, $"Failed to parse number: {ex.Message}")
        );
        tryResult.OnFailure(e => Console.WriteLine($"Parse error: {e.Message}"));
    }

    private static async Task AsyncOperationsExample()
    {
        // Async success and failure
        var asyncSuccess = await Task.FromResult(Result<int>.Success(42));
        var asyncFailure = await Task.FromResult(Result<int>.Failure(new Error(ErrorCode.InvalidOperation, "Async operation failed")));

        // Using MapAsync and BindAsync
        var result = await Task.FromResult(asyncSuccess)
            .ContinueWith(t => t.Result.Map(x => x * 2))
            .ContinueWith(t => t.Result.Map(x => Result<int>.Success(x + 1)));

        result.OnSuccess(value => Console.WriteLine($"Async result: {value}"));
    }

    private static void ValidationExample()
    {
        // Using ValidationResult
        var validation = Result<int>.Success(42);
        var invalidValidation = Result<int>.Failure(
            Error.Create(ErrorCode.ValidationError, "Value must be positive")
        );

        // Using Match with ValidationResult
        var result = validation.Match(
            value => $"Valid: {value}",
            error => $"Invalid: {error.Message}"
        );
        Console.WriteLine(result);
    }

    private static void ResultCombinationsExample()
    {
        // Basic success and failure results
        var successResult = Result<int>.Success(42);
        var failureResult = Result<int>.Failure("Operation failed");

        // Using Match
        var result = successResult.Match(
            value => $"Success: {value}",
            error => $"Error: {error.Message}"
        );
        Console.WriteLine(result);

        // Using OnSuccess and OnFailure
        successResult
            .OnSuccess(value => Console.WriteLine($"Value: {value}"))
            .OnFailure(error => Console.WriteLine($"Error: {error.Message}"));

        // Using Map
        var mappedResult = successResult.Map(value => value * 2);
        Console.WriteLine($"Map result: {mappedResult.Value}");

        // Using Bind
        var boundResult = successResult.Bind(value => 
            value > 0 
                ? Result<string>.Success($"Positive number: {value}") 
                : Result<string>.Failure("Number is not positive")
        );
        Console.WriteLine($"Bind result: {boundResult.Value}");

        // Using Filter
        var filteredResult = successResult.Filter(
            value => value > 0,
            "Number is not positive"
        );
        Console.WriteLine($"Filter result: {filteredResult.Value}");

        // Using Validate
        var validatedResult = successResult.Validate(
            value => value > 0,
            "Number is not positive"
        );
        Console.WriteLine($"Validate result: {validatedResult.Value}");

        // Using FromTry
        var tryResult = Result<int>.FromTry(
            () => int.Parse("42"),
            ex => new Error(ErrorCode.InvalidOperation, $"Parse error: {ex.Message}")
        );
        Console.WriteLine($"FromTry result: {tryResult.Value}");

        // Using GetValueOrDefault
        var defaultValue = failureResult.GetValueOrDefault(0);
        Console.WriteLine($"Default value: {defaultValue}");

        // Using GetValueOrThrow
        try
        {
            var value = failureResult.GetValueOrThrow();
            Console.WriteLine($"Value: {value}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // Using GetValueOr
        var valueOr = failureResult.GetValueOr(() => 0);
        Console.WriteLine($"Value or default: {valueOr}");
    }

    private static void ExtensionMethodsExample()
    {
        // Using extension methods
        var result = Result<int>.Success(42)
            .Map(value => value * 2)
            .Map(value => value > 0 ? Result<int>.Success(value) : Result<int>.Failure("Value must be positive"))
            .Map(value => value.ToString())
            .Map(value => Result<string>.Success($"Transformed: {value}"));

        // Using pattern matching
        if (result.IsSuccess)
        {
            Console.WriteLine(result.Value);
        }
    }

    private static async Task FactoryAndPipelineExample()
    {
        // ResultFactory example
        var httpResult = ResultFactory.FromHttpResponse(
            new HttpResponseMessage(HttpStatusCode.OK),
            content => new { Message = "Success" }
        );

        var dbResult = await Task.FromResult(ResultFactory.FromDatabaseOperation(
            () => new { Id = 1, Name = "Test" },
            "GetUser"
        ));

        // ResultPipeline example
        var pipelineResult = await Task.FromResult(ResultPipeline<object>
            .Start(Result<object>.Success(new { Id = 1 }))
            .Then(data => Result<object>.Success(new { Id = ((dynamic)data).Id, Name = "Test" }))
            .OnSuccess(data => Console.WriteLine($"Pipeline successful: {data}"))
            .WithTimeout(TimeSpan.FromSeconds(5))
            .WithRetry(3, TimeSpan.FromSeconds(1))
            .Build());
        
        // Error handling example
        var errorResult = Result<object>.Failure(
            Error.Create(
                ErrorCode.InvalidOperation,
                "Operation failed",
                new Dictionary<string, object> { { "Detail", "Detailed error information" } }
            )
        );

        // Validation example
        var validationResult = ValidationResult.Create()
            .AddError("Email", "Invalid email format")
            .AddError("Password", "Password too short");

        if (validationResult.IsValid)
        {
            Console.WriteLine("Validation successful");
        }
        else
        {
            Console.WriteLine("Validation errors:");
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"- {error.Key}: {error.Value}");
            }
        }
    }

    private static async Task CircuitBreakerExample()
    {
        var maxFailures = 3;
        var resetTimeout = TimeSpan.FromSeconds(5);
        var currentFailures = 0;
        var lastFailureTime = DateTime.MinValue;

        // Simulate circuit breaker behavior
        for (int i = 0; i < 5; i++)
        {
            if (currentFailures >= maxFailures && DateTime.UtcNow - lastFailureTime < resetTimeout)
            {
                Console.WriteLine("Circuit breaker is open");
                await Task.Delay(1000);
                continue;
            }

            try
            {
                await SimulateOperation(i);
                currentFailures = 0;
                Console.WriteLine("Operation succeeded");
            }
            catch (Exception)
            {
                currentFailures++;
                lastFailureTime = DateTime.UtcNow;
                Console.WriteLine($"Operation failed. Failures: {currentFailures}");
            }

            await Task.Delay(1000);
        }
    }

    private static async Task SimulateOperation(int iteration)
    {
        await Task.Delay(100);
        if (iteration % 2 == 0)
            throw new Exception("Simulated failure");
    }
} 