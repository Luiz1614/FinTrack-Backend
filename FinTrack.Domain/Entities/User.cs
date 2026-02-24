using Microsoft.AspNetCore.Identity;

namespace FinTrack.Domain.Entities;

public class User : IdentityUser<int>
{
    public DateTime DateOfBirth { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
}
