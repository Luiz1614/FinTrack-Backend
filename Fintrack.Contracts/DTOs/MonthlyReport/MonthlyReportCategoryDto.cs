namespace Fintrack.Contracts.DTOs.MonthlyReport;

public class MonthlyReportCategoryDto
{
    public int CategoryId { get; set; }
    public string? CategoryTitle { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Percentage { get; set; }
}
