using System.Text;
using LearnAtHomeApi._Core.Exceptions;
using LearnAtHomeApi.Authentication.Dto;
using LearnAtHomeApi.Authentication.Security;
using LearnAtHomeApi.User.Dto;
using LearnAtHomeApi.User.Service;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LearnAtHomeApi.Authentication.Service;

public interface IAuthService
{
    (string refreshToken, string accessToken) Register(
        AuthRegisterDto dto,
        out DateTime tokenExpiration
    );
    (string refreshToken, string accessToken) Login(AuthLoginDto dto, out DateTime tokenExpiration);
    string RefreshAccessToken(string? refreshToken);
}

internal sealed class AuthService(
    IRpUserService service,
    TokenProvider tokenProvider,
    IPasswordHasher passwordHasher,
    IConfiguration configuration
) : IAuthService
{
    public (string refreshToken, string accessToken) Register(
        AuthRegisterDto dto,
        out DateTime tokenExpiration
    )
    {
        if (dto.Password != dto.PasswordConfirm)
            throw new PasswordsNotMatchingException();

        var user = service.Add(
            new UserDto
            {
                Email = dto.Email,
                Password = passwordHasher.Hash(dto.Password),
                Username = dto.Username,
                Role = UserRole.Mentor,
            }
        );

        var refreshToken = tokenProvider.GenerateRefreshToken(user, out tokenExpiration);
        var accessToken = tokenProvider.GenerateAccessToken(user, out tokenExpiration);
        return (refreshToken, accessToken);
    }

    public (string refreshToken, string accessToken) Login(
        AuthLoginDto dto,
        out DateTime refreshTokenExpiration
    )
    {
        var user = service.TryGetByEmail(dto.Email);
        if (user is null || !passwordHasher.VerifyHashedPassword(user.Password, dto.Password))
            throw new InvalidCredentialsException();

        var refreshToken = tokenProvider.GenerateRefreshToken(user, out refreshTokenExpiration);
        var accessToken = tokenProvider.GenerateAccessToken(user, out _);
        return (refreshToken, accessToken);
    }

    public string RefreshAccessToken(string? refreshToken)
    {
        if (refreshToken == null)
            throw new SecurityTokenException();

        var parsedUserId = tokenProvider.ParseUserId(refreshToken);
        var user = service.Get(parsedUserId);
        return tokenProvider.GenerateAccessToken(user, out _);
    }
}
