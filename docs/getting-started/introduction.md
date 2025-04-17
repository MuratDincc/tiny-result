# Introduction

## What is TinyResult?

TinyResult is a lightweight and powerful implementation of the Result Pattern for .NET. It provides a clean and type-safe way to handle success and failure cases in your applications.

## Why Use TinyResult?

### 1. Clean Error Handling

TinyResult helps you handle errors in a clean and type-safe way. Instead of throwing exceptions or returning null, you can use the Result type to represent both success and failure cases.

```csharp
// Without TinyResult
public User GetUser(int id)
{
    var user = _userRepository.GetById(id);
    if (user == null)
    {
        throw new UserNotFoundException($"User with id {id} not found");
    }
    return user;
}

// With TinyResult
public Result<User> GetUser(int id)
{
    var user = _userRepository.GetById(id);
    return user != null 
        ? Result<User>.Success(user)
        : Result<User>.Failure($"User with id {id} not found");
}
```

### 2. Type Safety

TinyResult ensures that you handle both success and failure cases explicitly. The compiler will help you catch potential errors at compile time.

```csharp
// The compiler will force you to handle both cases
var result = GetUser(1);
result.Match(
    user => Console.WriteLine($"User found: {user.Name}"),
    error => Console.WriteLine($"Error: {error.Message}")
);
```

### 3. Functional Programming Features

TinyResult provides functional programming features like map, bind, and match operations that make your code more expressive and easier to reason about.

```csharp
// Chain operations together
var result = GetUser(1)
    .Map(user => user.Name)
    .Map(name => name.ToUpper())
    .OnSuccess(name => Console.WriteLine($"User name: {name}"))
    .OnFailure(error => Console.WriteLine($"Error: {error.Message}"));
```

### 4. Validation Support

TinyResult includes built-in validation support that makes it easy to validate your data and collect validation errors.

```csharp
// Validate user data
var validationResult = ValidationResult.Create()
    .AddError("Name", "Name is required")
    .AddError("Email", "Email is invalid");

if (!validationResult.IsValid)
{
    return Result<User>.Failure(validationResult);
}
```

### 5. Async Support

TinyResult provides full async/await support for all operations, making it easy to work with asynchronous code.

```csharp
// Async operations
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = await _userRepository.GetByIdAsync(id);
    return user != null 
        ? Result<User>.Success(user)
        : Result<User>.Failure($"User with id {id} not found");
}
```

### 6. LINQ Support

TinyResult supports LINQ-style operations that make it easy to work with collections of results.

```csharp
// LINQ operations
var results = new[] { GetUser(1), GetUser(2), GetUser(3) };
var validUsers = results.Where(r => r.IsSuccess).Select(r => r.Value);
```

## Getting Started

To get started with TinyResult, follow these steps:

1. [Install TinyResult](installation.md)
2. [Learn the basics](basic-usage.md)
3. [Explore the features](features/result-pattern.md)
4. [Check out the examples](examples/basic-examples.md)

## Next Steps

- [Installation](installation.md)
- [Basic Usage](basic-usage.md)
- [Features](features/result-pattern.md)
- [Examples](examples/basic-examples.md) 