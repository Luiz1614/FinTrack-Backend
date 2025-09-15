using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Title { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    public List<Transaction> Transactions { get; set; } = new();
}