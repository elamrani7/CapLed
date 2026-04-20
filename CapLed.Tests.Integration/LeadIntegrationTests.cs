using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StockManager.Core.Application.DTOs.Commercial;
using StockManager.Core.Domain.Entities;
using StockManager.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace CapLed.Tests.Integration;

public class LeadIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public LeadIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostCheckoutQuote_CreatesClientAndLead()
    {
        // Arrange
        // We must seed an Equipment so that adding LigneLead doesn't fail on FK validation (even in SQLite)
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StockManagementDbContext>();
            // Add a mock category and article
            var cat = new Category { Label = "Cables", Description = "Desc", TypeGestionStock = "QUANTITE" };
            db.Categories.Add(cat);
            db.SaveChanges(); // to get ID
            
            var article = new Equipment { CategoryId = cat.Id, Name = "Test Cable", Reference = "REF001" };
            db.Equipments.Add(article);
            db.SaveChanges();
        }

        var payload = new CreateLeadDto
        {
            NomClient = "TestLead",
            EmailClient = "lead@test.com",
            SourceAcquisition = "SITE_WEB",
            Lignes = new List<CreateLigneLeadDto>
            {
                new CreateLigneLeadDto { ArticleId = 1, QuantiteDemandee = 5 } // 1 is safely the seeded article id 
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/leads", payload);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, content);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StockManagementDbContext>();
            
            // Validate client creation
            var client = db.Clients.FirstOrDefault(c => c.Email == "lead@test.com");
            client.Should().NotBeNull();
            client!.Nom.Should().Be("TestLead");

            // Validate Lead creation
            var lead = db.Leads.FirstOrDefault(l => l.ClientId == client.Id);
            lead.Should().NotBeNull();
            lead!.SourceAcquisition.Should().Be("SITE_WEB");
            lead.Statut.Should().Be("NOUVEAU");

            // Validate LigneLead creation
            var ligne = db.LignesLead.FirstOrDefault(l => l.LeadId == lead.Id);
            ligne.Should().NotBeNull();
            ligne!.ArticleId.Should().Be(1);
            ligne.QuantiteDemandee.Should().Be(5);
        }
    }
}
