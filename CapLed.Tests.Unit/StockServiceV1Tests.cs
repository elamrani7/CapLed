using FluentAssertions;
using Moq;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Services;
using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Enums;
using StockManager.Core.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapLed.Tests.Unit;

/// <summary>
/// Tests unitaires pour StockService (v1) — mouvements QUANTITE simples.
/// Couvre : RecordEntry, RecordExit (stock insuffisant), DeleteMovement, GetLowStockAlerts.
/// </summary>
public class StockServiceV1Tests
{
    private readonly Mock<IEquipmentRepository> _equipmentMock;
    private readonly Mock<IStockMovementRepository> _movementMock;
    private readonly StockService _sut;

    public StockServiceV1Tests()
    {
        _equipmentMock = new Mock<IEquipmentRepository>();
        _movementMock = new Mock<IStockMovementRepository>();
        _sut = new StockService(_equipmentMock.Object, _movementMock.Object);
    }

    // ── RecordEntryAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task RecordEntryAsync_ValidArticle_IncrementsQuantityAndReturnsMovement()
    {
        var equipment = new Equipment { Id = 1, Name = "Cable RJ45", Quantity = 10 };
        _equipmentMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(equipment);

        var result = await _sut.RecordEntryAsync(1, 5, userId: 1, remarks: "Réception fournisseur");

        result.Should().NotBeNull();
        result.Type.Should().Be(MovementType.ENTRY);
        result.Quantity.Should().Be(5);
        equipment.Quantity.Should().Be(15); // 10 + 5
        _movementMock.Verify(x => x.AddAsync(It.IsAny<StockMovement>()), Times.Once);
        _equipmentMock.Verify(x => x.UpdateAsync(equipment), Times.Once);
    }

    [Fact]
    public async Task RecordEntryAsync_ArticleNotFound_ThrowsNotFoundException()
    {
        _equipmentMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Equipment?)null);

        Func<Task> act = async () => await _sut.RecordEntryAsync(999, 5, 1);

        var ex = await act.Should().ThrowAsync<NotFoundException>();
        ex.Which.Code.Should().Be("ARTICLE_NOT_FOUND");
    }

    // ── RecordExitAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task RecordExitAsync_SufficientStock_DecrementsQuantity()
    {
        var equipment = new Equipment { Id = 2, Name = "Switch", Quantity = 20 };
        _equipmentMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(equipment);

        var result = await _sut.RecordExitAsync(2, 8, userId: 1);

        result.Type.Should().Be(MovementType.EXIT);
        equipment.Quantity.Should().Be(12); // 20 - 8
    }

    [Fact]
    public async Task RecordExitAsync_InsufficientStock_ThrowsDomainException()
    {
        var equipment = new Equipment { Id = 3, Name = "Routeur", Quantity = 2 };
        _equipmentMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(equipment);

        Func<Task> act = async () => await _sut.RecordExitAsync(3, 10, 1);

        var ex = await act.Should().ThrowAsync<DomainException>();
        ex.Which.Code.Should().Be("STOCK_INSUFFICIENT");
    }

    [Fact]
    public async Task RecordExitAsync_ArticleNotFound_ThrowsNotFoundException()
    {
        _equipmentMock.Setup(x => x.GetByIdAsync(404)).ReturnsAsync((Equipment?)null);

        Func<Task> act = async () => await _sut.RecordExitAsync(404, 1, 1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── DeleteMovementAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task DeleteMovementAsync_EntryMovement_ReversesStockIncrease()
    {
        var movement = new StockMovement { Id = 10, EquipmentId = 1, Type = MovementType.ENTRY, Quantity = 5 };
        var equipment = new Equipment { Id = 1, Name = "Cable", Quantity = 15 };

        _movementMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(movement);
        _equipmentMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(equipment);

        await _sut.DeleteMovementAsync(10);

        equipment.Quantity.Should().Be(10); // 15 - 5 reversed
        _movementMock.Verify(x => x.DeleteAsync(10), Times.Once);
    }

    [Fact]
    public async Task DeleteMovementAsync_ExitMovement_ReversesStockDecrease()
    {
        var movement = new StockMovement { Id = 11, EquipmentId = 2, Type = MovementType.EXIT, Quantity = 3 };
        var equipment = new Equipment { Id = 2, Name = "Switch", Quantity = 7 };

        _movementMock.Setup(x => x.GetByIdAsync(11)).ReturnsAsync(movement);
        _equipmentMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(equipment);

        await _sut.DeleteMovementAsync(11);

        equipment.Quantity.Should().Be(10); // 7 + 3 reversed
    }

    [Fact]
    public async Task DeleteMovementAsync_NotFound_ThrowsNotFoundException()
    {
        _movementMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((StockMovement?)null);

        Func<Task> act = async () => await _sut.DeleteMovementAsync(999);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── GetLowStockAlertsAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetLowStockAlertsAsync_ReturnsOnlyItemsBelowThreshold()
    {
        var items = new List<Equipment>
        {
            new() { Id = 1, Name = "Abondant", Quantity = 100 },
            new() { Id = 2, Name = "Bas", Quantity = 3 },
            new() { Id = 3, Name = "Vide", Quantity = 0 },
            new() { Id = 4, Name = "Seuil", Quantity = 5 }
        };
        _equipmentMock.Setup(x => x.GetAllAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<EquipmentCondition?>(), It.IsAny<bool?>(), It.IsAny<string?>(), 1, 1000)).ReturnsAsync((items, items.Count));

        var alerts = await _sut.GetLowStockAlertsAsync();

        alerts.Should().HaveCount(3); // Bas (3), Vide (0), Seuil (5)
        alerts.Should().NotContain(e => e.Name == "Abondant");
    }

    // ── UpdateMovementAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task UpdateMovementAsync_SameEquipment_RecalculatesStockCorrectly()
    {
        var oldMovement = new StockMovement { Id = 1, EquipmentId = 1, Type = MovementType.ENTRY, Quantity = 10 };
        var equipment = new Equipment { Id = 1, Name = "Cable", Quantity = 30 };

        _movementMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(oldMovement);
        _equipmentMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(equipment);

        // Change from ENTRY 10 to ENTRY 15 on same equipment
        await _sut.UpdateMovementAsync(1, 1, MovementType.ENTRY, 15, "Updated");

        // Revert: 30 - 10 = 20. Apply: 20 + 15 = 35
        equipment.Quantity.Should().Be(35);
    }
}
