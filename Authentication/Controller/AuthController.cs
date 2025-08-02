using LearnAtHomeApi.Authentication.Dto;
using LearnAtHomeApi.Authentication.Service;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace LearnAtHomeApi.Authentication.Controller;

[Route("auth")]
[ApiController]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult RegisterUser(AuthRegisterDto dto)
    {
        var token = service.Register(dto, out var tokenExpiration);
        CreateCookie(token, tokenExpiration);

        return Ok();
    }

    [HttpPost("login")]
    public IActionResult LoginUser(AuthLoginDto dto)
    {
        var token = service.Login(dto, out var tokenExpiration);
        CreateCookie(token, tokenExpiration);

        return Ok();
    }

    private void CreateCookie(string token, DateTime tokenExpiration)
    {
        Response.Cookies.Append(
            "access_token",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = tokenExpiration,
                Path = "/",
            }
        );
    }
}
