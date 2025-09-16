using FinTrack.Domain.Enums;

namespace Fintrack.Contracts.DTOs.Transaction;

public class TransactionCreateDto
{
    public string? Title { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public int AccountId { get; set; }
}
