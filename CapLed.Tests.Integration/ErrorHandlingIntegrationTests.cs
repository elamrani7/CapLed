using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StockManager.Infrastructure.Persistence;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Entities.Commercial;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace CapLed.Tests.Integration;

/// <summary>
/// Tests d'intégration supplémentaires : validation d'erreurs, panier vide, doublons email.
/// </summary>
public class ErrorHandlingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ErrorHandlingIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostLead_EmptyCart_Returns400_WithStandardError()
    {
        var payload = new
        {
            nomClient = "Client Test",
            emailClient = "empty@test.com",
            sourceAcquisition = "SITE_WEB",
            lignes = new object[] { } // Empty!
        };

        var response = await _client.PostAsJsonAsync("/api/leads", payload);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().MatchRegex("VALIDATION_ERROR|LEAD_EMPTY_CART");
    }

    [Fact]
    public async Task PostRegister_DuplicateEmail_Returns409()
    {
        var dto = new
        {
            nom = "Premier",
            prenom = "User",
            email = "duplicate@test.com",
            telephone = "0612345678",
            societe = "TestSARL",
            password = "Password123!"
        };

        // First register succeeds
        var first = await _client.PostAsJsonAsync("/api/v1/ClientAuth/register", dto);
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        // Second register with same email should conflict
        var second = await _client.PostAsJsonAsync("/api/v1/ClientAuth/register", dto);
        var content = await second.Content.ReadAsStringAsync();

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
        content.Should().Contain("EMAIL_ALREADY_EXISTS");
    }

    [Fact]
    public async Task PostLogin_WrongPassword_ReturnsBadRequest()
    {
        // Register first
        var registerDto = new
        {
            nom = "LoginTest",
            prenom = "User",
            email = "wrongpwd@test.com",
            telephone = "0600000000",
            societe = "TestCorp",
            password = "CorrectPassword1!"
        };
        await _client.PostAsJsonAsync("/api/v1/ClientAuth/register", registerDto);

        // Try login with wrong password
        var loginDto = new { email = "wrongpwd@test.com", password = "WrongPassword99!" };
        var response = await _client.PostAsJsonAsync("/api/v1/ClientAuth/login", loginDto);
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Contain("INVALID_CREDENTIALS");
    }
}
