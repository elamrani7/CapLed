using FluentAssertions;
using Moq;
using StockManager.Core.Application.DTOs.Stock;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Application.Services;
using StockManager.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapLed.Tests.Unit;

public class StockServiceTests
{
    private readonly Mock<IStockServiceV2> _v2Mock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IEquipmentRepository> _equipmentMock;
    private readonly Mock<ICategoryRepository> _categoryMock;
    private readonly Mock<ILotRepository> _lotMock;
    private readonly Mock<INumeroSerieRepository> _serialMock;
    private readonly Mock<IStockQuantiteRepository> _stockQuantMock;
    private readonly Mock<IAlerteStockRepository> _alerteMock;
    private readonly Mock<IStockMovementRepository> _movementMock;
    
    private readonly StockServiceV3 _sut;

    public StockServiceTests()
    {
        _v2Mock = new Mock<IStockServiceV2>();
        _uowMock = new Mock<IUnitOfWork>();
        _equipmentMock = new Mock<IEquipmentRepository>();
        _categoryMock = new Mock<ICategoryRepository>();
        _lotMock = new Mock<ILotRepository>();
        _serialMock = new Mock<INumeroSerieRepository>();
        _stockQuantMock = new Mock<IStockQuantiteRepository>();
        _alerteMock = new Mock<IAlerteStockRepository>();
        _movementMock = new Mock<IStockMovementRepository>();

        _sut = new StockServiceV3(
            _v2Mock.Object,
            _uowMock.Object,
            _equipmentMock.Object,
            _categoryMock.Object,
            _lotMock.Object,
            _serialMock.Object,
            _stockQuantMock.Object,
            _alerteMock.Object,
            _movementMock.Object
        );
    }

    [Fact]
    public async Task CreateMouvementAsync_LotWithoutNumeroLot_ThrowsException()
    {
        // Arrange
        var equipment = new Equipment { Id = 1, Name = "Produit LOT", CategoryId = 5 };
        var category = new Category { Id = 5, TypeGestionStock = "LOT" };
        var dto = new CreateMouvementDto
        {
            ArticleId = 1,
            TypeMouvement = "ENTREE",
            DepotDestinationId = 10,
            Quantite = 50,
            NumeroLot = null // Missing Lot Number
        };

        _equipmentMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(equipment);
        _categoryMock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(category);

        // Act
        Func<Task> act = async () => await _sut.CreateMouvementAsync(dto, 1);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();
        exception.WithMessage("NumeroLot est obligatoire pour un article géré par LOT.");
    }

    [Fact]
    public async Task CreateMouvementAsync_SerialiseWithDuplicates_ThrowsException()
    {
        // Arrange
        var equipment = new Equipment { Id = 2, Name = "Produit SERIAL", CategoryId = 8 };
        var category = new Category { Id = 8, TypeGestionStock = "SERIALISE" };
        var dto = new CreateMouvementDto
        {
            ArticleId = 2,
            TypeMouvement = "ENTREE",
            DepotDestinationId = 10,
            Quantite = 2,
            NumeroSeries = new List<string> { "SN-001", "SN-001" } // Duplicate
        };

        _equipmentMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(equipment);
        _categoryMock.Setup(x => x.GetByIdAsync(8)).ReturnsAsync(category);

        // Act
        Func<Task> act = async () => await _sut.CreateMouvementAsync(dto, 1);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();
        exception.WithMessage("Les numéros de série fournis contiennent des doublons.");
    }
}
