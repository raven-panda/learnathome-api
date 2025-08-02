using LearnAtHomeApi._Core.Definitions;
using LearnAtHomeApi._Core.Exceptions.Entity;
using LearnAtHomeApi.Authentication.Dto;
using LearnAtHomeApi.User.Dto;
using LearnAtHomeApi.User.Service;

namespace LearnAtHomeApi.Authentication.Service;

public interface IAuthService
{
    string Register(AuthRegisterDto dto, out DateTime tokenExpiration);
    string Login(AuthLoginDto dto, out DateTime tokenExpiration);
}

internal sealed class AuthService(IRpUserService service, TokenProvider tokenProvider)
    : IAuthService
{
    public string Register(AuthRegisterDto dto, out DateTime tokenExpiration)
    {
        if (dto.Password != dto.PasswordConfirm)
            throw new PasswordsNotMatchingException();

        var user = service.Add(
            new UserDto()
            {
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                Role = UserRole.Mentor,
            }
        );

        var token = tokenProvider.Generate(user, out tokenExpiration);
        return token;
    }

    public string Login(AuthLoginDto dto, out DateTime tokenExpiration)
    {
        var user = service.GetByEmail(dto.Email);

        var token = tokenProvider.Generate(user, out tokenExpiration);
        return token;
    }
}
