using LearnAtHomeApi.Dto;
using LearnAtHomeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnAtHomeApi.Controllers;

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
