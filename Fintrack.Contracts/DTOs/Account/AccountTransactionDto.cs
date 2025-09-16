namespace FinTrack.Application.DTOs.Accounts;

public class AccountTransactionDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Type { get; init; } = string.Empty;
    public int CategoryId { get; init; }
    public string CategoryTitle { get; init; } = string.Empty;
}