using AutoMapper;
using Fintrack.Contracts.DTOs.MonthlyReport;
using Fintrack.Contracts.DTOs.Transaction;
using FinTrack.Application.Services.Interfaces;
using FinTrack.Domain.Enums;
using FinTrack.Infraestructure.Repositories.Interfaces;

namespace FinTrack.Application.Services;

public class ReportService : IReportService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public ReportService(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<MonthlyReportDto> GetMonthlyReportAsync(int idUser, int year, int month)
    {
        var transactions = await _transactionRepository.GetTransactionByMonthAsync(idUser, year, month);

        var totalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        var expenseGroups = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => new { t.CategoryId, Title = t.Category?.Title ?? string.Empty })
            .Select(g =>
            {
                var total = g.Sum(x => x.Amount);
                return new MonthlyReportCategoryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryTitle = g.Key.Title,
                    TotalExpense = total,
                    Percentage = totalExpense == 0 ? 0 : Math.Round((total / totalExpense) * 100, 2)
                };
            })
            .OrderByDescending(c => c.TotalExpense)
            .ToList();

        var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);

        return new MonthlyReportDto
        {
            Year = year,
            Month = month,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = totalIncome - totalExpense,
            Categories = expenseGroups,
            Transactions = transactionDtos
        };
    }
}