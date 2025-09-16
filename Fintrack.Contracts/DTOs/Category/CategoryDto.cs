namespace Fintrack.Contracts.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int TransactionsCount { get; set; }
}
