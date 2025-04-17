namespace TinyResult.Enums;

public enum ErrorCode
{
    None,
    ValidationError,
    NotFound,
    InvalidOperation,
    NetworkError,
    DatabaseError,
    SerializationError,
    DeserializationError,
    EncryptionError,
    DecryptionError,
    CompressionError,
    DecompressionError,
    CircuitBreakerOpen,
    CircuitBreakerError,
    TimeoutError,
    Unknown,
    Exception,
    Timeout
} 