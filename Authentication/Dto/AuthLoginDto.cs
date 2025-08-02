using System.ComponentModel.DataAnnotations;

namespace LearnAtHomeApi.Authentication.Dto;

public class AuthLoginDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
