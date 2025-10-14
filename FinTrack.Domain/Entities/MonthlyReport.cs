namespace FinTrack.Domain.Entities;

public class MonthlyReport
{
    public int Year { get; set; }
    public int Month { get; set; }

    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }

    public decimal Balance => TotalIncome - TotalExpense;

    public List<MonthlyReportCategory> Categories { get; set; } = new List<MonthlyReportCategory>();
    public List<Transaction> transactions { get; set; } = new List<Transaction>();
}
