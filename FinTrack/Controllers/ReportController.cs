using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("MonthlyReport")]
    public async Task<IActionResult> GetMonthlyReport([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 1 || month < 1 || month > 12)
            return StatusCode((int)HttpStatusCode.BadRequest, "Invalid year/month.");

        try
        {
            var report = await _reportService.GetMonthlyReportAsync(year, month);
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, $"Erro interno do servidor: {ex.Message}");
        }
    }
}