using FinTrack.Domain.Enums;

namespace Fintrack.Contracts.DTOs.Transaction;

public class TransactionDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryTitle { get; set; }
    public string? AccountId { get; set; }
    public string? AccountName { get; set; }
}
