using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LearnAtHomeApi._Core.Definitions;

namespace LearnAtHomeApi.User.Dto;

public class UserDto
{
    public int? Id { get; set; }

    [Required, StringLength(30)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }

    [Required, StringLength(255), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(255)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
