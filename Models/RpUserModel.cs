using System.ComponentModel.DataAnnotations;
using LearnAtHomeApi._Core.Definitions;

namespace LearnAtHomeApi.Models;

public class RpUserModel
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(30)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    [Required, StringLength(255), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
