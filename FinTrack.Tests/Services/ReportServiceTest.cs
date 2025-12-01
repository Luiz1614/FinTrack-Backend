using AutoMapper;
using Fintrack.Contracts.DTOs.MonthlyReport;
using Fintrack.Contracts.DTOs.Transaction;
using FinTrack.Application.Services;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Moq;

namespace FinTrack.Tests.Services;

public class ReportServiceTest
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ReportService _reportService;

    public ReportServiceTest()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockMapper = new Mock<IMapper>();
        _reportService = new ReportService(_mockTransactionRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldReturnCompleteReport_WhenTransactionsExist()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 11;

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Salary",
                Type = TransactionType.Income,
                Amount = 5000m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Income" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 11, 1)
            },
            new Transaction
            {
                Id = 2,
                Title = "Groceries",
                Type = TransactionType.Expense,
                Amount = 300m,
                CategoryId = 2,
                Category = new Category { Id = 2, Title = "Food" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 11, 5)
            },
            new Transaction
            {
                Id = 3,
                Title = "Restaurant",
                Type = TransactionType.Expense,
                Amount = 150m,
                CategoryId = 2,
                Category = new Category { Id = 2, Title = "Food" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 11, 10)
            },
            new Transaction
            {
                Id = 4,
                Title = "Rent",
                Type = TransactionType.Expense,
                Amount = 1200m,
                CategoryId = 3,
                Category = new Category { Id = 3, Title = "Housing" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 11, 1)
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto { Id = 1, Title = "Salary", Type = TransactionType.Income, Amount = 5000m },
            new TransactionDto { Id = 2, Title = "Groceries", Type = TransactionType.Expense, Amount = 300m },
            new TransactionDto { Id = 3, Title = "Restaurant", Type = TransactionType.Expense, Amount = 150m },
            new TransactionDto { Id = 4, Title = "Rent", Type = TransactionType.Expense, Amount = 1200m }
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(transactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(year, result.Year);
        Assert.Equal(month, result.Month);
        Assert.Equal(5000m, result.TotalIncome);
        Assert.Equal(1650m, result.TotalExpense); // 300 + 150 + 1200
        Assert.Equal(3350m, result.Balance); // 5000 - 1650
        Assert.Equal(4, result.Transactions.Count());
        Assert.Equal(2, result.Categories.Count()); // Food and Housing

        // Verify categories are ordered by TotalExpense descending
        var categoriesList = result.Categories.ToList();
        Assert.Equal("Housing", categoriesList[0].CategoryTitle);
        Assert.Equal(1200m, categoriesList[0].TotalExpense);
        Assert.Equal(72.73m, categoriesList[0].Percentage); // (1200 / 1650) * 100 = 72.73%

        Assert.Equal("Food", categoriesList[1].CategoryTitle);
        Assert.Equal(450m, categoriesList[1].TotalExpense); // 300 + 150
        Assert.Equal(27.27m, categoriesList[1].Percentage); // (450 / 1650) * 100 = 27.27%

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TransactionDto>>(transactions), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldReturnEmptyReport_WhenNoTransactionsExist()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 12;

        var emptyTransactions = new List<Transaction>();
        var emptyTransactionDtos = new List<TransactionDto>();

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(emptyTransactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(emptyTransactions))
            .Returns(emptyTransactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(year, result.Year);
        Assert.Equal(month, result.Month);
        Assert.Equal(0m, result.TotalIncome);
        Assert.Equal(0m, result.TotalExpense);
        Assert.Equal(0m, result.Balance);
        Assert.Empty(result.Transactions);
        Assert.Empty(result.Categories);

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TransactionDto>>(emptyTransactions), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldReturnOnlyIncomeReport_WhenOnlyIncomeTransactionsExist()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 10;

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Salary",
                Type = TransactionType.Income,
                Amount = 5000m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Income" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 10, 1)
            },
            new Transaction
            {
                Id = 2,
                Title = "Freelance",
                Type = TransactionType.Income,
                Amount = 1500m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Income" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 10, 15)
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto { Id = 1, Title = "Salary", Type = TransactionType.Income, Amount = 5000m },
            new TransactionDto { Id = 2, Title = "Freelance", Type = TransactionType.Income, Amount = 1500m }
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(transactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6500m, result.TotalIncome); // 5000 + 1500
        Assert.Equal(0m, result.TotalExpense);
        Assert.Equal(6500m, result.Balance);
        Assert.Equal(2, result.Transactions.Count());
        Assert.Empty(result.Categories); // No expense categories

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldReturnOnlyExpenseReport_WhenOnlyExpenseTransactionsExist()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 9;

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Shopping",
                Type = TransactionType.Expense,
                Amount = 800m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Shopping" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 9, 5)
            },
            new Transaction
            {
                Id = 2,
                Title = "Bills",
                Type = TransactionType.Expense,
                Amount = 200m,
                CategoryId = 2,
                Category = new Category { Id = 2, Title = "Utilities" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 9, 10)
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto { Id = 1, Title = "Shopping", Type = TransactionType.Expense, Amount = 800m },
            new TransactionDto { Id = 2, Title = "Bills", Type = TransactionType.Expense, Amount = 200m }
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(transactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.TotalIncome);
        Assert.Equal(1000m, result.TotalExpense); // 800 + 200
        Assert.Equal(-1000m, result.Balance); // Negative balance
        Assert.Equal(2, result.Transactions.Count());
        Assert.Equal(2, result.Categories.Count());

        var categoriesList = result.Categories.ToList();
        Assert.Equal("Shopping", categoriesList[0].CategoryTitle);
        Assert.Equal(80m, categoriesList[0].Percentage);

        Assert.Equal("Utilities", categoriesList[1].CategoryTitle);
        Assert.Equal(20m, categoriesList[1].Percentage);

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldGroupExpensesByCategory_Correctly()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 8;

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Lunch",
                Type = TransactionType.Expense,
                Amount = 50m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Food" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 8, 1)
            },
            new Transaction
            {
                Id = 2,
                Title = "Dinner",
                Type = TransactionType.Expense,
                Amount = 100m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Food" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 8, 5)
            },
            new Transaction
            {
                Id = 3,
                Title = "Breakfast",
                Type = TransactionType.Expense,
                Amount = 30m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Food" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 8, 10)
            },
            new Transaction
            {
                Id = 4,
                Title = "Gas",
                Type = TransactionType.Expense,
                Amount = 120m,
                CategoryId = 2,
                Category = new Category { Id = 2, Title = "Transport" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 8, 15)
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto { Id = 1, Title = "Lunch", Type = TransactionType.Expense, Amount = 50m },
            new TransactionDto { Id = 2, Title = "Dinner", Type = TransactionType.Expense, Amount = 100m },
            new TransactionDto { Id = 3, Title = "Breakfast", Type = TransactionType.Expense, Amount = 30m },
            new TransactionDto { Id = 4, Title = "Gas", Type = TransactionType.Expense, Amount = 120m }
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(transactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(300m, result.TotalExpense); // 50 + 100 + 30 + 120
        Assert.Equal(2, result.Categories.Count());

        var categoriesList = result.Categories.ToList();

        // Should be ordered by TotalExpense descending
        Assert.Equal("Food", categoriesList[0].CategoryTitle);
        Assert.Equal(180m, categoriesList[0].TotalExpense); // 50 + 100 + 30
        Assert.Equal(60m, categoriesList[0].Percentage); // (180 / 300) * 100

        Assert.Equal("Transport", categoriesList[1].CategoryTitle);
        Assert.Equal(120m, categoriesList[1].TotalExpense);
        Assert.Equal(40m, categoriesList[1].Percentage); // (120 / 300) * 100

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldHandleNullCategoryTitle()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 7;

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Unknown Expense",
                Type = TransactionType.Expense,
                Amount = 100m,
                CategoryId = 1,
                Category = null, // Null category
                AccountId = 1,
                CreatedAt = new DateTime(2024, 7, 1)
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto { Id = 1, Title = "Unknown Expense", Type = TransactionType.Expense, Amount = 100m }
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(transactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100m, result.TotalExpense);
        Assert.Single(result.Categories);
        Assert.Equal(string.Empty, result.Categories.First().CategoryTitle); // Should handle null as empty string

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldCalculatePercentageAsZero_WhenTotalExpenseIsZero()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 6;

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Income Only",
                Type = TransactionType.Income,
                Amount = 1000m,
                CategoryId = 1,
                Category = new Category { Id = 1, Title = "Salary" },
                AccountId = 1,
                CreatedAt = new DateTime(2024, 6, 1)
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto { Id = 1, Title = "Income Only", Type = TransactionType.Income, Amount = 1000m }
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(transactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000m, result.TotalIncome);
        Assert.Equal(0m, result.TotalExpense);
        Assert.Empty(result.Categories); // No expense categories, so percentage calculation doesn't apply

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyReportAsync_ShouldCalculateBalanceCorrectly_WithMultipleTransactions()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 5;

        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, Type = TransactionType.Income, Amount = 3000m, CategoryId = 1, Category = new Category { Title = "Salary" }, CreatedAt = new DateTime(2024, 5, 1) },
            new Transaction { Id = 2, Type = TransactionType.Income, Amount = 500m, CategoryId = 1, Category = new Category { Title = "Bonus" }, CreatedAt = new DateTime(2024, 5, 5) },
            new Transaction { Id = 3, Type = TransactionType.Expense, Amount = 1000m, CategoryId = 2, Category = new Category { Title = "Rent" }, CreatedAt = new DateTime(2024, 5, 1) },
            new Transaction { Id = 4, Type = TransactionType.Expense, Amount = 200m, CategoryId = 3, Category = new Category { Title = "Food" }, CreatedAt = new DateTime(2024, 5, 10) },
            new Transaction { Id = 5, Type = TransactionType.Expense, Amount = 150m, CategoryId = 4, Category = new Category { Title = "Transport" }, CreatedAt = new DateTime(2024, 5, 15) }
        };

        var transactionDtos = transactions.Select(t => new TransactionDto { Id = t.Id, Type = t.Type, Amount = t.Amount }).ToList();

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month))
            .ReturnsAsync(transactions);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _reportService.GetMonthlyReportAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3500m, result.TotalIncome); // 3000 + 500
        Assert.Equal(1350m, result.TotalExpense); // 1000 + 200 + 150
        Assert.Equal(2150m, result.Balance); // 3500 - 1350

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
    }
}