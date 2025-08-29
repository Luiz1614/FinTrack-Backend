using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Domain.Entities;

public class Account
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal InitialBalance { get; set; }

    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}
