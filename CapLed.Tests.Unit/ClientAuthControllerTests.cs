using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using StockManager.API.Controllers;
using StockManager.Core.Application.DTOs.Auth;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Entities.Commercial;
using StockManager.Core.Domain.Exceptions;
using System.Threading.Tasks;

namespace CapLed.Tests.Unit;

public class ClientAuthControllerTests
{
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IEmailService> _emailMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly ClientAuthController _sut;

    public ClientAuthControllerTests()
    {
        _clientRepoMock = new Mock<IClientRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _emailMock = new Mock<IEmailService>();
        _configMock = new Mock<IConfiguration>();

        _sut = new ClientAuthController(
            _clientRepoMock.Object,
            _uowMock.Object,
            _emailMock.Object,
            _configMock.Object
        );
    }

    [Fact]
    public async Task Register_RequiresEmailConfirmation()
    {
        // Arrange
        var dto = new ClientRegisterDto
        {
            Nom = "Test",
            Email = "test@capled.com",
            Password = "Password123!"
        };

        _clientRepoMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((Client?)null);

        // Act
        var result = await _sut.Register(dto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _clientRepoMock.Verify(x => x.AddAsync(It.Is<Client>(c => c.IsEmailConfirmed == false && c.ConfirmationToken != null)), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _emailMock.Verify(x => x.SendConfirmationEmailAsync(dto.Email, dto.Nom, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithoutConfirmation_ThrowsForbiddenException()
    {
        // Arrange
        var client = new Client
        {
            Email = "test@capled.com",
            PasswordHash = "dummyhash", // Actually we need a valid hash or we will fail on Invalid Credentials
            IsEmailConfirmed = false
        };

        // We can't mock PasswordHasher directly inside the controller because it's instantiated inside the controller.
        // So we will just simulate what verify does. Wait, if we use a real Hash in the test, it works.
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Client>();
        client.PasswordHash = hasher.HashPassword(client, "Password123!");

        var dto = new ClientLoginDto { Email = "test@capled.com", Password = "Password123!" };

        _clientRepoMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync(client);

        // Act
        Func<Task> act = async () => await _sut.Login(dto);

        // Assert
        var exception = await act.Should().ThrowAsync<ForbiddenException>();
        exception.Which.Code.Should().Be("EMAIL_NOT_CONFIRMED");
    }
}
