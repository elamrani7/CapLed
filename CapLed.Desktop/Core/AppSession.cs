namespace CapLed.Desktop.Core;

/// <summary>
/// Singleton session — holds the currently authenticated user's information.
/// Populated after a successful login (or hardcoded for now until auth is implemented).
/// </summary>
public class AppSession
{
    private static AppSession? _instance;
    public static AppSession Current => _instance ??= new AppSession();

    private AppSession() { }

    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? JwtToken { get; set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(JwtToken);

    public bool IsAdmin => Role == "ADMIN";

    /// <summary>Clear session on logout.</summary>
    public void Clear()
    {
        UserId = 0;
        UserName = string.Empty;
        FullName = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
        JwtToken = null;
    }
}
