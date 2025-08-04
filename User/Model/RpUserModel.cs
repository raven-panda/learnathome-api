using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using LearnAtHomeApi.User.Dto;

namespace LearnAtHomeApi.User.Model;

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

    public RpUserModel? Mentor { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
