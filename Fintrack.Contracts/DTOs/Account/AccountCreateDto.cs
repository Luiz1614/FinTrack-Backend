using System.Text.Json.Serialization;

namespace Fintrack.Contracts.DTOs.Account;

public class AccountCreateDto
{
    [JsonIgnore]
    public int UserId { get; set; }
    [JsonIgnore]
    public string? Name { get; set; }
    public decimal InitalBalance { get; set; }
}
