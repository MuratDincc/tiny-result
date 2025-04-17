namespace TinyResult.Configurations;

public class SecuritySettings
{
    public string EncryptionKey { get; set; } = "YourSecretKey123!";
    public int VisibleChars { get; set; } = 4;
    public string MaskCharacter { get; set; } = "*";
} 