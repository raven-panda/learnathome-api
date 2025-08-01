using LearnAtHomeApi.Dto;
using LearnAtHomeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnAtHomeApi.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(IRpUserService service) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult RegisterUser(UserDto userDto)
    {
        var createdUser = service.Add(userDto);
        return Ok(createdUser);
    }
}
