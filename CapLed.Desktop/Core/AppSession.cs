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

    public int UserId { get; set; } = 1;                  // Hardcoded until auth is implemented
    public string UserName { get; set; } = "Admin";
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "ADMIN";            // "ADMIN" or "STOCK_MANAGER"

    public bool IsAdmin => Role == "ADMIN";

    /// <summary>Clear session on logout.</summary>
    public void Clear()
    {
        UserId = 0;
        UserName = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
    }
}
