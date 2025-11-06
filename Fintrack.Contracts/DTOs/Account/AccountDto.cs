namespace FinTrack.Application.DTOs.Accounts;

public class AccountDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal InitialBalance { get; init; }
    public decimal? CurrentBalance { get; init; }
    public IEnumerable<AccountTransactionDto>? Transactions { get; init; }
}