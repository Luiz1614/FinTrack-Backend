using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack.Domain.Entities;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    public int UserId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal InitialBalance { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CurrentBalance { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
}