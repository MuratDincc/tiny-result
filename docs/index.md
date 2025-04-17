# TinyResult

A lightweight and powerful Result Pattern implementation for .NET that helps you write more robust and maintainable code.

## 🚀 Features

- **Result Pattern Implementation**: Clean and type-safe way to handle success and failure cases
- **Error Handling**: Comprehensive error handling with detailed error information
- **Validation Support**: Built-in validation support with detailed validation results
- **Async Support**: Full async/await support for all operations
- **LINQ Support**: LINQ-style operations for working with results
- **Pipeline Support**: Fluent API for chaining operations and handling results
- **Extensible**: Easy to extend with custom functionality
- **Lightweight**: Minimal dependencies and overhead
- **Wide .NET Support**: Compatible with .NET 6.0, 7.0, 8.0, and 9.0

## 🛠️ Requirements

- .NET 6.0, 7.0, 8.0, or 9.0

## 📦 Packages

### Core Package
```bash
dotnet add package TinyResult
```

## 💡 Quick Start

```csharp
// Create a successful result
var success = Result<string>.Success("Hello, World!");

// Create a failed result
var failure = Result<string>.Failure("Something went wrong");

// Handle the result
var message = success.Match(
    value => $"Success: {value}",
    error => $"Error: {error.Message}"
);
```

## 🎯 Target Audience

This library is designed for .NET developers who want to:

- Write more robust and maintainable code
- Handle errors in a clean and type-safe way
- Implement the Result Pattern in their applications
- Improve their code quality and reliability
- Make their code more testable

## 📚 Documentation Structure

### Getting Started

- [Introduction](getting-started/introduction.md)
- [Installation](getting-started/installation.md)
- [Basic Usage](getting-started/basic-usage.md)

### Features

- [Result Pattern](features/result-pattern.md)
- [Error Handling](features/error-handling.md)
- [Validation](features/validation.md)
- [Async Support](features/async-support.md)
- [LINQ Support](features/linq-support.md)
- [Extensions](features/extensions.md)
- [Pipeline](features/pipeline.md)

### Examples

- [Basic Examples](examples/basic-examples.md)
- [Advanced Examples](examples/advanced-examples.md)
- [Real World Scenarios](examples/real-world-scenarios.md)

### API Reference

- [Result](api-reference/result.md)
- [Error](api-reference/error.md)
- [ValidationResult](api-reference/validation-result.md)
- [Extensions](api-reference/extensions.md)

### Advanced Topics

- [Best Practices](advanced-topics/best-practices.md)
- [Performance](advanced-topics/performance.md)
- [Testing](advanced-topics/testing.md)
- [Integration](advanced-topics/integration.md)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Contact

- GitHub: [MuratDincc](https://github.com/MuratDincc)
- LinkedIn: [Murat Dinç](https://www.linkedin.com/in/muratdincc)
- Medium: [Murat Dinç](https://medium.com/@muratdincc)

## Next Steps

- [Getting Started](getting-started/introduction.md)
- [Features](features/result-pattern.md)
- [Examples](examples/basic-examples.md)
- [API Reference](api-reference/result.md)
- [Advanced Topics](advanced-topics/best-practices.md) 