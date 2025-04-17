# Installation

## Prerequisites

- .NET 6.0, 7.0, 8.0, or 9.0
- Visual Studio 2022 or later (recommended)
- Visual Studio Code with C# extension (alternative)

## Dependencies

TinyResult requires the following dependencies:
- System.Text.Json (7.0.3)

## Package Installation

### NuGet Package Manager

1. Open your project in Visual Studio
2. Right-click on the project in Solution Explorer
3. Select "Manage NuGet Packages"
4. Search for "TinyResult"
5. Click "Install"

### .NET CLI

```bash
dotnet add package TinyResult
```

### PackageReference

Add the following to your project file:

```xml
<ItemGroup>
    <PackageReference Include="TinyResult" Version="1.0.0" />
</ItemGroup>
```

## Project Setup

### 1. Create a New Project

```bash
dotnet new console -n MyApp
cd MyApp
```

### 2. Add TinyResult Package

```bash
dotnet add package TinyResult
```

### 3. Add Using Directive

Add the following using directive to your code:

```csharp
using TinyResult;
```

## Configuration

### Basic Configuration

No special configuration is required to start using TinyResult. The library is ready to use after installation.

### Optional Configuration

If you want to customize the behavior of TinyResult, you can do so by creating a configuration class:

```csharp
public class TinyResultConfig
{
    public static void Configure()
    {
        // Configure default error messages
        Result.DefaultErrorMessages = new Dictionary<ErrorCode, string>
        {
            { ErrorCode.Unknown, "An unknown error occurred" },
            { ErrorCode.NotFound, "The requested resource was not found" },
            { ErrorCode.ValidationError, "Validation failed" }
        };

        // Configure logging
        Result.OnError = error => Console.WriteLine($"Error: {error.Message}");
    }
}
```

## Verification

To verify that TinyResult is installed correctly, create a simple test:

```csharp
using TinyResult;

class Program
{
    static void Main(string[] args)
    {
        var result = Result<int>.Success(42);
        result.Match(
            value => Console.WriteLine($"Success: {value}"),
            error => Console.WriteLine($"Error: {error.Message}")
        );
    }
}
```

Run the program and you should see the output:

```
Success: 42
```

## Troubleshooting

### Common Issues

1. **Package Not Found**
   - Ensure you have the correct NuGet source configured
   - Check your internet connection
   - Verify the package name is correct

2. **Version Conflicts**
   - Check for version conflicts with other packages
   - Update to the latest version of TinyResult
   - Consider using a specific version

3. **Build Errors**
   - Clean and rebuild your solution
   - Check for missing references
   - Verify your .NET version

### Getting Help

If you encounter any issues:

1. Check the [documentation](https://github.com/MuratDincc/TinyResult/docs)
2. Search for existing issues on GitHub
3. Create a new issue if needed

## Next Steps

- [Basic Usage](basic-usage.md)
- [Features](features/result-pattern.md)
- [Examples](examples/basic-examples.md) 