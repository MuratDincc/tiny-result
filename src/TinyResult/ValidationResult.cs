namespace TinyResult;

/// <summary>
/// Represents the result of a validation operation.
/// Contains information about validation errors and whether the validation was successful.
/// </summary>
public class ValidationResult
{
    private readonly Dictionary<string, string> _errors = new();

    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid => !_errors.Any();

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IReadOnlyDictionary<string, string> Errors => _errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    private ValidationResult()
    {
    }

    /// <summary>
    /// Creates a new validation result.
    /// </summary>
    /// <returns>A new <see cref="ValidationResult"/> instance.</returns>
    public static ValidationResult Create()
    {
        return new ValidationResult();
    }

    /// <summary>
    /// Adds a validation error.
    /// </summary>
    /// <param name="key">The key of the error.</param>
    /// <param name="message">The error message.</param>
    /// <returns>The current <see cref="ValidationResult"/> instance.</returns>
    public ValidationResult AddError(string key, string message)
    {
        _errors[key] = message;
        return this;
    }

    /// <summary>
    /// Adds multiple validation errors.
    /// </summary>
    /// <param name="errors">The errors to add.</param>
    /// <returns>The current <see cref="ValidationResult"/> instance.</returns>
    public ValidationResult AddErrors(IEnumerable<KeyValuePair<string, string>> errors)
    {
        foreach (var error in errors)
        {
            _errors[error.Key] = error.Value;
        }
        return this;
    }

    /// <summary>
    /// Clears all validation errors.
    /// </summary>
    /// <returns>The current <see cref="ValidationResult"/> instance.</returns>
    public ValidationResult Clear()
    {
        _errors.Clear();
        return this;
    }

    /// <summary>
    /// Creates a successful validation result with no errors.
    /// </summary>
    /// <returns>A new successful <see cref="ValidationResult"/> instance.</returns>
    public static ValidationResult Success()
    {
        return new ValidationResult();
    }

    /// <summary>
    /// Creates a failed validation result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the validation to fail.</param>
    /// <returns>A new failed <see cref="ValidationResult"/> instance.</returns>
    public static ValidationResult Failure(Error error)
    {
        return new ValidationResult().AddError("Error", error.Message);
    }

    /// <summary>
    /// Combines multiple validation results into a single validation result.
    /// </summary>
    /// <param name="results">The validation results to combine.</param>
    /// <returns>A new validation result containing all errors from the input results.</returns>
    public static ValidationResult Combine(params ValidationResult[] results)
    {
        var combined = new ValidationResult();
        foreach (var result in results)
        {
            foreach (var error in result.Errors)
            {
                combined._errors[error.Key] = error.Value;
            }
        }
        return combined;
    }
} 