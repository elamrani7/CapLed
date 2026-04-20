using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Enums;

using Microsoft.AspNetCore.Authorization;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly IStockMovementRepository _movementRepository;
    private readonly IMapper _mapper;

    public StockController(IStockService stockService, IStockMovementRepository movementRepository, IMapper mapper)
    {
        _stockService = stockService;
        _movementRepository = movementRepository;
        _mapper = mapper;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    /// <summary>
    /// Record a stock entry (restock).
    /// Used by WPF Back-office.
    /// </summary>
    [HttpPost("entry")]
    public async Task<ActionResult<StockMovementReadDto>> RecordEntry(StockMovementCreateDto request)
    {
        var userId = GetCurrentUserId();
        var movement = await _stockService.RecordEntryAsync(
            request.EquipmentId, 
            request.Quantity, 
            userId, 
            remarks: request.Comment);

        // Re-fetch to include relations for the DTO
        var created = await _movementRepository.GetByIdAsync(movement.Id);
        return Ok(_mapper.Map<StockMovementReadDto>(created));
    }

    /// <summary>
    /// Record a stock exit (dispatched).
    /// Used by WPF Back-office.
    /// </summary>
    [HttpPost("exit")]
    public async Task<ActionResult<StockMovementReadDto>> RecordExit(StockMovementCreateDto request)
    {
        var userId = GetCurrentUserId();
        var movement = await _stockService.RecordExitAsync(
            request.EquipmentId,
            request.Quantity,
            userId,
            remarks: request.Comment);

        var created = await _movementRepository.GetByIdAsync(movement.Id);
        return Ok(_mapper.Map<StockMovementReadDto>(created));
    }

    /// <summary>
    /// Get all equipments currently under their minimum threshold.
    /// Used by WPF Back-office Dashboard.
    /// </summary>
    [HttpGet("alerts/low-stock")]
    public async Task<ActionResult<IEnumerable<AlertReadDto>>> GetLowStockAlerts()
    {
        var alerts = await _stockService.GetLowStockAlertsAsync();
        return Ok(_mapper.Map<IEnumerable<AlertReadDto>>(alerts));
    }

    /// <summary>
    /// Get current stock level for a specific equipment.
    /// Used by WPF Back-office.
    /// </summary>
    [HttpGet("level/{equipmentId}")]
    public async Task<ActionResult<int>> GetStockLevel(int equipmentId)
    {
        var level = await _stockService.GetStockLevelAsync(equipmentId);
        return Ok(level);
    }

    /// <summary>
    /// Get history of movements for a specific equipment.
    /// Used by WPF Back-office.
    /// </summary>
    [HttpGet("history/{equipmentId}")]
    public async Task<ActionResult<IEnumerable<StockMovementReadDto>>> GetHistory(int equipmentId)
    {
        var movements = await _movementRepository.GetByEquipmentIdAsync(equipmentId);
        return Ok(_mapper.Map<IEnumerable<StockMovementReadDto>>(movements));
    }

    /// <summary>
    /// Get history of movements with advanced filters and pagination.
    /// Used by WPF Back-office Movements screen.
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<PagedResultDto<StockMovementReadDto>>> GetAllHistory(
        [FromQuery] int? equipmentId,
        [FromQuery] MovementType? type,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var (entities, totalCount) = await _movementRepository.GetAllAsync(
            equipmentId, type, dateFrom, dateTo, page, pageSize);

        var dtos = _mapper.Map<IEnumerable<StockMovementReadDto>>(entities);

        return Ok(new PagedResultDto<StockMovementReadDto>(dtos, totalCount, page, pageSize));
    }

    /// <summary>
    /// Update an existing movement (Quantity or Comment).
    /// Only Admins can modify history.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateMovement(int id, [FromBody] StockMovementCreateDto request)
    {
        await _stockService.UpdateMovementAsync(
            id,
            request.EquipmentId,
            request.Type,
            request.Quantity,
            request.Comment);
        return NoContent();
    }

    /// <summary>
    /// Delete an existing movement.
    /// Only Admins can delete history.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteMovement(int id)
    {
        await _stockService.DeleteMovementAsync(id);
        return NoContent();
    }
}
