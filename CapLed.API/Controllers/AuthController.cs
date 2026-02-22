using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        
        if (user == null)
        {
            Console.WriteLine($"[AUTH DEBUG] User not found: {loginDto.Email}");
            return Unauthorized(new { error = "Invalid email or password" });
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            Console.WriteLine($"[AUTH DEBUG] Password mismatch for: {loginDto.Email}");
            return Unauthorized(new { error = "Invalid email or password" });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "CapLedSecretKey_ChangeInProduction_2024");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new LoginResponseDto
        {
            Token = tokenString,
            ExpiresAt = tokenDescriptor.Expires.Value,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString()
        });
    }
}
