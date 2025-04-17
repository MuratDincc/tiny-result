# LINQ Support

## Overview

TinyResult provides comprehensive LINQ support, allowing you to work with collections of results in a functional and expressive way. This support enables you to perform complex operations on result collections while maintaining type safety and error handling.

## Basic LINQ Operations

### 1. Filtering Results

```csharp
// Filter successful results
var successfulResults = results.Where(r => r.IsSuccess);

// Filter failed results
var failedResults = results.Where(r => r.IsFailure);

// Filter by error code
var notFoundResults = results.Where(r => r.Error.Code == ErrorCode.NotFound);
```

### 2. Transforming Results

```csharp
// Map successful results
var userNames = results
    .Where(r => r.IsSuccess)
    .Select(r => r.Value.Name);

// Map with error handling
var processedResults = results
    .Select(r => r.Map(user => ProcessUser(user)))
    .Where(r => r.IsSuccess);
```

### 3. Aggregating Results

```csharp
// Combine multiple results
var combinedResult = results.Aggregate(
    Result<List<User>>.Success(new List<User>()),
    (acc, result) => acc.Bind(users => 
        result.Map(user => {
            users.Add(user);
            return users;
        })
    )
);

// Count successful results
var successCount = results.Count(r => r.IsSuccess);
```

## Advanced LINQ Features

### 1. Result Sequences

```csharp
public Result<IEnumerable<User>> GetUsersByDepartment(int departmentId)
{
    return GetDepartmentUsers(departmentId)
        .Bind(users => ValidateUsers(users))
        .Map(validUsers => validUsers.OrderBy(u => u.Name));
}

private Result<IEnumerable<User>> ValidateUsers(IEnumerable<User> users)
{
    var validationResults = users
        .Select(user => ValidateUser(user))
        .ToList();

    if (validationResults.All(r => r.IsSuccess))
    {
        return Result<IEnumerable<User>>.Success(users);
    }

    var errors = validationResults
        .Where(r => r.IsFailure)
        .Select(r => r.Error)
        .ToList();

    return Result<IEnumerable<User>>.Failure(
        ErrorCode.ValidationError,
        "One or more users failed validation",
        new Dictionary<string, object> { { "Errors", errors } }
    );
}
```

### 2. Parallel Processing

```csharp
public async Task<Result<IEnumerable<User>>> ProcessUsersInParallel(IEnumerable<int> userIds)
{
    var tasks = userIds.Select(id => GetUserAsync(id));
    var results = await Task.WhenAll(tasks);

    if (results.All(r => r.IsSuccess))
    {
        return Result<IEnumerable<User>>.Success(
            results.Select(r => r.Value)
        );
    }

    var errors = results
        .Where(r => r.IsFailure)
        .Select(r => r.Error)
        .ToList();

    return Result<IEnumerable<User>>.Failure(
        ErrorCode.ProcessingError,
        "One or more users failed to process",
        new Dictionary<string, object> { { "Errors", errors } }
    );
}
```

### 3. Result Chaining

```csharp
public Result<Order> ProcessOrder(OrderRequest request)
{
    return ValidateOrder(request)
        .Bind(validRequest => GetCustomer(validRequest.CustomerId))
        .Bind(customer => CheckInventory(request.Items))
        .Bind(inventory => CalculateTotal(request.Items))
        .Bind(total => CreateOrder(request, total));
}
```

## Best Practices

### 1. Use Appropriate LINQ Methods

```csharp
// Avoid
var users = results
    .Where(r => r.IsSuccess)
    .Select(r => r.Value)
    .ToList();

// Prefer
var users = results
    .SelectMany(r => r.Match(
        user => new[] { user },
        error => Enumerable.Empty<User>()
    ))
    .ToList();
```

### 2. Handle Errors Appropriately

```csharp
// Avoid
var processedResults = results
    .Select(r => r.Map(ProcessUser))
    .ToList();

// Prefer
var processedResults = results
    .Select(r => r.Map(ProcessUser))
    .Where(r => r.IsSuccess)
    .Select(r => r.Value)
    .ToList();
```

### 3. Use Result Aggregation

```csharp
// Avoid
var allSuccess = results.All(r => r.IsSuccess);
if (allSuccess)
{
    var values = results.Select(r => r.Value).ToList();
}

// Prefer
var combinedResult = results.Aggregate(
    Result<List<User>>.Success(new List<User>()),
    (acc, result) => acc.Bind(users => 
        result.Map(user => {
            users.Add(user);
            return users;
        })
    )
);
```

## Common Use Cases

### 1. Batch Processing

```csharp
public Result<IEnumerable<ProcessedItem>> ProcessBatch(IEnumerable<Item> items)
{
    return items
        .Select(item => ValidateItem(item))
        .Aggregate(
            Result<List<ProcessedItem>>.Success(new List<ProcessedItem>()),
            (acc, result) => acc.Bind(processedItems => 
                result.Map(processedItem => {
                    processedItems.Add(processedItem);
                    return processedItems;
                })
            )
        );
}
```

### 2. Data Validation

```csharp
public Result<IEnumerable<ValidatedData>> ValidateDataSet(IEnumerable<Data> dataSet)
{
    var validationResults = dataSet
        .Select(data => ValidateData(data))
        .ToList();

    if (validationResults.All(r => r.IsSuccess))
    {
        return Result<IEnumerable<ValidatedData>>.Success(
            validationResults.Select(r => r.Value)
        );
    }

    var errors = validationResults
        .Where(r => r.IsFailure)
        .Select(r => r.Error)
        .ToList();

    return Result<IEnumerable<ValidatedData>>.Failure(
        ErrorCode.ValidationError,
        "One or more data items failed validation",
        new Dictionary<string, object> { { "Errors", errors } }
    );
}
```

### 3. API Response Handling

```csharp
public async Task<IActionResult> ProcessBatchRequest(BatchRequest request)
{
    var results = await Task.WhenAll(
        request.Items.Select(item => ProcessItemAsync(item))
    );

    if (results.All(r => r.IsSuccess))
    {
        return Ok(results.Select(r => r.Value));
    }

    var errors = results
        .Where(r => r.IsFailure)
        .Select(r => r.Error)
        .ToList();

    return BadRequest(new
    {
        Message = "One or more items failed to process",
        Errors = errors
    });
}
```

## Next Steps

- [Extensions](extensions.md)
- [Examples](examples/basic-examples.md)
- [API Reference](api-reference/result.md) 