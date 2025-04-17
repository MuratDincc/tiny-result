namespace TinyResult.Configurations;

public class CircuitBreakerSettings
{
    public int FailureThreshold { get; set; } = 3;
    public TimeSpan ResetTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int HalfOpenSuccessThreshold { get; set; } = 2;
} 