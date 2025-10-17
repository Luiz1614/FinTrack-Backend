using Fintrack.Contracts.DTOs.MonthlyReport;

namespace FinTrack.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<MonthlyReportDto> GetMonthlyReportAsync(int idUser, int year, int month);
    }
}