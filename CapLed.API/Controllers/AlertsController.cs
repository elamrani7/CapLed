using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockManager.Core.Application.DTOs;
using StockManager.Core.Application.Interfaces.Services;

namespace StockManager.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly IMapper _mapper;

    public AlertsController(IStockService stockService, IMapper mapper)
    {
        _stockService = stockService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all equipments currently under their minimum threshold.
    /// Used by WPF Back-office.
    /// </summary>
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<AlertReadDto>>> GetLowStockAlerts()
    {
        var alerts = await _stockService.GetLowStockAlertsAsync();
        return Ok(_mapper.Map<IEnumerable<AlertReadDto>>(alerts));
    }
}
