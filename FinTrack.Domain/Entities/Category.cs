using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}
