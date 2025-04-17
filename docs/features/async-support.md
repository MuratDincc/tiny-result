# Async Support

## Overview

TinyResult provides comprehensive support for asynchronous operations through the `Result<T>` type. This allows you to work with asynchronous code in a clean and type-safe way, while maintaining the benefits of the Result Pattern.

## Basic Async Operations

### 1. Creating Async Results

```csharp
// Async success
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = await _repository.GetByIdAsync(id);
    return user != null
        ? Result<User>.Success(user)
        : Result<User>.Failure(ErrorCode.NotFound, $"User {id} not found");
}

// Async failure
public async Task<Result<User>> CreateUserAsync(User user)
{
    try
    {
        var createdUser = await _repository.CreateAsync(user);
        return Result<User>.Success(createdUser);
    }
    catch (Exception ex)
    {
        return Result<User>.Failure(
            ErrorCode.DatabaseError,
            "Failed to create user",
            new Dictionary<string, object> { { "Exception", ex } }
        );
    }
}
```

### 2. Handling Async Results

```csharp
// Using async/await
public async Task<IActionResult> GetUser(int id)
{
    var result = await GetUserAsync(id);
    return result.Match(
        user => Ok(user),
        error => StatusCode(GetStatusCode(error.Code), error.Message)
    );
}

// Using ContinueWith
GetUserAsync(1)
    .ContinueWith(task =>
    {
        var result = task.Result;
        result.Match(
            user => Console.WriteLine($"User found: {user.Name}"),
            error => Console.WriteLine($"Error: {error.Message}")
        );
    });
```

## Advanced Async Features

### 1. Async Pipeline

```csharp
public async Task<Result<string>> ProcessUserAsync(int id)
{
    return await GetUserAsync(id)
        .MapAsync(user => GetUserProfileAsync(user))
        .MapAsync(profile => FormatProfileAsync(profile))
        .OnSuccessAsync(formatted => Console.WriteLine($"Formatted: {formatted}"))
        .OnFailureAsync(error => Console.WriteLine($"Error: {error.Message}"));
}
```

### 2. Async Validation

```csharp
public async Task<Result<User>> ValidateAndCreateUserAsync(User user)
{
    // Async validation
    var validationResult = await ValidateUserAsync(user);
    if (validationResult.IsFailure)
    {
        return Result<User>.Failure(validationResult);
    }

    // Async creation
    return await CreateUserAsync(user);
}

private async Task<ValidationResult> ValidateUserAsync(User user)
{
    var result = ValidationResult.Create();

    // Async name validation
    if (await IsNameTakenAsync(user.Name))
    {
        result.AddError("Name", "Username is already taken");
    }

    // Async email validation
    if (await IsEmailValidAsync(user.Email))
    {
        result.AddError("Email", "Email is invalid");
    }

    return result;
}
```

### 3. Async Error Handling

```csharp
public async Task<Result<User>> GetUserWithRetryAsync(int id)
{
    return await GetUserAsync(id)
        .CatchAsync(async error =>
        {
            if (error.Code == ErrorCode.NetworkError)
            {
                // Retry after delay
                await Task.Delay(1000);
                return await GetUserAsync(id);
            }
            return Result<User>.Failure(error);
        });
}
```

## Best Practices

### 1. Use Async All the Way

```csharp
// Avoid
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = _repository.GetById(id); // Synchronous call
    return user != null
        ? Result<User>.Success(user)
        : Result<User>.Failure(ErrorCode.NotFound);
}

// Prefer
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = await _repository.GetByIdAsync(id);
    return user != null
        ? Result<User>.Success(user)
        : Result<User>.Failure(ErrorCode.NotFound);
}
```

### 2. Handle Async Exceptions

```csharp
// Avoid
public async Task<Result<User>> GetUserAsync(int id)
{
    var user = await _repository.GetByIdAsync(id);
    return Result<User>.Success(user);
}

// Prefer
public async Task<Result<User>> GetUserAsync(int id)
{
    try
    {
        var user = await _repository.GetByIdAsync(id);
        return Result<User>.Success(user);
    }
    catch (Exception ex)
    {
        return Result<User>.Failure(
            ErrorCode.Unknown,
            "An error occurred while getting user",
            new Dictionary<string, object> { { "Exception", ex } }
        );
    }
}
```

### 3. Use ConfigureAwait

```csharp
public async Task<Result<User>> GetUserAsync(int id)
{
    try
    {
        var user = await _repository.GetByIdAsync(id).ConfigureAwait(false);
        return Result<User>.Success(user);
    }
    catch (Exception ex)
    {
        return Result<User>.Failure(
            ErrorCode.Unknown,
            "An error occurred while getting user",
            new Dictionary<string, object> { { "Exception", ex } }
        );
    }
}
```

## Common Use Cases

### 1. API Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var result = await _userService.GetUserAsync(id);
        return result.Match(
            user => Ok(user),
            error => StatusCode(GetStatusCode(error.Code), error.Message)
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var result = await _userService.CreateUserAsync(user);
        return result.Match(
            createdUser => CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser),
            error => StatusCode(GetStatusCode(error.Code), error.Message)
        );
    }
}
```

### 2. Database Operations

```csharp
public class UserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> GetUserAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            return user != null
                ? Result<User>.Success(user)
                : Result<User>.Failure(ErrorCode.NotFound, $"User {id} not found");
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(
                ErrorCode.DatabaseError,
                "Failed to get user",
                new Dictionary<string, object> { { "Exception", ex } }
            );
        }
    }

    public async Task<Result<User>> CreateUserAsync(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(
                ErrorCode.DatabaseError,
                "Failed to create user",
                new Dictionary<string, object> { { "Exception", ex } }
            );
        }
    }
}
```

### 3. External Service Calls

```csharp
public class ExternalService
{
    private readonly HttpClient _httpClient;

    public ExternalService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<WeatherData>> GetWeatherAsync(string city)
    {
        try
        {
            var response = await _httpClient.GetAsync($"weather/{city}");
            if (!response.IsSuccessStatusCode)
            {
                return Result<WeatherData>.Failure(
                    ErrorCode.NetworkError,
                    $"Failed to get weather data: {response.StatusCode}"
                );
            }

            var content = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<WeatherData>(content);
            return Result<WeatherData>.Success(weatherData);
        }
        catch (Exception ex)
        {
            return Result<WeatherData>.Failure(
                ErrorCode.Unknown,
                "Failed to get weather data",
                new Dictionary<string, object> { { "Exception", ex } }
            );
        }
    }
}
```

## Next Steps

- [LINQ Support](linq-support.md)
- [Extensions](extensions.md)
- [Examples](examples/basic-examples.md) 