using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Application.Interfaces.Services;
using StockManager.Core.Domain.Enums;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
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

    /// <summary>
    /// Record a stock entry (restock).
    /// Used by WPF Back-office.
    /// </summary>
    [HttpPost("entry")]
    public async Task<ActionResult<StockMovementReadDto>> RecordEntry(StockMovementCreateDto request)
    {
        // For now, userId is hardcoded to 1 (Admin) until Auth is implemented
        var movement = await _stockService.RecordEntryAsync(
            request.EquipmentId, 
            request.Quantity, 
            userId: 1, 
            remarks: request.Comment);

        return Ok(_mapper.Map<StockMovementReadDto>(movement));
    }

    /// <summary>
    /// Record a stock exit (dispatched).
    /// Used by WPF Back-office.
    /// </summary>
    [HttpPost("exit")]
    public async Task<ActionResult<StockMovementReadDto>> RecordExit(StockMovementCreateDto request)
    {
        try
        {
            var movement = await _stockService.RecordExitAsync(
                request.EquipmentId, 
                request.Quantity, 
                userId: 1, 
                remarks: request.Comment);

            return Ok(_mapper.Map<StockMovementReadDto>(movement));
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
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
}
