using System.ComponentModel.DataAnnotations;

namespace StockManager.Core.Application.DTOs.Auth;

public class ClientRegisterDto
{
    [Required(ErrorMessage = "Le nom est requis.")]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Prenom { get; set; }

    [Required(ErrorMessage = "L'e-mail est requis.")]
    [EmailAddress(ErrorMessage = "Adresse e-mail invalide.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis.")]
    [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
    public string Password { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telephone { get; set; }

    [StringLength(200)]
    public string? Societe { get; set; }
}

public class ClientLoginDto
{
    [Required(ErrorMessage = "L'e-mail est requis.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis.")]
    public string Password { get; set; } = string.Empty;
}

public class ClientLoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int ClientId { get; set; }
}
