using StockManager.Core.Domain.Enums;

namespace StockManager.Core.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    // Navigation Properties
    public virtual ICollection<StockMovement> PerformedMovements { get; set; } = new List<StockMovement>();
}

