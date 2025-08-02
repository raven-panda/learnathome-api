using LearnAtHomeApi.Authentication.Dto;
using LearnAtHomeApi.Authentication.Service;
using Microsoft.AspNetCore.Mvc;

namespace LearnAtHomeApi.Authentication.Controller;

[Route("auth")]
[ApiController]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult RegisterUser(AuthRegisterDto dto)
    {
        service.Register(dto);
        return Ok();
    }
}
