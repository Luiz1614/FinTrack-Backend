using Fintrack.Contracts.DTOs.Transaction;

namespace Fintrack.Contracts.DTOs.MonthlyReport;

public class MonthlyReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }

    public IEnumerable<MonthlyReportCategoryDto> Categories { get; set; } = Enumerable.Empty<MonthlyReportCategoryDto>();
    public IEnumerable<TransactionDto> Transactions { get; set; } = Enumerable.Empty<TransactionDto>();
}
