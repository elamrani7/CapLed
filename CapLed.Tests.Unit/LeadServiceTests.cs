using FluentAssertions;
using Moq;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Services;
using StockManager.Core.Domain.Entities.Commercial;
using StockManager.Core.Domain.Exceptions;

namespace CapLed.Tests.Unit;

public class LeadServiceTests
{
    private readonly Mock<ILeadRepository> _leadRepoMock;
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly LeadService _sut;

    public LeadServiceTests()
    {
        _leadRepoMock = new Mock<ILeadRepository>();
        _clientRepoMock = new Mock<IClientRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _sut = new LeadService(_leadRepoMock.Object, _clientRepoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task CreateLeadAsync_EmptyCart_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateLeadDto
        {
            EmailClient = "test@capled.com",
            Lignes = new List<CreateLigneLeadDto>() // Empty cart
        };

        // Act
        Func<Task> act = async () => await _sut.CreateLeadAsync(dto);

        // Assert
        var exception = await act.Should().ThrowAsync<DomainException>();
        exception.Which.Code.Should().Be("LEAD_EMPTY_CART");
    }

    [Fact]
    public async Task CreateLeadAsync_ValidQuote_CreatesLead()
    {
        // Arrange
        var dto = new CreateLeadDto
        {
            EmailClient = "test@capled.com",
            Lignes = new List<CreateLigneLeadDto>
            {
                new CreateLigneLeadDto { ArticleId = 1, QuantiteDemandee = 10 }
            }
        };

        _clientRepoMock.Setup(x => x.GetByEmailAsync(dto.EmailClient))
            .ReturnsAsync(new Client { Id = 1, Email = dto.EmailClient });

        _leadRepoMock.Setup(x => x.CountByYearAsync(It.IsAny<int>())).ReturnsAsync(5);
        _leadRepoMock.Setup(x => x.AddAsync(It.IsAny<Lead>()))
            .Callback<Lead>(l => l.Id = 99); // Simulate auto ID
        _leadRepoMock.Setup(x => x.GetByIdAsync(99))
            .ReturnsAsync(new Lead { Id = 99, Statut = "NOUVEAU" });

        // Act
        var result = await _sut.CreateLeadAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Statut.Should().Be("NOUVEAU");
        _uowMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _uowMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
        _leadRepoMock.Verify(x => x.AddAsync(It.Is<Lead>(l => l.Lignes.Count == 1)), Times.Once);
    }
}
