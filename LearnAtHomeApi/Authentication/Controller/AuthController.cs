using LearnAtHomeApi.Authentication.Dto;
using LearnAtHomeApi.Authentication.Service;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace LearnAtHomeApi.Authentication.Controller;

[Route("auth")]
[ApiController]
public class AuthController(IAuthService service, IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult RegisterUser(AuthRegisterDto dto)
    {
        var (refreshToken, accessToken) = service.Register(dto, out var tokenExpiration);
        CreateRefreshTokenCookie(refreshToken, tokenExpiration);
        CreateSessionTokenCookie(accessToken);

        return Ok();
    }

    [HttpPost("login")]
    public IActionResult LoginUser(AuthLoginDto dto)
    {
        var (refreshToken, accessToken) = service.Login(dto, out var refreshTokenExpiration);
        CreateRefreshTokenCookie(refreshToken, refreshTokenExpiration);
        CreateSessionTokenCookie(accessToken);

        return Ok();
    }

    [HttpGet("refresh")]
    public IActionResult RefreshUserToken()
    {
        var refreshToken = Request.Cookies[configuration["Jwt:RefreshTokenCookieName"]!];
        var accessToken = service.RefreshAccessToken(refreshToken);
        CreateSessionTokenCookie(accessToken);
        return Ok();
    }

    [HttpGet("logout")]
    public IActionResult LogoutUser()
    {
        Response.Cookies.Delete(configuration["Jwt:RefreshTokenCookieName"]!);
        Response.Cookies.Delete(configuration["Jwt:SessionTokenCookieName"]!);
        return Ok();
    }

    private void CreateRefreshTokenCookie(string token, DateTime tokenExpiration)
    {
        Response.Cookies.Append(
            configuration["Jwt:RefreshTokenCookieName"]!,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = tokenExpiration,
                Secure = true,
                Path = "/",
            }
        );
    }

    private void CreateSessionTokenCookie(string token)
    {
        Response.Cookies.Append(
            configuration["Jwt:SessionTokenCookieName"]!,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/",
            }
        );
    }
}
