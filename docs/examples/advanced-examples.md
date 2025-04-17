# Advanced Examples

## 1. Pipeline Operations

### Basic Pipeline

```csharp
public Result<Order> ProcessOrder(OrderRequest request)
{
    return ResultPipeline<Order>
        .StartWith(request)
        .Then(ValidateOrder)
        .Then(CheckInventory)
        .Then(CalculateTotal)
        .Then(CreateOrder)
        .Build();
}
```

### Conditional Pipeline

```csharp
public Result<Order> ProcessOrderWithDiscount(OrderRequest request)
{
    return ResultPipeline<Order>
        .StartWith(request)
        .Then(ValidateOrder)
        .When(
            order => order.Total > 1000,
            order => ApplyDiscount(order),
            order => order
        )
        .Then(CreateOrder)
        .Build();
}
```

## 2. Circuit Breaker Pattern

### Basic Usage

```csharp
public class OrderService
{
    private readonly ResultCircuitBreaker _circuitBreaker;

    public OrderService()
    {
        _circuitBreaker = new ResultCircuitBreaker(
            maxFailures: 3,
            resetTimeout: TimeSpan.FromMinutes(1)
        );
    }

    public async Task<Result<Order>> ProcessOrderAsync(OrderRequest request)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            return await _orderRepository.CreateOrderAsync(request);
        });
    }
}
```

### Custom Error Handling

```csharp
public async Task<Result<Order>> ProcessOrderWithCustomHandling(OrderRequest request)
{
    return await _circuitBreaker.ExecuteAsync(
        async () => await _orderRepository.CreateOrderAsync(request),
        error => error.Code == ErrorCode.Timeout,
        async () => await ProcessOrderWithFallbackAsync(request)
    );
}
```

## 3. Validation Pipeline

### Complex Validation

```csharp
public Result<User> ValidateUser(User user)
{
    return ResultPipeline<User>
        .StartWith(user)
        .Ensure(u => u.Email != null, "Email cannot be null")
        .Ensure(u => IsValidEmail(u.Email), "Invalid email format")
        .Ensure(u => u.Password != null, "Password cannot be null")
        .Ensure(u => IsStrongPassword(u.Password), "Weak password")
        .Then(ValidateUserAge)
        .Then(ValidateUserLocation)
        .Build();
}
```

### Async Validation

```csharp
public async Task<Result<User>> ValidateUserAsync(User user)
{
    return await ResultPipeline<User>
        .StartWith(user)
        .EnsureAsync(async u => await IsEmailUniqueAsync(u.Email), "Email already in use")
        .EnsureAsync(async u => await IsUsernameAvailableAsync(u.Username), "Username taken")
        .ThenAsync(async u => await ValidateUserLocationAsync(u))
        .BuildAsync();
}
```

## 4. Retry Mechanism

### Basic Retry

```csharp
public async Task<Result<Data>> GetDataWithRetryAsync()
{
    return await ResultPipeline<Data>
        .StartWith(() => GetDataAsync())
        .WithRetry(
            maxAttempts: 3,
            delay: TimeSpan.FromSeconds(1)
        )
        .BuildAsync();
}
```

### Conditional Retry

```csharp
public async Task<Result<Data>> GetDataWithConditionalRetryAsync()
{
    return await ResultPipeline<Data>
        .StartWith(() => GetDataAsync())
        .WithRetry(
            maxAttempts: 3,
            delay: TimeSpan.FromSeconds(1),
            shouldRetry: error => error.Code == ErrorCode.Timeout
        )
        .BuildAsync();
}
```

## 5. Timeout Management

### Basic Timeout

```csharp
public async Task<Result<Data>> GetDataWithTimeoutAsync()
{
    return await ResultPipeline<Data>
        .StartWith(() => GetDataAsync())
        .WithTimeout(TimeSpan.FromSeconds(5))
        .BuildAsync();
}
```

### Custom Timeout Handling

```csharp
public async Task<Result<Data>> GetDataWithCustomTimeoutAsync()
{
    return await ResultPipeline<Data>
        .StartWith(() => GetDataAsync())
        .WithTimeout(
            timeout: TimeSpan.FromSeconds(5),
            onTimeout: () => Result<Data>.Failure(ErrorCode.Timeout, "Operation timed out")
        )
        .BuildAsync();
}
```

## 6. Parallel Operations

### Parallel Result Processing

```csharp
public async Task<Result<List<Data>>> ProcessDataInParallelAsync(List<int> ids)
{
    var tasks = ids.Select(id => ProcessDataAsync(id));
    var results = await Task.WhenAll(tasks);

    return results
        .Aggregate(
            Result<List<Data>>.Success(new List<Data>()),
            (acc, result) => acc.Bind(dataList =>
                result.Map(data =>
                {
                    dataList.Add(data);
                    return dataList;
                })
            )
        );
}
```

### Parallel Validation

```csharp
public async Task<Result<List<User>>> ValidateUsersInParallelAsync(List<User> users)
{
    var validationTasks = users.Select(user => ValidateUserAsync(user));
    var validationResults = await Task.WhenAll(validationTasks);

    if (validationResults.All(r => r.IsSuccess))
    {
        return Result<List<User>>.Success(users);
    }

    var errors = validationResults
        .Where(r => r.IsFailure)
        .Select(r => r.Error)
        .ToList();

    return Result<List<User>>.Failure(
        ErrorCode.ValidationError,
        "Some users failed validation",
        new Dictionary<string, object> { { "Errors", errors } }
    );
}
```

## 7. Event Sourcing

### Event Processing

```csharp
public Result<Order> ProcessOrderEvents(List<OrderEvent> events)
{
    return ResultPipeline<Order>
        .StartWith(new Order())
        .Then(order => events.Aggregate(
            Result<Order>.Success(order),
            (result, @event) => result.Bind(currentOrder =>
                ProcessEvent(currentOrder, @event)
            )
        ))
        .Build();
}
```

### Event Validation

```csharp
public Result<OrderEvent> ValidateAndProcessEvent(OrderEvent @event)
{
    return ResultPipeline<OrderEvent>
        .StartWith(@event)
        .Ensure(e => e.Timestamp <= DateTime.UtcNow, "Event cannot be in the future")
        .Ensure(e => e.Version > 0, "Invalid version")
        .Then(ValidateEventData)
        .Then(ApplyEvent)
        .Build();
}
```

## 1. ResultFactory Usage

### HTTP Requests

```csharp
public async Task<Result<User>> GetUserFromApiAsync(int userId)
{
    var response = await _httpClient.GetAsync($"api/users/{userId}");
    return ResultFactory.FromHttpResponse(
        response,
        content => JsonSerializer.Deserialize<User>(content)
    );
}
```

### Database Operations

```csharp
public Result<User> GetUserFromDatabase(int userId)
{
    return ResultFactory.FromDatabaseOperation(
        () => _dbContext.Users.Find(userId),
        "GetUser"
    );
}
```

### File Operations

```csharp
public Result<string> ReadConfigFile(string filePath)
{
    return ResultFactory.FromFileOperation(
        filePath,
        content => JsonSerializer.Deserialize<Config>(content)
    );
}
```

## 2. ResultPipeline Usage

### Order Processing

```csharp
public async Task<Result<Order>> ProcessOrderAsync(OrderRequest request)
{
    return await ResultPipeline<Order>
        .Start(request)
        .Then(ValidateOrderRequest)
        .ThenAsync(async validRequest => await CheckInventoryAsync(validRequest))
        .ThenAsync(async inventory => await CalculateTotalAsync(inventory))
        .ThenAsync(async total => await ProcessPaymentAsync(total))
        .ThenAsync(async payment => await CreateOrderAsync(payment))
        .WithTimeout(TimeSpan.FromSeconds(30))
        .WithRetry(3, TimeSpan.FromSeconds(1))
        .BuildAsync();
}
```

### User Registration

```csharp
public async Task<Result<User>> RegisterUserAsync(UserRegistration request)
{
    return await ResultPipeline<User>
        .Start(request)
        .Then(ValidateUserData)
        .ThenAsync(async validData => await CheckEmailAvailabilityAsync(validData))
        .ThenAsync(async available => await HashPasswordAsync(available))
        .ThenAsync(async hashed => await CreateUserAsync(hashed))
        .ThenAsync(async user => await SendWelcomeEmailAsync(user))
        .BuildAsync();
}
```

### Complex Validation

```csharp
public Result<User> ValidateAndCreateUser(UserRequest request)
{
    var validationResult = ValidationResult.Create()
        .AddErrorIf(string.IsNullOrEmpty(request.Name), "Name", "Name is required")
        .AddErrorIf(!IsValidEmail(request.Email), "Email", "Invalid email format")
        .AddErrorIf(request.Age < 18, "Age", "User must be at least 18 years old");

    if (!validationResult.IsValid)
    {
        return Result<User>.Failure(
            Error.Create(
                ErrorCode.ValidationError,
                "Validation failed",
                new Dictionary<string, object> { { "Errors", validationResult.Errors } }
            )
        );
    }

    return Result<User>.Success(new User
    {
        Name = request.Name,
        Email = request.Email,
        Age = request.Age
    });
}
```

### Parallel Operations

```csharp
public async Task<Result<(User, Order)>> GetUserAndOrderAsync(int userId, int orderId)
{
    var userTask = GetUserAsync(userId);
    var orderTask = GetOrderAsync(orderId);

    await Task.WhenAll(userTask, orderTask);

    var userResult = await userTask;
    var orderResult = await orderTask;

    return Result<(User, Order)>.Combine(userResult, orderResult);
}
```

### Detailed Error Information

```csharp
public Result<Order> ProcessOrder(OrderRequest request)
{
    try
    {
        var order = CreateOrder(request);
        return Result<Order>.Success(order);
    }
    catch (Exception ex)
    {
        return Result<Order>.Failure(
            Error.Create(
                ErrorCode.InvalidOperation,
                "Failed to process order",
                new Dictionary<string, object>
                {
                    { "Request", request },
                    { "Exception", ex },
                    { "Timestamp", DateTime.UtcNow }
                }
            )
        );
    }
}
```

### Error Transformation

```csharp
public Result<User> GetUser(int userId)
{
    return ResultFactory.FromDatabaseOperation(
        () => _dbContext.Users.Find(userId),
        "GetUser"
    ).Catch(error =>
    {
        if (error.Code == ErrorCode.DatabaseError)
        {
            return Result<User>.Failure(
                Error.Create(
                    ErrorCode.NotFound,
                    "User not found",
                    new Dictionary<string, object> { { "UserId", userId } }
                )
            );
        }
        return Result<User>.Failure(error);
    });
}
```

## 6. GetValueOr* Methods

### Default Values

```csharp
public string GetUserName(Result<User> userResult)
{
    return userResult.GetValueOr(user => user.Name, () => "Anonymous");
}
```

### Alternative Operations on Error

```csharp
public async Task<string> GetUserDataAsync(int userId)
{
    var userResult = await GetUserAsync(userId);
    
    return await userResult.GetValueOrAsync(
        user => Task.FromResult(user.Data),
        async error => await GetDefaultUserDataAsync()
    );
}
```

### Exception Throwing

```csharp
public User GetUserOrThrow(Result<User> userResult)
{
    return userResult.GetValueOrThrow(
        error => new UserNotFoundException($"User not found: {error.Message}")
    );
}
```

## Next Steps

- [Real World Scenarios](real-world-scenarios.md)
- [API Reference](api-reference/result.md)
- [Advanced Topics](advanced-topics/performance.md) 