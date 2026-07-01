using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StockManager.Core.Application.DTOs.Auth;
using StockManager.Infrastructure.Persistence;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace CapLed.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterAndConfirmFlow_WorksProperly()
    {
        // 1. Arrange & ACT: Register
        var registerDto = new ClientRegisterDto
        {
            Nom = "Integration",
            Prenom = "User",
            Email = "integration@test.com",
            Telephone = "000000000",
            Societe = "Integrations Inc",
            Password = "Password123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/ClientAuth/register", registerDto);

        // Assert Registration
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Fetch user directly from DB to get the confirmation token
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StockManagementDbContext>();
        var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.Clients, c => c.Email == "integration@test.com");

        user.Should().NotBeNull();
        user!.IsEmailConfirmed.Should().BeFalse();
        user.ConfirmationToken.Should().NotBeNullOrEmpty();

        // 2. Arrange & ACT: Confirm Email
        var confirmUrl = $"/api/v1/ClientAuth/confirm-email?token={user.ConfirmationToken}&email={user.Email}";
        var confirmResponse = await _client.GetAsync(confirmUrl);

        // Assert Confirmation
        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Fetch again and verify state changed
        using var scope2 = _factory.Services.CreateScope();
        var db2 = scope2.ServiceProvider.GetRequiredService<StockManagementDbContext>();
        var updatedUser = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db2.Clients, c => c.Email == "integration@test.com");

        updatedUser.Should().NotBeNull();
        updatedUser!.IsEmailConfirmed.Should().BeTrue();
        updatedUser.ConfirmationToken.Should().BeNull();
    }
}
