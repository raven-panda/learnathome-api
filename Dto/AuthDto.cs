using System.ComponentModel.DataAnnotations;

namespace LearnAtHomeApi.Dto;

public class AuthRegisterDto
{
    [Required, StringLength(30)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(255), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(255)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$")]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(255)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$")]
    public string PasswordConfirm { get; set; } = string.Empty;
}
