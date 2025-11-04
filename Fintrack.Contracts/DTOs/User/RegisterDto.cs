using System.ComponentModel.DataAnnotations;

namespace Fintrack.Contracts.DTOs.User;

public class RegisterDto
{
    [Required(ErrorMessage = "User Name is required")]
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }
    [Required(ErrorMessage = "Phone number is required")]
    public string? PhoneNumber { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
