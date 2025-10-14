namespace FinTrack.Domain.Entities;

public class MonthlyReportCategory
{
    public int CategoryId { get; set; }
    public string? CategoryTitle { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Percentage { get; set; }
}
