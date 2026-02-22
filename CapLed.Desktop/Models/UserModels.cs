using System;

namespace CapLed.Desktop.Models;

/// <summary>
/// Mirrors UserReadDto — used for displaying user information.
/// </summary>
public class UserModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "STOCK_MANAGER"; // Handled as string for easier UI binding if needed, or enum
    public bool IsActive { get; set; } = true;

    // Helper for UI display
    public string DisplayRole => Role == "ADMIN" ? "Administrateur" : "Gestionnaire Stock";
}

/// <summary>
/// Mirrors UserCreateDto — used in the Add User form.
/// </summary>
public class UserCreateModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = "STOCK_MANAGER";
}

/// <summary>
/// Mirrors UserUpdateDto — used in the Edit User form.
/// </summary>
public class UserUpdateModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "STOCK_MANAGER";
    public bool IsActive { get; set; } = true;
}
