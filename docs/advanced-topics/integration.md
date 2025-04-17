# Integration

## Overview

This section explains how to integrate the TinyResult library with other libraries and frameworks.

## ASP.NET Core Integration

### Controller Integration
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
            error => error.Code switch
            {
                ErrorCode.NotFound => NotFound(error.Message),
                _ => StatusCode(500, error.Message)
            }
        );
    }
}
```

### Middleware Integration
```csharp
public class ResultMiddleware
{
    private readonly RequestDelegate _next;

    public ResultMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var error = Error.FromException(ex);
            context.Response.StatusCode = GetStatusCode(error.Code);
            await context.Response.WriteAsJsonAsync(new { error.Message });
        }
    }

    private static int GetStatusCode(ErrorCode code) => code switch
    {
        ErrorCode.NotFound => 404,
        ErrorCode.ValidationError => 400,
        _ => 500
    };
}
```

## Entity Framework Core Integration

### Repository Pattern
```csharp
public class UserRepository : IUserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> GetUserAsync(int id)
    {
        return await Result<User>.TryAsync(async () =>
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return Result<User>.Failure(ErrorCode.NotFound, "User not found");
            return Result<User>.Success(user);
        });
    }
}
```

### Unit of Work Pattern
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> SaveChangesAsync()
    {
        return await Result<int>.TryAsync(async () =>
        {
            var changes = await _context.SaveChangesAsync();
            return Result<int>.Success(changes);
        });
    }
}
```

## MediatR Integration

### Command Handler
```csharp
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<User>>
{
    private readonly IUserRepository _repository;
    private readonly IUserValidator _validator;

    public CreateUserCommandHandler(IUserRepository repository, IUserValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request.User);
        if (!validationResult.IsValid)
            return Result<User>.Failure(validationResult.Errors);

        return await _repository.CreateUserAsync(request.User);
    }
}
```

### Query Handler
```csharp
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<User>>
{
    private readonly IUserRepository _repository;

    public GetUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetUserAsync(request.UserId);
    }
}
```

## AutoMapper Integration

### Profile Definition
```csharp
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
    }
}
```

### Service Usage
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> GetUserAsync(int id)
    {
        return await _repository.GetUserAsync(id)
            .MapAsync(user => _mapper.Map<UserDto>(user));
    }
}
```

## FluentValidation Integration

### Validator Definition
```csharp
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).EmailAddress();
    }
}
```

### Service Usage
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IValidator<User> _validator;

    public UserService(IUserRepository repository, IValidator<User> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<User>> CreateUserAsync(User user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
            return Result<User>.Failure(ErrorCode.ValidationError, "Validation failed", errors);
        }

        return await _repository.CreateUserAsync(user);
    }
}
```

## Serilog Integration

### Logger Definition
```csharp
public static class ResultLogger
{
    private static readonly ILogger Log = Serilog.Log.ForContext<ResultLogger>();

    public static void LogResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            Log.Information("Operation succeeded: {Value}", result.Value);
        else
            Log.Error("Operation failed: {Error}", result.Error);
    }
}
```

### Service Usage
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<User>> GetUserAsync(int id)
    {
        var result = await _repository.GetUserAsync(id);
        ResultLogger.LogResult(result);
        return result;
    }
}
```

## Next Steps

- [Best Practices](best-practices.md)
- [Performance](performance.md)
- [Testing](testing.md) 