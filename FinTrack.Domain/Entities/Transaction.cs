using FinTrack.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Domain.Entities;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public TransactionType Type { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int AccountId { get; set; }
    public Account? Account { get; set; }
}
