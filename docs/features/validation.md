# Validation

## Overview

TinyResult provides a robust validation system that helps you validate your data in a clean and type-safe way. The validation features allow you to collect validation errors and handle them appropriately.

## Key Concepts

### 1. ValidationResult

The `ValidationResult` type represents the result of a validation operation:

```csharp
public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyDictionary<string, string> Errors { get; }
}
```

### 2. Creating Validation Results

```csharp
// Create an empty validation result
var validationResult = ValidationResult.Create();

// Create a validation result with errors
var validationResult = ValidationResult.Create()
    .AddError("Name", "Name is required")
    .AddError("Email", "Email is invalid");
```

## Basic Validation

### 1. Simple Validation

```csharp
public Result<User> ValidateUser(User user)
{
    var validationResult = ValidationResult.Create();

    if (string.IsNullOrEmpty(user.Name))
    {
        validationResult.AddError("Name", "Name is required");
    }

    if (user.Age < 18)
    {
        validationResult.AddError("Age", "User must be at least 18 years old");
    }

    return validationResult.IsValid
        ? Result<User>.Success(user)
        : Result<User>.Failure(validationResult);
}
```

### 2. Using Validation Rules

```csharp
public class UserValidationRules
{
    public static ValidationResult Validate(User user)
    {
        var result = ValidationResult.Create();

        // Name validation
        if (string.IsNullOrEmpty(user.Name))
        {
            result.AddError("Name", "Name is required");
        }
        else if (user.Name.Length < 3)
        {
            result.AddError("Name", "Name must be at least 3 characters");
        }

        // Email validation
        if (string.IsNullOrEmpty(user.Email))
        {
            result.AddError("Email", "Email is required");
        }
        else if (!IsValidEmail(user.Email))
        {
            result.AddError("Email", "Email is invalid");
        }

        return result;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
```

## Advanced Validation

### 1. Fluent Validation

```csharp
public class UserValidator
{
    private readonly ValidationResult _result;

    public UserValidator()
    {
        _result = ValidationResult.Create();
    }

    public UserValidator ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            _result.AddError("Name", "Name is required");
        }
        else if (name.Length < 3)
        {
            _result.AddError("Name", "Name must be at least 3 characters");
        }
        return this;
    }

    public UserValidator ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            _result.AddError("Email", "Email is required");
        }
        else if (!IsValidEmail(email))
        {
            _result.AddError("Email", "Email is invalid");
        }
        return this;
    }

    public ValidationResult GetResult() => _result;
}

// Usage
var validator = new UserValidator()
    .ValidateName(user.Name)
    .ValidateEmail(user.Email);

var result = validator.GetResult();
```

### 2. Validation with Metadata

```csharp
public class ValidationError
{
    public string Field { get; }
    public string Message { get; }
    public Dictionary<string, object> Metadata { get; }

    public ValidationError(string field, string message, Dictionary<string, object> metadata = null)
    {
        Field = field;
        Message = message;
        Metadata = metadata ?? new Dictionary<string, object>();
    }
}

public class EnhancedValidationResult
{
    private readonly List<ValidationError> _errors = new();

    public bool IsValid => _errors.Count == 0;
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    public void AddError(string field, string message, Dictionary<string, object> metadata = null)
    {
        _errors.Add(new ValidationError(field, message, metadata));
    }
}
```

### 3. Cross-Field Validation

```csharp
public class OrderValidator
{
    public ValidationResult Validate(Order order)
    {
        var result = ValidationResult.Create();

        // Basic field validation
        if (order.Quantity <= 0)
        {
            result.AddError("Quantity", "Quantity must be greater than 0");
        }

        if (order.Price <= 0)
        {
            result.AddError("Price", "Price must be greater than 0");
        }

        // Cross-field validation
        if (order.Quantity > 0 && order.Price > 0)
        {
            var total = order.Quantity * order.Price;
            if (total > order.Customer.CreditLimit)
            {
                result.AddError("Total", 
                    "Order total exceeds customer credit limit",
                    new Dictionary<string, object>
                    {
                        { "Total", total },
                        { "CreditLimit", order.Customer.CreditLimit }
                    });
            }
        }

        return result;
    }
}
```

## Integration with Result Pattern

### 1. Converting Validation Results

```csharp
public static class ValidationResultExtensions
{
    public static Result<T> ToResult<T>(this ValidationResult validationResult, T value)
    {
        return validationResult.IsValid
            ? Result<T>.Success(value)
            : Result<T>.Failure(validationResult);
    }
}

// Usage
var validationResult = ValidateUser(user);
var result = validationResult.ToResult(user);
```

### 2. Combining Results

```csharp
public Result<Order> ValidateAndCreateOrder(OrderRequest request)
{
    // Validate customer
    var customerResult = ValidateCustomer(request.Customer);
    if (customerResult.IsFailure)
    {
        return Result<Order>.Failure(customerResult.Error);
    }

    // Validate order items
    var itemsResult = ValidateOrderItems(request.Items);
    if (itemsResult.IsFailure)
    {
        return Result<Order>.Failure(itemsResult.Error);
    }

    // Create order
    var order = new Order
    {
        Customer = customerResult.Value,
        Items = itemsResult.Value
    };

    return Result<Order>.Success(order);
}
```

## Best Practices

### 1. Keep Validation Rules Separate

```csharp
// Avoid
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }

    public bool IsValid()
    {
        // Validation logic here
    }
}

// Prefer
public class UserValidator
{
    public ValidationResult Validate(User user)
    {
        // Validation logic here
    }
}
```

### 2. Use Descriptive Error Messages

```csharp
// Avoid
result.AddError("Name", "Invalid");

// Prefer
result.AddError("Name", "Name must be between 3 and 50 characters");
```

### 3. Include Context in Error Messages

```csharp
// Avoid
result.AddError("Age", "Invalid age");

// Prefer
result.AddError("Age", 
    "Age must be between 18 and 100",
    new Dictionary<string, object>
    {
        { "MinAge", 18 },
        { "MaxAge", 100 },
        { "CurrentAge", age }
    });
```

## Common Use Cases

### 1. Form Validation

```csharp
public Result<User> ValidateRegistrationForm(RegistrationForm form)
{
    var validationResult = ValidationResult.Create();

    // Validate username
    if (string.IsNullOrEmpty(form.Username))
    {
        validationResult.AddError("Username", "Username is required");
    }
    else if (form.Username.Length < 3)
    {
        validationResult.AddError("Username", "Username must be at least 3 characters");
    }

    // Validate password
    if (string.IsNullOrEmpty(form.Password))
    {
        validationResult.AddError("Password", "Password is required");
    }
    else if (form.Password.Length < 8)
    {
        validationResult.AddError("Password", "Password must be at least 8 characters");
    }

    return validationResult.ToResult(new User
    {
        Username = form.Username,
        Password = form.Password
    });
}
```

### 2. Business Rule Validation

```csharp
public Result<Order> ValidateOrder(Order order)
{
    var validationResult = ValidationResult.Create();

    // Validate order status
    if (order.Status == OrderStatus.Cancelled && order.Items.Any())
    {
        validationResult.AddError("Status", "Cannot cancel order with items");
    }

    // Validate payment
    if (order.PaymentStatus == PaymentStatus.Paid && order.TotalAmount == 0)
    {
        validationResult.AddError("Payment", "Cannot mark empty order as paid");
    }

    return validationResult.ToResult(order);
}
```

### 3. Data Integrity Validation

```csharp
public Result<DatabaseRecord> ValidateRecord(DatabaseRecord record)
{
    var validationResult = ValidationResult.Create();

    // Validate required fields
    if (record.Id == Guid.Empty)
    {
        validationResult.AddError("Id", "Id is required");
    }

    if (record.CreatedAt > DateTime.UtcNow)
    {
        validationResult.AddError("CreatedAt", "Creation date cannot be in the future");
    }

    // Validate relationships
    if (record.ParentId.HasValue && !_repository.Exists(record.ParentId.Value))
    {
        validationResult.AddError("ParentId", "Parent record does not exist");
    }

    return validationResult.ToResult(record);
}
```

## Next Steps

- [Async Support](async-support.md)
- [LINQ Support](linq-support.md)
- [Examples](examples/basic-examples.md) 