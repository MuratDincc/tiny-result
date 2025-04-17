# Real World Scenarios

## 1. E-Commerce System

### Order Processing

```csharp
public class OrderService
{
    public async Task<Result<Order>> ProcessOrderAsync(OrderRequest request)
    {
        return await ResultPipeline<Order>
            .Start(request)
            .Then(ValidateOrderRequest)
            .ThenAsync(async validRequest => await CheckInventoryAsync(validRequest))
            .ThenAsync(async inventory => await CalculateTotalAsync(inventory))
            .ThenAsync(async total => await ProcessPaymentAsync(total))
            .ThenAsync(async payment => await CreateOrderAsync(payment))
            .ThenAsync(async order => await SendConfirmationEmailAsync(order))
            .WithRetry(maxAttempts: 3, delay: TimeSpan.FromSeconds(1))
            .WithTimeout(TimeSpan.FromSeconds(30))
            .BuildAsync();
    }

    private Result<OrderRequest> ValidateOrderRequest(OrderRequest request)
    {
        var validationResult = ValidationResult.Create()
            .AddErrorIf(request.Items.Count == 0, "Items", "Order must contain at least one item")
            .AddErrorIf(request.ShippingAddress == null, "ShippingAddress", "Shipping address is required")
            .AddErrorIf(request.PaymentMethod == null, "PaymentMethod", "Payment method is required");

        return validationResult.IsValid
            ? Result<OrderRequest>.Success(request)
            : Result<OrderRequest>.Failure(
                Error.Create(
                    ErrorCode.ValidationError,
                    "Invalid order request",
                    new Dictionary<string, object> { { "Errors", validationResult.Errors } }
                )
            );
    }
}
```

### Inventory Management

```csharp
public class InventoryService
{
    private readonly ResultCircuitBreaker _circuitBreaker;

    public InventoryService()
    {
        _circuitBreaker = new ResultCircuitBreaker(
            maxFailures: 5,
            resetTimeout: TimeSpan.FromMinutes(5)
        );
    }

    public async Task<Result<Inventory>> UpdateInventoryAsync(InventoryUpdate update)
    {
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            return await ResultPipeline<Inventory>
                .Start(update)
                .Then(ValidateInventoryUpdate)
                .ThenAsync(async validUpdate => await CheckStockLevelsAsync(validUpdate))
                .ThenAsync(async levels => await UpdateStockAsync(levels))
                .ThenAsync(async updated => await LogInventoryChangeAsync(updated))
                .BuildAsync();
        });
    }
}
```

## 2. Financial Operations

### Money Transfer

```csharp
public class TransferService
{
    public async Task<Result<Transfer>> ProcessTransferAsync(TransferRequest request)
    {
        return await ResultPipeline<Transfer>
            .Start(request)
            .Then(ValidateTransferRequest)
            .ThenAsync(async validRequest => await CheckAccountBalanceAsync(validRequest))
            .ThenAsync(async balance => await VerifyTransferLimitAsync(balance))
            .ThenAsync(async verified => await ExecuteTransferAsync(verified))
            .ThenAsync(async transfer => await SendTransferNotificationAsync(transfer))
            .WithTimeout(TimeSpan.FromSeconds(60))
            .WithRetry(3, TimeSpan.FromSeconds(5))
            .BuildAsync();
    }

    private Result<TransferRequest> ValidateTransferRequest(TransferRequest request)
    {
        var validationResult = ValidationResult.Create()
            .AddErrorIf(request.Amount <= 0, "Amount", "Transfer amount must be positive")
            .AddErrorIf(request.SourceAccountId == request.TargetAccountId, "Accounts", "Source and target accounts must be different")
            .AddErrorIf(string.IsNullOrEmpty(request.Description), "Description", "Transfer description is required");

        return validationResult.IsValid
            ? Result<TransferRequest>.Success(request)
            : Result<TransferRequest>.Failure(
                Error.Create(
                    ErrorCode.ValidationError,
                    "Invalid transfer request",
                    new Dictionary<string, object> { { "Errors", validationResult.Errors } }
                )
            );
    }
}
```

### Credit Application

```csharp
public class CreditApplicationService
{
    public async Task<Result<CreditApplication>> ProcessApplicationAsync(CreditApplication application)
    {
        return await ResultPipeline<CreditApplication>
            .StartWith(application)
            .Then(ValidateApplication)
            .ThenAsync(async validApp => await CheckCreditScoreAsync(validApp))
            .ThenAsync(async score => await VerifyIncomeAsync(score))
            .ThenAsync(async income => await CalculateRiskAsync(income))
            .ThenAsync(async risk => await MakeDecisionAsync(risk))
            .ThenAsync(async decision => await NotifyApplicantAsync(decision))
            .WithTimeout(TimeSpan.FromMinutes(5))
            .BuildAsync();
    }
}
```

## 3. User Management System

### User Registration

```csharp
public class UserService
{
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

    private Result<UserRegistration> ValidateUserData(UserRegistration request)
    {
        var validationResult = ValidationResult.Create()
            .AddErrorIf(string.IsNullOrEmpty(request.Email), "Email", "Email is required")
            .AddErrorIf(!IsValidEmail(request.Email), "Email", "Invalid email format")
            .AddErrorIf(string.IsNullOrEmpty(request.Password), "Password", "Password is required")
            .AddErrorIf(request.Password.Length < 8, "Password", "Password must be at least 8 characters")
            .AddErrorIf(request.Age < 18, "Age", "User must be at least 18 years old");

        return validationResult.IsValid
            ? Result<UserRegistration>.Success(request)
            : Result<UserRegistration>.Failure(
                Error.Create(
                    ErrorCode.ValidationError,
                    "Invalid user registration data",
                    new Dictionary<string, object> { { "Errors", validationResult.Errors } }
                )
            );
    }
}
```

## 4. API Gateway

### Request Processing

```csharp
public class ApiGateway
{
    public async Task<Result<ApiResponse>> ProcessRequestAsync(ApiRequest request)
    {
        return await ResultPipeline<ApiResponse>
            .Start(request)
            .Then(ValidateApiRequest)
            .ThenAsync(async validRequest => await AuthenticateRequestAsync(validRequest))
            .ThenAsync(async authenticated => await AuthorizeRequestAsync(authenticated))
            .ThenAsync(async authorized => await RateLimitRequestAsync(authorized))
            .ThenAsync(async limited => await ProcessRequestAsync(limited))
            .ThenAsync(async response => await LogRequestAsync(response))
            .WithTimeout(TimeSpan.FromSeconds(10))
            .WithRetry(2, TimeSpan.FromSeconds(1))
            .BuildAsync();
    }

    private Result<ApiRequest> ValidateApiRequest(ApiRequest request)
    {
        var validationResult = ValidationResult.Create()
            .AddErrorIf(string.IsNullOrEmpty(request.Endpoint), "Endpoint", "API endpoint is required")
            .AddErrorIf(request.Headers == null, "Headers", "Request headers are required")
            .AddErrorIf(request.Timestamp > DateTime.UtcNow.AddMinutes(5), "Timestamp", "Request timestamp is too far in the future");

        return validationResult.IsValid
            ? Result<ApiRequest>.Success(request)
            : Result<ApiRequest>.Failure(
                Error.Create(
                    ErrorCode.ValidationError,
                    "Invalid API request",
                    new Dictionary<string, object> { { "Errors", validationResult.Errors } }
                )
            );
    }
}
```

## 5. File Processing System

### File Upload and Processing

```csharp
public class FileService
{
    public async Task<Result<ProcessedFile>> ProcessFileAsync(FileUpload request)
    {
        return await ResultPipeline<ProcessedFile>
            .Start(request)
            .Then(ValidateFile)
            .ThenAsync(async validFile => await SaveFileAsync(validFile))
            .ThenAsync(async saved => await ProcessFileContentAsync(saved))
            .ThenAsync(async processed => await GenerateThumbnailAsync(processed))
            .ThenAsync(async thumbnail => await UpdateDatabaseAsync(thumbnail))
            .WithTimeout(TimeSpan.FromMinutes(5))
            .WithRetry(3, TimeSpan.FromSeconds(10))
            .BuildAsync();
    }

    private Result<FileUpload> ValidateFile(FileUpload request)
    {
        var validationResult = ValidationResult.Create()
            .AddErrorIf(request.File == null, "File", "No file was uploaded")
            .AddErrorIf(request.File.Length == 0, "File", "Uploaded file is empty")
            .AddErrorIf(request.File.Length > 10 * 1024 * 1024, "File", "File size exceeds 10MB limit")
            .AddErrorIf(!IsValidFileType(request.File.ContentType), "File", "Invalid file type");

        return validationResult.IsValid
            ? Result<FileUpload>.Success(request)
            : Result<FileUpload>.Failure(
                Error.Create(
                    ErrorCode.ValidationError,
                    "Invalid file upload",
                    new Dictionary<string, object> { { "Errors", validationResult.Errors } }
                )
            );
    }
}
```

## Next Steps

- [Basic Examples](basic-examples.md)
- [Advanced Examples](advanced-examples.md)
- [API Reference](api-reference/result.md) 