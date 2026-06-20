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

[ApiController]
[Route("api/v1/[controller]")]
public class ClientAuthController : ControllerBase
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<Client> _passwordHasher;

    private bool RequireEmailConfirmation =>
        bool.TryParse(_configuration["ClientAuth:RequireEmailConfirmation"], out var require) && require;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ClientRegisterDto dto)
    {
        var existing = await _clientRepository.GetByEmailAsync(dto.Email);
        if (existing != null)
        {
            if (existing.PasswordHash == null)
            {
                existing.PasswordHash = _passwordHasher.HashPassword(existing, dto.Password);
                PrepareEmailConfirmation(existing);

                await _clientRepository.UpdateAsync(existing);
                await _unitOfWork.SaveChangesAsync();

                if (RequireEmailConfirmation)
                {
                    await SendConfirmationEmailAsync(existing);
                    return Ok(new { message = "Compte associe. Veuillez verifier votre boite e-mail pour confirmer." });
                }

                return Ok(new { message = "Compte associe avec succes. Vous pouvez vous connecter." });
            }

            throw new ConflictException("EMAIL_ALREADY_EXISTS", "Un compte avec cet e-mail existe deja.");
        }

        var client = new Client
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Societe = dto.Societe,
            CreatedAt = DateTime.UtcNow
        };

        client.PasswordHash = _passwordHasher.HashPassword(client, dto.Password);
        PrepareEmailConfirmation(client);

        await _clientRepository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        if (RequireEmailConfirmation)
        {
            await SendConfirmationEmailAsync(client);
            return Ok(new { message = "Inscription reussie. Veuillez verifier votre boite e-mail pour confirmer votre compte." });
        }

        return Ok(new { message = "Inscription reussie. Vous pouvez vous connecter." });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            return BadRequest(new { error = "Lien de confirmation invalide." });

        var client = await _clientRepository.GetByEmailAsync(email);
        if (client == null)
            throw new NotFoundException("CLIENT_NOT_FOUND", "Aucun compte trouve pour cet e-mail.");

        if (client.IsEmailConfirmed)
            return Ok(new { message = "Votre compte est deja confirme. Vous pouvez vous connecter." });

        if (client.ConfirmationToken != token)
            throw new DomainException("TOKEN_INVALID", "Le lien de confirmation est invalide.");

        if (client.TokenExpiry.HasValue && client.TokenExpiry.Value < DateTime.UtcNow)
            throw new DomainException("TOKEN_EXPIRED", "Le lien de confirmation a expire. Veuillez demander un nouveau lien.");

        client.IsEmailConfirmed = true;
        client.ConfirmationToken = null;
        client.TokenExpiry = null;

        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Votre adresse e-mail a ete confirmee avec succes. Vous pouvez maintenant vous connecter." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<ClientLoginResponseDto>> Login([FromBody] ClientLoginDto dto)
    {
        var client = await _clientRepository.GetByEmailAsync(dto.Email);

        if (client == null || client.PasswordHash == null)
            throw new DomainException("INVALID_CREDENTIALS", "E-mail ou mot de passe incorrect.");

        var verifyResult = _passwordHasher.VerifyHashedPassword(client, client.PasswordHash, dto.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
            throw new DomainException("INVALID_CREDENTIALS", "E-mail ou mot de passe incorrect.");

        if (RequireEmailConfirmation && !client.IsEmailConfirmed)
            throw new ForbiddenException("EMAIL_NOT_CONFIRMED",
                "Veuillez confirmer votre adresse e-mail avant de vous connecter. Verifiez votre boite de reception.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "CapLedSecretKey_ChangeInProduction_2024");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new(ClaimTypes.Name, $"{client.Prenom} {client.Nom}".Trim()),
            new(ClaimTypes.Email, client.Email),
            new(ClaimTypes.Role, "CLIENT_PUBLIC")
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

    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation([FromBody] ClientLoginDto dto)
    {
        if (!RequireEmailConfirmation)
            return Ok(new { message = "La confirmation e-mail n'est pas activee pour cette version." });

        var client = await _clientRepository.GetByEmailAsync(dto.Email);

        if (client == null || client.PasswordHash == null)
            return NotFound(new { error = "Aucun compte trouve pour cet e-mail." });

        if (client.IsEmailConfirmed)
            return Ok(new { message = "Votre compte est deja confirme." });

        var verifyResult = _passwordHasher.VerifyHashedPassword(client, client.PasswordHash, dto.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
            return Unauthorized(new { error = "Mot de passe incorrect." });

        client.ConfirmationToken = Guid.NewGuid().ToString("N");
        client.TokenExpiry = DateTime.UtcNow.AddHours(24);

        await _clientRepository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();

        await SendConfirmationEmailAsync(client);
        return Ok(new { message = "Un nouvel e-mail de confirmation a ete envoye." });
    }

    private void PrepareEmailConfirmation(Client client)
    {
        client.IsEmailConfirmed = !RequireEmailConfirmation;
        client.ConfirmationToken = RequireEmailConfirmation ? Guid.NewGuid().ToString("N") : null;
        client.TokenExpiry = RequireEmailConfirmation ? DateTime.UtcNow.AddHours(24) : null;
    }

    private async Task SendConfirmationEmailAsync(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.ConfirmationToken)) return;

        var confirmLink = BuildConfirmationLink(client.Email, client.ConfirmationToken);
        await _emailService.SendConfirmationEmailAsync(client.Email, client.Nom, confirmLink);
    }

    private string BuildConfirmationLink(string email, string token)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        return $"{frontendUrl}/confirm-email?token={token}&email={Uri.EscapeDataString(email)}";
    }
}
