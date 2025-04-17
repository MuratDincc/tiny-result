# Migration Guide

## Overview

This guide helps you migrate your existing code to use TinyResult effectively. It covers common scenarios and provides step-by-step instructions for migration.

## From Traditional Error Handling

### 1. Try-Catch Blocks

```csharp
// Before: Traditional try-catch
public User GetUser(int id)
{
    try
    {
        var user = _repository.GetUser(id);
        if (user == null)
            throw new UserNotFoundException($"User {id} not found");
        return user;
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "Error getting user");
        throw;
    }
}

// After: Using Result
public Result<User> GetUser(int id)
{
    return ResultPipeline<User>
        .Start(id)
        .Then(id => _repository.GetUser(id))
        .Validate(user => user != null, ErrorCode.NotFound, $"User {id} not found")
        .Catch(error => 
        {
            _logger.Error(error.Message);
            return Result<User>.Failure(error);
        })
        .Build();
}
```

### 2. Null Checks

```csharp
// Before: Null checks with exceptions
public string ProcessData(string data)
{
    if (data == null)
        throw new ArgumentNullException(nameof(data));
    
    return data.ToUpper();
}

// After: Using Result
public Result<string> ProcessData(string data)
{
    return ResultPipeline<string>
        .Start(data)
        .Validate(d => d != null, ErrorCode.ValidationError, "Data cannot be null")
        .Map(d => d.ToUpper())
        .Build();
}
```

## From Other Result Libraries

### 1. FluentResults

```csharp
// Before: FluentResults
public Result<User> GetUser(int id)
{
    return Result.Ok(id)
        .Bind(id => _repository.GetUser(id))
        .Ensure(user => user != null, "User not found")
        .OnSuccess(user => _logger.Info($"User found: {user.Id}"))
        .OnFailure(error => _logger.Error(error.Message));
}

// After: TinyResult
public Result<User> GetUser(int id)
{
    return ResultPipeline<User>
        .Start(id)
        .Then(id => _repository.GetUser(id))
        .Validate(user => user != null, ErrorCode.NotFound, "User not found")
        .OnSuccess(user => _logger.Info($"User found: {user.Id}"))
        .OnFailure(error => _logger.Error(error.Message))
        .Build();
}
```

### 2. LanguageExt

```csharp
// Before: LanguageExt
public Either<Error, User> GetUser(int id)
{
    return from user in _repository.GetUser(id)
           from validated in ValidateUser(user)
           select validated;
}

// After: TinyResult
public Result<User> GetUser(int id)
{
    return ResultPipeline<User>
        .Start(id)
        .Then(id => _repository.GetUser(id))
        .Then(user => ValidateUser(user))
        .Build();
}
```

## From Custom Result Types

### 1. Simple Result

```csharp
// Before: Custom Result
public class OperationResult<T>
{
    public bool Success { get; }
    public T Value { get; }
    public string ErrorMessage { get; }
}

public OperationResult<User> GetUser(int id)
{
    var user = _repository.GetUser(id);
    if (user == null)
        return OperationResult<User>.Failure("User not found");
    return OperationResult<User>.Success(user);
}

// After: TinyResult
public Result<User> GetUser(int id)
{
    return ResultPipeline<User>
        .Start(id)
        .Then(id => _repository.GetUser(id))
        .Validate(user => user != null, ErrorCode.NotFound, "User not found")
        .Build();
}
```

### 2. Result with Metadata

```csharp
// Before: Custom Result with Metadata
public class OperationResult<T>
{
    public bool Success { get; }
    public T Value { get; }
    public string ErrorMessage { get; }
    public Dictionary<string, object> Metadata { get; }
}

// After: TinyResult
public Result<User> GetUser(int id)
{
    return ResultPipeline<User>
        .Start(id)
        .Then(id => _repository.GetUser(id))
        .Validate(user => user != null, ErrorCode.NotFound, "User not found")
        .OnSuccess(user => 
        {
            user.Metadata["RetrievedAt"] = DateTime.UtcNow;
            return user;
        })
        .Build();
}
```

## Common Migration Patterns

### 1. Converting Exceptions

```csharp
// Before: Exception-based
public User GetUser(int id)
{
    try
    {
        return _repository.GetUser(id);
    }
    catch (SqlException ex)
    {
        throw new DatabaseException("Database error", ex);
    }
}

// After: Result-based
public Result<User> GetUser(int id)
{
    return Result.FromTry(
        () => _repository.GetUser(id),
        ex => ex is SqlException
            ? Error.Create(ErrorCode.DatabaseError, "Database error", ex)
            : Error.Create(ErrorCode.InternalError, "Unexpected error", ex)
    );
}
```

### 2. Handling Multiple Results

```csharp
// Before: Multiple checks
public (User User, Order Order) GetUserAndOrder(int userId, int orderId)
{
    var user = _userRepository.GetUser(userId);
    if (user == null)
        throw new UserNotFoundException($"User {userId} not found");

    var order = _orderRepository.GetOrder(orderId);
    if (order == null)
        throw new OrderNotFoundException($"Order {orderId} not found");

    return (user, order);
}

// After: Using Result.Combine
public Result<(User User, Order Order)> GetUserAndOrder(int userId, int orderId)
{
    var userResult = ResultPipeline<User>
        .Start(userId)
        .Then(id => _userRepository.GetUser(id))
        .Validate(user => user != null, ErrorCode.NotFound, $"User {userId} not found")
        .Build();

    var orderResult = ResultPipeline<Order>
        .Start(orderId)
        .Then(id => _orderRepository.GetOrder(id))
        .Validate(order => order != null, ErrorCode.NotFound, $"Order {orderId} not found")
        .Build();

    return Result.Combine(userResult, orderResult);
}
```

## Migration Checklist

1. **Identify Error Handling Patterns**
   - Find try-catch blocks
   - Locate null checks
   - Identify custom result types

2. **Replace Exception Throwing**
   - Convert throw statements to Result.Failure
   - Use appropriate error codes
   - Add error metadata

3. **Update Method Signatures**
   - Change return types to Result<T>
   - Update async methods to return Task<Result<T>>
   - Modify method documentation

4. **Refactor Validation**
   - Replace if statements with Validate
   - Use batch validation where appropriate
   - Implement custom validation rules

5. **Update Error Handling**
   - Replace catch blocks with Catch
   - Use appropriate error recovery strategies
   - Add error logging

6. **Test Migration**
   - Verify success cases
   - Test error scenarios
   - Check performance impact

7. **Document Changes**
   - Update API documentation
   - Add migration notes
   - Document breaking changes

## Breaking Changes

1. **Method Signatures**
   - Return types changed to Result<T>
   - Async methods return Task<Result<T>>

2. **Error Handling**
   - Exceptions replaced with Result.Failure
   - Custom error types replaced with Error

3. **Validation**
   - Validation logic moved to Validate method
   - Custom validation rules need adaptation

4. **Async Operations**
   - ConfigureAwait usage required
   - Parallel processing patterns changed

## Tips for Smooth Migration

1. **Start Small**
   - Begin with simple methods
   - Migrate one component at a time
   - Test thoroughly after each change

2. **Use Automation**
   - Create migration scripts
   - Use code analysis tools
   - Automate testing

3. **Maintain Compatibility**
   - Keep old methods temporarily
   - Use adapter patterns
   - Phase out old code gradually

4. **Document Progress**
   - Track migrated components
   - Note issues encountered
   - Share lessons learned

5. **Get Feedback**
   - Involve team members
   - Gather user feedback
   - Adjust approach as needed 