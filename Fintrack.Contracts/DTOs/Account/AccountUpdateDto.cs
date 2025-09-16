namespace Fintrack.Contracts.DTOs.Account;

public class AccountUpdateDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal InitialBalance { get; set; }
}
