using System.ComponentModel.DataAnnotations;

namespace Blazor.HelloGalaxy.Shared;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;
 
    [Required]
    [DataType(DataType.Password)]
    [StringLength(50, ErrorMessage = "Password must be length between {2} and {1}", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}