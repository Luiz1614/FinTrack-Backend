using FinTrack.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinTrack.Controllers;

[Authorize(Policy = "UserOnly")]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Gera um relatório mensal para uma conta específica.
    /// </summary>
    /// <param name="idAccount">O ID da conta para a qual o relatório será gerado.</param>
    /// <param name="year">O ano do relatório.</param>
    /// <param name="month">O mês do relatório.</param>
    /// <returns>O relatório mensal gerado.</returns>
    /// <response code="200">Retorna o relatório mensal gerado.</response>
    /// <response code="400">Se o ano ou mês fornecido for inválido.</response>
    [HttpGet("MonthlyReport")]
    public async Task<IActionResult> GetMonthlyReport([FromQuery] int idAccount, [FromQuery] int year, [FromQuery] int month)
    {
        if (year < 1 || month < 1 || month > 12)
            return StatusCode((int)HttpStatusCode.BadRequest, "Invalid year/month.");

        var report = await _reportService.GetMonthlyReportAsync(idAccount, year, month);
        return Ok(report);
    }
}