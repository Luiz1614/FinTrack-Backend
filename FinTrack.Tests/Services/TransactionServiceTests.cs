using AutoMapper;
using Fintrack.Contracts.DTOs.Transaction;
using Fintrack.Contracts.Pagination;
using FinTrack.Application.Services;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Moq;

namespace FinTrack.Tests.Services;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockMapper = new Mock<IMapper>();
        _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldReturnTransactionDto_WhenTransactionIsCreatedSuccessfully()
    {
        // Arrange
        var transactionCreateDto = new TransactionCreateDto
        {
            Title = "Grocery Shopping",
            Type = TransactionType.Expense,
            Amount = 150.50m,
            CategoryId = 1,
            AccountId = 1
        };

        var transactionEntity = new Transaction
        {
            Id = 1,
            Title = "Grocery Shopping",
            Type = TransactionType.Expense,
            Amount = 150.50m,
            CategoryId = 1,
            AccountId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var transactionDto = new TransactionDto
        {
            Id = 1,
            Title = "Grocery Shopping",
            Type = TransactionType.Expense,
            Amount = 150.50m,
            CategoryId = 1,
            CategoryTitle = "Food",
            AccountId = 1,
            AccountName = "Main Account",
            CreatedAt = transactionEntity.CreatedAt
        };

        _mockMapper.Setup(m => m.Map<Transaction>(transactionCreateDto)).Returns(transactionEntity);
        _mockTransactionRepository.Setup(r => r.AddTransactionAsync(transactionEntity)).ReturnsAsync(transactionEntity);
        _mockMapper.Setup(m => m.Map<TransactionDto>(transactionEntity)).Returns(transactionDto);

        // Act
        var result = await _transactionService.AddTransactionAsync(transactionCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionDto.Id, result.Id);
        Assert.Equal(transactionDto.Title, result.Title);
        Assert.Equal(transactionDto.Type, result.Type);
        Assert.Equal(transactionDto.Amount, result.Amount);
        Assert.Equal(transactionDto.CategoryId, result.CategoryId);
        Assert.Equal(transactionDto.AccountId, result.AccountId);

        _mockMapper.Verify(m => m.Map<Transaction>(transactionCreateDto), Times.Once);
        _mockTransactionRepository.Verify(r => r.AddTransactionAsync(transactionEntity), Times.Once);
        _mockMapper.Verify(m => m.Map<TransactionDto>(transactionEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteTransactionAsync_ShouldReturnTrue_WhenTransactionIsDeletedSuccessfully()
    {
        // Arrange
        var transactionId = 1;

        _mockTransactionRepository.Setup(r => r.DeleteTransactionAsync(transactionId)).ReturnsAsync(true);

        // Act
        var result = await _transactionService.DeleteTransactionAsync(transactionId);

        // Assert
        Assert.True(result);
        _mockTransactionRepository.Verify(r => r.DeleteTransactionAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ShouldReturnListOfTransactionDtos_WhenTransactionsExist()
    {
        // Arrange
        var transactionParameters = new TransactionParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var transactionEntities = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Salary",
                Type = TransactionType.Income,
                Amount = 5000m,
                CategoryId = 1,
                AccountId = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = 2,
                Title = "Rent",
                Type = TransactionType.Expense,
                Amount = 1200m,
                CategoryId = 2,
                AccountId = 1,
                CreatedAt = DateTime.UtcNow
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto
            {
                Id = 1,
                Title = "Salary",
                Type = TransactionType.Income,
                Amount = 5000m,
                CategoryId = 1,
                CategoryTitle = "Income",
                AccountId = 1,
                AccountName = "Main Account",
                CreatedAt = transactionEntities[0].CreatedAt
            },
            new TransactionDto
            {
                Id = 2,
                Title = "Rent",
                Type = TransactionType.Expense,
                Amount = 1200m,
                CategoryId = 2,
                CategoryTitle = "Housing",
                AccountId = 1,
                AccountName = "Main Account",
                CreatedAt = transactionEntities[1].CreatedAt
            }
        };

        _mockTransactionRepository.Setup(r => r.GetAllTransactionsAsync(transactionParameters)).ReturnsAsync(transactionEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactionEntities)).Returns(transactionDtos);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(transactionParameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(transactionDtos[0].Title, result.First().Title);
        Assert.Equal(transactionDtos[1].Title, result.Last().Title);

        _mockTransactionRepository.Verify(r => r.GetAllTransactionsAsync(transactionParameters), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TransactionDto>>(transactionEntities), Times.Once);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        var transactionParameters = new TransactionParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var emptyTransactionEntities = new List<Transaction>();
        var emptyTransactionDtos = new List<TransactionDto>();

        _mockTransactionRepository.Setup(r => r.GetAllTransactionsAsync(transactionParameters)).ReturnsAsync(emptyTransactionEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(emptyTransactionEntities)).Returns(emptyTransactionDtos);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync(transactionParameters);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockTransactionRepository.Verify(r => r.GetAllTransactionsAsync(transactionParameters), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TransactionDto>>(emptyTransactionEntities), Times.Once);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnTransactionDto_WhenTransactionExists()
    {
        // Arrange
        var transactionId = 1;
        var transactionEntity = new Transaction
        {
            Id = transactionId,
            Title = "Coffee",
            Type = TransactionType.Expense,
            Amount = 4.50m,
            CategoryId = 1,
            AccountId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var transactionDto = new TransactionDto
        {
            Id = transactionId,
            Title = "Coffee",
            Type = TransactionType.Expense,
            Amount = 4.50m,
            CategoryId = 1,
            CategoryTitle = "Food",
            AccountId = 1,
            AccountName = "Main Account",
            CreatedAt = transactionEntity.CreatedAt
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transactionEntity);
        _mockMapper.Setup(m => m.Map<TransactionDto>(transactionEntity)).Returns(transactionDto);

        // Act
        var result = await _transactionService.GetTransactionByIdAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionDto.Id, result.Id);
        Assert.Equal(transactionDto.Title, result.Title);
        Assert.Equal(transactionDto.Type, result.Type);
        Assert.Equal(transactionDto.Amount, result.Amount);

        _mockTransactionRepository.Verify(r => r.GetTransactionByIdAsync(transactionId), Times.Once);
        _mockMapper.Verify(m => m.Map<TransactionDto>(transactionEntity), Times.Once);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnNull_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = 999;
        Transaction? nullTransaction = null;

        _mockTransactionRepository.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(nullTransaction);
        _mockMapper.Setup(m => m.Map<TransactionDto>(nullTransaction)).Returns((TransactionDto)null!);

        // Act
        var result = await _transactionService.GetTransactionByIdAsync(transactionId);

        // Assert
        Assert.Null(result);

        _mockTransactionRepository.Verify(r => r.GetTransactionByIdAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionByAccountAsync_ShouldReturnListOfTransactionDtos_WhenTransactionsExist()
    {
        // Arrange
        var accountId = 1;
        var transactionEntities = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "Gas",
                Type = TransactionType.Expense,
                Amount = 50m,
                CategoryId = 1,
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = 2,
                Title = "Freelance",
                Type = TransactionType.Income,
                Amount = 500m,
                CategoryId = 2,
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto
            {
                Id = 1,
                Title = "Gas",
                Type = TransactionType.Expense,
                Amount = 50m,
                CategoryId = 1,
                CategoryTitle = "Transport",
                AccountId = accountId,
                AccountName = "Main Account",
                CreatedAt = transactionEntities[0].CreatedAt
            },
            new TransactionDto
            {
                Id = 2,
                Title = "Freelance",
                Type = TransactionType.Income,
                Amount = 500m,
                CategoryId = 2,
                CategoryTitle = "Work",
                AccountId = accountId,
                AccountName = "Main Account",
                CreatedAt = transactionEntities[1].CreatedAt
            }
        };

        _mockTransactionRepository.Setup(r => r.GetByAccountAsync(accountId)).ReturnsAsync(transactionEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactionEntities)).Returns(transactionDtos);

        // Act
        var result = await _transactionService.GetTransactionByAccountAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal(accountId, t.AccountId));

        _mockTransactionRepository.Verify(r => r.GetByAccountAsync(accountId), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TransactionDto>>(transactionEntities), Times.Once);
    }

    [Fact]
    public async Task UpdateTransactionAsync_ShouldReturnUpdatedTransactionDto_WhenTransactionIsUpdatedSuccessfully()
    {
        // Arrange
        var transactionUpdateDto = new TransactionUpdateDto
        {
            Id = 1,
            Title = "Updated Transaction",
            Type = TransactionType.Expense,
            Amount = 200m,
            CategoryId = 2,
            AccountId = 1
        };

        var transactionEntity = new Transaction
        {
            Id = 1,
            Title = "Updated Transaction",
            Type = TransactionType.Expense,
            Amount = 200m,
            CategoryId = 2,
            AccountId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var updatedTransactionDto = new TransactionDto
        {
            Id = 1,
            Title = "Updated Transaction",
            Type = TransactionType.Expense,
            Amount = 200m,
            CategoryId = 2,
            CategoryTitle = "Shopping",
            AccountId = 1,
            AccountName = "Main Account",
            CreatedAt = transactionEntity.CreatedAt
        };

        _mockMapper.Setup(m => m.Map<Transaction>(transactionUpdateDto)).Returns(transactionEntity);
        _mockTransactionRepository.Setup(r => r.UpdateTransactionAsync(transactionEntity)).ReturnsAsync(transactionEntity);
        _mockMapper.Setup(m => m.Map<TransactionDto>(transactionEntity)).Returns(updatedTransactionDto);

        // Act
        var result = await _transactionService.UpdateTransactionAsync(transactionUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedTransactionDto.Id, result.Id);
        Assert.Equal(updatedTransactionDto.Title, result.Title);
        Assert.Equal(updatedTransactionDto.Amount, result.Amount);
        Assert.Equal(updatedTransactionDto.Type, result.Type);

        _mockMapper.Verify(m => m.Map<Transaction>(transactionUpdateDto), Times.Once);
        _mockTransactionRepository.Verify(r => r.UpdateTransactionAsync(transactionEntity), Times.Once);
        _mockMapper.Verify(m => m.Map<TransactionDto>(transactionEntity), Times.Once);
    }

    [Fact]
    public async Task GetTrasactionsByMonthAsync_ShouldReturnListOfTransactionDtos_WhenTransactionsExistForGivenMonth()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 11;

        var transactionEntities = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Title = "November Expense",
                Type = TransactionType.Expense,
                Amount = 100m,
                CategoryId = 1,
                AccountId = 1,
                CreatedAt = new DateTime(2024, 11, 15)
            },
            new Transaction
            {
                Id = 2,
                Title = "November Income",
                Type = TransactionType.Income,
                Amount = 3000m,
                CategoryId = 2,
                AccountId = 1,
                CreatedAt = new DateTime(2024, 11, 1)
            }
        };

        var transactionDtos = new List<TransactionDto>
        {
            new TransactionDto
            {
                Id = 1,
                Title = "November Expense",
                Type = TransactionType.Expense,
                Amount = 100m,
                CategoryId = 1,
                CategoryTitle = "Food",
                AccountId = 1,
                AccountName = "Main Account",
                CreatedAt = transactionEntities[0].CreatedAt
            },
            new TransactionDto
            {
                Id = 2,
                Title = "November Income",
                Type = TransactionType.Income,
                Amount = 3000m,
                CategoryId = 2,
                CategoryTitle = "Salary",
                AccountId = 1,
                AccountName = "Main Account",
                CreatedAt = transactionEntities[1].CreatedAt
            }
        };

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month)).ReturnsAsync(transactionEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(transactionEntities)).Returns(transactionDtos);

        // Act
        var result = await _transactionService.GetTrasactionsByMonthAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal(month, t.CreatedAt.Month));
        Assert.All(result, t => Assert.Equal(year, t.CreatedAt.Year));

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TransactionDto>>(transactionEntities), Times.Once);
    }

    [Fact]
    public async Task GetTrasactionsByMonthAsync_ShouldReturnEmptyList_WhenNoTransactionsExistForGivenMonth()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 12;

        var emptyTransactionEntities = new List<Transaction>();
        var emptyTransactionDtos = new List<TransactionDto>();

        _mockTransactionRepository.Setup(r => r.GetTransactionByMonthAsync(userId, year, month)).ReturnsAsync(emptyTransactionEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<TransactionDto>>(emptyTransactionEntities)).Returns(emptyTransactionDtos);

        // Act
        var result = await _transactionService.GetTrasactionsByMonthAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockTransactionRepository.Verify(r => r.GetTransactionByMonthAsync(userId, year, month), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TransactionDto>>(emptyTransactionEntities), Times.Once);
    }

    [Fact]
    public async Task AddTransactionAsync_ShouldHandleIncomeTransaction()
    {
        // Arrange
        var transactionCreateDto = new TransactionCreateDto
        {
            Title = "Salary Payment",
            Type = TransactionType.Income,
            Amount = 5000m,
            CategoryId = 1,
            AccountId = 1
        };

        var transactionEntity = new Transaction
        {
            Id = 1,
            Title = "Salary Payment",
            Type = TransactionType.Income,
            Amount = 5000m,
            CategoryId = 1,
            AccountId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var transactionDto = new TransactionDto
        {
            Id = 1,
            Title = "Salary Payment",
            Type = TransactionType.Income,
            Amount = 5000m,
            CategoryId = 1,
            CategoryTitle = "Salary",
            AccountId = 1,
            AccountName = "Main Account",
            CreatedAt = transactionEntity.CreatedAt
        };

        _mockMapper.Setup(m => m.Map<Transaction>(transactionCreateDto)).Returns(transactionEntity);
        _mockTransactionRepository.Setup(r => r.AddTransactionAsync(transactionEntity)).ReturnsAsync(transactionEntity);
        _mockMapper.Setup(m => m.Map<TransactionDto>(transactionEntity)).Returns(transactionDto);

        // Act
        var result = await _transactionService.AddTransactionAsync(transactionCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TransactionType.Income, result.Type);
        Assert.True(result.Amount > 0);

        _mockMapper.Verify(m => m.Map<Transaction>(transactionCreateDto), Times.Once);
        _mockTransactionRepository.Verify(r => r.AddTransactionAsync(transactionEntity), Times.Once);
    }
}