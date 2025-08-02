using LearnAtHomeApi._Core.Definitions;
using LearnAtHomeApi._Core.Exceptions.Entity;
using LearnAtHomeApi.Authentication.Dto;
using LearnAtHomeApi.User.Dto;
using LearnAtHomeApi.User.Service;

namespace LearnAtHomeApi.Authentication.Service;

public interface IAuthService
{
    void Register(AuthRegisterDto dto);
}

public class AuthService(IRpUserService service) : IAuthService
{
    public void Register(AuthRegisterDto dto)
    {
        if (dto.Password != dto.PasswordConfirm)
            throw new PasswordsNotMatchingException();

        service.Add(
            new UserDto()
            {
                Email = dto.Email,
                Password = dto.Password,
                Username = dto.Username,
                Role = UserRole.Mentor,
            }
        );
    }
}
