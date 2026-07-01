using FluentAssertions;
using Moq;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Services;
using StockManager.Core.Domain.Entities.Commercial;
using StockManager.Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapLed.Tests.Unit;

/// <summary>
/// Tests supplémentaires pour LeadService — workflow de statut, numérotation, et client upsert.
/// </summary>
public class LeadServiceWorkflowTests
{
    private readonly Mock<ILeadRepository> _leadRepoMock;
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly LeadService _sut;

    public LeadServiceWorkflowTests()
    {
        _leadRepoMock = new Mock<ILeadRepository>();
        _clientRepoMock = new Mock<IClientRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _sut = new LeadService(_leadRepoMock.Object, _clientRepoMock.Object, _uowMock.Object);
    }

    // ── Numérotation auto (RG-07) ────────────────────────────────────────────

    [Fact]
    public async Task CreateLeadAsync_GeneratesCorrectDevisNumber()
    {
        var dto = new CreateLeadDto
        {
            EmailClient = "test@capled.com",
            NomClient = "Test",
            Lignes = new List<CreateLigneLeadDto>
            {
                new() { ArticleId = 1, QuantiteDemandee = 2 }
            }
        };

        _clientRepoMock.Setup(x => x.GetByEmailAsync(dto.EmailClient))
            .ReturnsAsync(new Client { Id = 1, Email = dto.EmailClient });

        _leadRepoMock.Setup(x => x.CountByYearAsync(It.IsAny<int>())).ReturnsAsync(7);
        _leadRepoMock.Setup(x => x.AddAsync(It.IsAny<Lead>()))
            .Callback<Lead>(l => l.Id = 1);
        _leadRepoMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Lead { Id = 1, NumeroDevis = $"DEVIS-{DateTime.UtcNow.Year}-0008", Statut = "NOUVEAU" });

        var result = await _sut.CreateLeadAsync(dto);

        result.NumeroDevis.Should().StartWith("DEVIS-");
        result.NumeroDevis.Should().EndWith("-0008"); // 7 existing + 1 = 8
    }

    // ── Client upsert: new client created ────────────────────────────────────

    [Fact]
    public async Task CreateLeadAsync_NewEmail_CreatesNewClient()
    {
        var dto = new CreateLeadDto
        {
            EmailClient = "nouveau@test.com",
            NomClient = "Nouveau Client",
            Telephone = "0600000000",
            Societe = "NouveauSARL",
            Lignes = new List<CreateLigneLeadDto>
            {
                new() { ArticleId = 5, QuantiteDemandee = 1 }
            }
        };

        _clientRepoMock.Setup(x => x.GetByEmailAsync(dto.EmailClient)).ReturnsAsync((Client?)null);
        _leadRepoMock.Setup(x => x.CountByYearAsync(It.IsAny<int>())).ReturnsAsync(0);
        _leadRepoMock.Setup(x => x.AddAsync(It.IsAny<Lead>())).Callback<Lead>(l => l.Id = 1);
        _leadRepoMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Lead { Id = 1, Statut = "NOUVEAU" });

        await _sut.CreateLeadAsync(dto);

        _clientRepoMock.Verify(x => x.AddAsync(It.Is<Client>(c =>
            c.Email == "nouveau@test.com" &&
            c.Nom == "Nouveau Client" &&
            c.Societe == "NouveauSARL"
        )), Times.Once);
    }

    // ── Workflow statut transitions ──────────────────────────────────────────

    [Fact]
    public async Task UpdateStatutAsync_NOUVEAU_to_EN_COURS_Succeeds()
    {
        var lead = new Lead { Id = 1, Statut = "NOUVEAU", DateTraitement = null };
        _leadRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(lead);

        var result = await _sut.UpdateStatutAsync(1, new UpdateLeadStatutDto { Statut = "EN_COURS" });

        result.Statut.Should().Be("EN_COURS");
        result.DateTraitement.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateStatutAsync_NOUVEAU_to_ACCEPTE_ThrowsException()
    {
        var lead = new Lead { Id = 2, Statut = "NOUVEAU" };
        _leadRepoMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(lead);

        Func<Task> act = async () => await _sut.UpdateStatutAsync(2, new UpdateLeadStatutDto { Statut = "ACCEPTE" });

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Transition de statut invalide*");
    }

    [Fact]
    public async Task UpdateStatutAsync_LeadNotFound_ThrowsNotFoundException()
    {
        _leadRepoMock.Setup(x => x.GetByIdAsync(404)).ReturnsAsync((Lead?)null);

        Func<Task> act = async () => await _sut.UpdateStatutAsync(404, new UpdateLeadStatutDto { Statut = "EN_COURS" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateStatutAsync_EN_COURS_to_ACCEPTE_Succeeds()
    {
        var lead = new Lead { Id = 3, Statut = "EN_COURS", DateTraitement = DateTime.UtcNow };
        _leadRepoMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(lead);

        var result = await _sut.UpdateStatutAsync(3, new UpdateLeadStatutDto { Statut = "ACCEPTE" });

        result.Statut.Should().Be("ACCEPTE");
    }

    [Fact]
    public async Task UpdateStatutAsync_ACCEPTE_to_anything_ThrowsException()
    {
        var lead = new Lead { Id = 4, Statut = "ACCEPTE" };
        _leadRepoMock.Setup(x => x.GetByIdAsync(4)).ReturnsAsync(lead);

        Func<Task> act = async () => await _sut.UpdateStatutAsync(4, new UpdateLeadStatutDto { Statut = "REFUSE" });

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Transition de statut invalide*");
    }
}
