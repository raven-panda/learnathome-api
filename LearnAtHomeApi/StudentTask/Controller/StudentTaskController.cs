using LearnAtHomeApi.Authentication.Security;
using LearnAtHomeApi.StudentTask.Dto;
using LearnAtHomeApi.StudentTask.Service;
using LearnAtHomeApi.User.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace LearnAtHomeApi.StudentTask.Controller;

[Route("student-tasks")]
[ApiController]
[Authorize]
public class StudentTaskController(
    IStudentTaskService service,
    TokenProvider tokenProvider,
    IConfiguration configuration
) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllById()
    {
        var (userId, _, role) = tokenProvider.ParseUserToken(
            Request.Cookies[configuration["Jwt:RefreshTokenCookieName"]!]!
        );
        var tasks =
            role == UserRole.Mentor
                ? service.GetAllByMentorId(userId)
                : service.GetAllByAttributedStudentId(userId);
        return Ok(tasks);
    }

    [HttpGet("{taskId:int}")]
    public IActionResult Get(int taskId)
    {
        var tasks = service.Get(taskId);
        return Ok(tasks);
    }

    [HttpPost]
    public IActionResult Create(CreateStudentTaskDto task)
    {
        var userId = tokenProvider.ParseUserId(
            Request.Cookies[configuration["Jwt:RefreshTokenCookieName"]!]!
        );
        var createdTask = service.Add(task, userId);
        return Ok(createdTask);
    }

    [HttpPatch("{taskId:int}")]
    public IActionResult Update(int taskId, UpdateStudentTaskDto task)
    {
        task.Id = taskId;
        var userId = tokenProvider.ParseUserId(
            Request.Cookies[configuration["Jwt:RefreshTokenCookieName"]!]!
        );
        var updated = service.Update(task, userId);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return Ok(service.Remove(id).ToString());
    }
}
