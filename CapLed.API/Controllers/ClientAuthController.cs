using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockManager.Core.Application.DTOs.Auth;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Commercial;
using StockManager.Core.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockManager.API.Controllers;

/// <summary>
/// Authentification dédiée aux clients du site public.
/// Totalement isolée de AuthController (back-office / WPF).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ClientAuthController : ControllerBase
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<Client> _passwordHasher;

    public ClientAuthController(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<Client>();
    }

    // ═══════════════════════════════════════════════════════════════════
    // POST /api/v1/ClientAuth/register
    // ═══════════════════════════════════════════════════════════════════
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ClientRegisterDto dto)
    {
        // 1) Vérifier si l'e-mail est déjà utilisé
        var existing = await _clientRepository.GetByEmailAsync(dto.Email);
        if (existing != null)
        {
            // Si le client existe mais n'a pas de mot de passe (créé via CRM),
            // on lui permet de "revendiquer" son compte.
            if (existing.PasswordHash == null)
            {
                existing.PasswordHash = _passwordHasher.HashPassword(existing, dto.Password);
                existing.ConfirmationToken = Guid.NewGuid().ToString("N");
                existing.TokenExpiry = DateTime.UtcNow.AddHours(24);
                existing.IsEmailConfirmed = false;

                await _clientRepository.UpdateAsync(existing);
                await _unitOfWork.SaveChangesAsync();

                var claimLink = BuildConfirmationLink(existing.Email, existing.ConfirmationToken);
                await _emailService.SendConfirmationEmailAsync(existing.Email, existing.Nom, claimLink);
                return Ok(new { message = "Compte associé. Veuillez vérifier votre boîte e-mail pour confirmer." });
            }

            throw new ConflictException("EMAIL_ALREADY_EXISTS", "Un compte avec cet e-mail existe déjà.");
        }

        // 2) Créer un nouveau client
        var client = new Client
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Societe = dto.Societe,
            CreatedAt = DateTime.UtcNow,
            IsEmailConfirmed = false,
            ConfirmationToken = Guid.NewGuid().ToString("N"),
            TokenExpiry = DateTime.UtcNow.AddHours(24)
        };
        client.PasswordHash = _passwordHasher.HashPassword(client, dto.Password);

        await _clientRepository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        // 3) Envoyer l'e-mail de confirmation
        var confirmLink = BuildConfirmationLink(client.Email, client.ConfirmationToken);
        await _emailService.SendConfirmationEmailAsync(client.Email, client.Nom, confirmLink);
        return Ok(new { message = "Inscription réussie ! Veuillez vérifier votre boîte e-mail pour confirmer votre compte." });
    }

    // ═══════════════════════════════════════════════════════════════════
    // GET /api/v1/ClientAuth/confirm-email?token=...&email=...
    // ═══════════════════════════════════════════════════════════════════
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            return BadRequest(new { error = "Lien de confirmation invalide." });

        var client = await _clientRepository.GetByEmailAsync(email);
        if (client == null)
            throw new NotFoundException("CLIENT_NOT_FOUND", "Aucun compte trouvé pour cet e-mail.");

        if (client.IsEmailConfirmed)
            return Ok(new { message = "Votre compte est déjà confirmé. Vous pouvez vous connecter." });

        if (client.ConfirmationToken != token)
            throw new DomainException("TOKEN_INVALID", "Le lien de confirmation est invalide.");

        if (client.TokenExpiry.HasValue && client.TokenExpiry.Value < DateTime.UtcNow)
            throw new DomainException("TOKEN_EXPIRED", "Le lien de confirmation a expiré. Veuillez demander un nouveau lien.");

        // Confirmer le compte
        client.IsEmailConfirmed = true;
        client.ConfirmationToken = null;
        client.TokenExpiry = null;

        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Votre adresse e-mail a été confirmée avec succès ! Vous pouvez maintenant vous connecter." });
    }

    // ═══════════════════════════════════════════════════════════════════
    // POST /api/v1/ClientAuth/login
    // ═══════════════════════════════════════════════════════════════════
    [HttpPost("login")]
    public async Task<ActionResult<ClientLoginResponseDto>> Login([FromBody] ClientLoginDto dto)
    {
        var client = await _clientRepository.GetByEmailAsync(dto.Email);

        if (client == null || client.PasswordHash == null)
            throw new DomainException("INVALID_CREDENTIALS", "E-mail ou mot de passe incorrect.");

        var verifyResult = _passwordHasher.VerifyHashedPassword(client, client.PasswordHash, dto.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new DomainException("INVALID_CREDENTIALS", "E-mail ou mot de passe incorrect.");

        if (!client.IsEmailConfirmed)
            throw new ForbiddenException("EMAIL_NOT_CONFIRMED",
                "Veuillez confirmer votre adresse e-mail avant de vous connecter. Vérifiez votre boîte de réception.");

        // Générer le JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "CapLedSecretKey_ChangeInProduction_2024");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{client.Prenom} {client.Nom}".Trim()),
            new Claim(ClaimTypes.Email, client.Email),
            new Claim(ClaimTypes.Role, "CLIENT_PUBLIC")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new ClientLoginResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            ExpiresAt = tokenDescriptor.Expires!.Value,
            Email = client.Email,
            FullName = $"{client.Prenom} {client.Nom}".Trim(),
            ClientId = client.Id
        });
    }

    // ═══════════════════════════════════════════════════════════════════
    // POST /api/v1/ClientAuth/resend-confirmation
    // ═══════════════════════════════════════════════════════════════════
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ClientLoginDto dto)
    {
        var client = await _clientRepository.GetByEmailAsync(dto.Email);

        if (client == null || client.PasswordHash == null)
            return NotFound(new { error = "Aucun compte trouvé pour cet e-mail." });

        if (client.IsEmailConfirmed)
            return Ok(new { message = "Votre compte est déjà confirmé." });

        // Vérifier le mot de passe avant de renvoyer
        var verifyResult = _passwordHasher.VerifyHashedPassword(client, client.PasswordHash, dto.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
            return Unauthorized(new { error = "Mot de passe incorrect." });

        // Regénérer le token
        client.ConfirmationToken = Guid.NewGuid().ToString("N");
        client.TokenExpiry = DateTime.UtcNow.AddHours(24);

        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();

        var confirmLink = BuildConfirmationLink(client.Email, client.ConfirmationToken);
        await _emailService.SendConfirmationEmailAsync(client.Email, client.Nom, confirmLink);
        return Ok(new { message = "Un nouvel e-mail de confirmation a été envoyé." });
    }

    // ── Helpers ─────────────────────────────────────────────────────────
    private string BuildConfirmationLink(string email, string token)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        return $"{frontendUrl}/confirm-email?token={token}&email={Uri.EscapeDataString(email)}";
    }
}
