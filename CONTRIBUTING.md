# Contributing to TinyResult

Thank you for your interest in contributing to TinyResult! This document provides guidelines and instructions for contributing to the project.

## Prerequisites

- .NET 9.0 SDK or later
- Your favorite IDE (Visual Studio, VS Code, Rider, etc.)

## Code of Conduct

By participating in this project, you agree to abide by the [Code of Conduct](CODE_OF_CONDUCT.md).

## How to Contribute

1. Fork the repository
2. Create a new branch for your feature or bugfix
3. Make your changes
4. Write or update tests
5. Update documentation
6. Submit a pull request

## Development Setup

1. Clone the repository
```bash
git clone https://github.com/MuratDincc/TinyResult.git
cd TinyResult
```

2. Install dependencies
```bash
dotnet restore
```

3. Build the project
```bash
dotnet build
```

4. Run tests
```bash
dotnet test
```

## Coding Standards

- Follow C# coding conventions
- Use meaningful variable and method names
- Write clear and concise comments
- Keep methods small and focused
- Use appropriate access modifiers
- Follow SOLID principles
- Target .NET 9.0 features and capabilities

## Testing

- Write unit tests for new features
- Ensure all tests pass
- Maintain high test coverage
- Use appropriate test naming conventions

## Documentation

- Update README.md for significant changes
- Add or update examples in docs/examples
- Update API documentation in docs/api-reference
- Add new guides if necessary

## Pull Request Process

1. Ensure your code builds successfully
2. Run all tests and ensure they pass
3. Update documentation as needed
4. Submit a pull request with a clear description
5. Address any feedback or requested changes

## Release Process

1. Update version in TinyResult.csproj
2. Update CHANGELOG.md
3. Create a new release tag
4. Build and publish the NuGet package

## Questions?

If you have any questions, please open an issue or contact the maintainers. 