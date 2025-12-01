using AutoMapper;
using Fintrack.Contracts.DTOs.Account;
using FinTrack.Application.DTOs.Accounts;
using FinTrack.Application.Services;
using FinTrack.Domain.Entities;
using FinTrack.Infraestructure.Repositories.Interfaces;
using Moq;

namespace FinTrack.Tests.Services;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockMapper = new Mock<IMapper>();
        _accountService = new AccountService(_mockAccountRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddAccountAsync_ShouldReturnAccountDto_WhenAccountIsCreatedSuccessfully()
    {
        // Arrange
        var accountCreateDto = new AccountCreateDto
        {
            Name = "Savings Account",
            InitalBalance = 1000m
        };

        var accountEntity = new Account
        {
            Id = 1,
            Name = "Savings Account",
            InitialBalance = 1000m,
            CurrentBalance = 1000m
        };

        var accountDto = new AccountDto
        {
            Id = 1,
            Name = "Savings Account",
            InitialBalance = 1000m,
            CurrentBalance = 1000m
        };

        _mockMapper.Setup(m => m.Map<Account>(accountCreateDto)).Returns(accountEntity);
        _mockAccountRepository.Setup(r => r.AddAccountAsync(accountEntity)).ReturnsAsync(accountEntity);
        _mockMapper.Setup(m => m.Map<AccountDto>(accountEntity)).Returns(accountDto);

        // Act
        var result = await _accountService.AddAccountAsync(accountCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountDto.Id, result.Id);
        Assert.Equal(accountDto.Name, result.Name);
        Assert.Equal(accountDto.InitialBalance, result.InitialBalance);
        Assert.Equal(accountDto.CurrentBalance, result.CurrentBalance);

        _mockMapper.Verify(m => m.Map<Account>(accountCreateDto), Times.Once);
        _mockAccountRepository.Verify(r => r.AddAccountAsync(accountEntity), Times.Once);
        _mockMapper.Verify(m => m.Map<AccountDto>(accountEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldReturnTrue_WhenAccountIsDeletedSuccessfully()
    {
        // Arrange
        var accountId = 1;

        _mockAccountRepository.Setup(r => r.DeleteAccountAsync(accountId)).ReturnsAsync(true);
        // Act
        var result = await _accountService.DeleteAccountAsync(accountId);

        // Assert
        Assert.True(result);
        _mockAccountRepository.Verify(r => r.DeleteAccountAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task GetAllAccountsAsync_ShouldReturnListOfAccountDtos_WhenAccountsExist()
    {
        // Arrange
        var accountEntities = new List<Account>
        {
            new Account { Id = 1, Name = "Checking Account", InitialBalance = 500m, CurrentBalance = 500m },
            new Account { Id = 2, Name = "Savings Account", InitialBalance = 1000m, CurrentBalance = 1200m }
        };

        var accountDtos = new List<AccountDto>
        {
            new AccountDto { Id = 1, Name = "Checking Account", InitialBalance = 500m, CurrentBalance = 500m },
            new AccountDto { Id = 2, Name = "Savings Account", InitialBalance = 1000m, CurrentBalance = 1200m }
        };

        _mockAccountRepository.Setup(r => r.GetAllAccountsWithTransactionsAsync()).ReturnsAsync(accountEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<AccountDto>>(accountEntities)).Returns(accountDtos);

        // Act
        var result = await _accountService.GetAllAccountsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(accountDtos[0].Name, result.First().Name);
        Assert.Equal(accountDtos[1].Name, result.Last().Name);

        _mockAccountRepository.Verify(r => r.GetAllAccountsWithTransactionsAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<AccountDto>>(accountEntities), Times.Once);
    }

    [Fact]
    public async Task GetAllAccountsAsync_ShouldReturnEmptyList_WhenNoAccountsExist()
    {
        // Arrange
        var emptyAccountEntities = new List<Account>();
        var emptyAccountDtos = new List<AccountDto>();

        _mockAccountRepository.Setup(r => r.GetAllAccountsWithTransactionsAsync()).ReturnsAsync(emptyAccountEntities);
        _mockMapper.Setup(m => m.Map<IEnumerable<AccountDto>>(emptyAccountEntities)).Returns(emptyAccountDtos);

        // Act
        var result = await _accountService.GetAllAccountsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockAccountRepository.Verify(r => r.GetAllAccountsWithTransactionsAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<AccountDto>>(emptyAccountEntities), Times.Once);
    }

    [Fact]
    public async Task GetAccountByIdAsync_ShouldReturnAccountDto_WhenAccountExists()
    {
        // Arrange
        var accountId = 1;
        var accountEntity = new Account
        {
            Id = accountId,
            Name = "Checking Account",
            InitialBalance = 750m,
            CurrentBalance = 750m
        };

        var accountDto = new AccountDto
        {
            Id = accountId,
            Name = "Checking Account",
            InitialBalance = 750m,
            CurrentBalance = 750m
        };

        _mockAccountRepository.Setup(r => r.GetAccountWithTransactionsAsync(accountId)).ReturnsAsync(accountEntity);
        _mockMapper.Setup(m => m.Map<AccountDto>(accountEntity)).Returns(accountDto);

        // Act
        var result = await _accountService.GetAccountByIdAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountDto.Id, result.Id);
        Assert.Equal(accountDto.Name, result.Name);
        Assert.Equal(accountDto.InitialBalance, result.InitialBalance);
        Assert.Equal(accountDto.CurrentBalance, result.CurrentBalance);

        _mockAccountRepository.Verify(r => r.GetAccountWithTransactionsAsync(accountId), Times.Once);
        _mockMapper.Verify(m => m.Map<AccountDto>(accountEntity), Times.Once);
    }

    [Fact]
    public async Task GetAccountByIdAsync_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = 999;
        Account? nullAccount = null;

        _mockAccountRepository.Setup(r => r.GetAccountWithTransactionsAsync(accountId)).ReturnsAsync(nullAccount);
        _mockMapper.Setup(m => m.Map<AccountDto>(nullAccount)).Returns((AccountDto)null!);

        // Act
        var result = await _accountService.GetAccountByIdAsync(accountId);

        // Assert
        Assert.Null(result);

        _mockAccountRepository.Verify(r => r.GetAccountWithTransactionsAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task UpdateAccountAsync_ShouldReturnUpdatedAccountDto_WhenAccountIsUpdatedSuccessfully()
    {
        // Arrange
        var accountUpdateDto = new AccountUpdateDto
        {
            Id = 1,
            Name = "Updated Account",
            InitialBalance = 2000m
        };

        var accountEntity = new Account
        {
            Id = 1,
            Name = "Updated Account",
            InitialBalance = 2000m,
            CurrentBalance = 2000m
        };

        var updatedAccountDto = new AccountDto
        {
            Id = 1,
            Name = "Updated Account",
            InitialBalance = 2000m,
            CurrentBalance = 2000m
        };

        _mockMapper.Setup(m => m.Map<Account>(accountUpdateDto)).Returns(accountEntity);
        _mockAccountRepository.Setup(r => r.UpdateAccountAsync(accountEntity)).ReturnsAsync(accountEntity);
        _mockMapper.Setup(m => m.Map<AccountDto>(accountEntity)).Returns(updatedAccountDto);

        // Act
        var result = await _accountService.UpdateAccountAsync(accountUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedAccountDto.Id, result.Id);
        Assert.Equal(updatedAccountDto.Name, result.Name);
        Assert.Equal(updatedAccountDto.InitialBalance, result.InitialBalance);
        Assert.Equal(updatedAccountDto.CurrentBalance, result.CurrentBalance);

        _mockMapper.Verify(m => m.Map<Account>(accountUpdateDto), Times.Once);
        _mockAccountRepository.Verify(r => r.UpdateAccountAsync(accountEntity), Times.Once);
        _mockMapper.Verify(m => m.Map<AccountDto>(accountEntity), Times.Once);
    }
}