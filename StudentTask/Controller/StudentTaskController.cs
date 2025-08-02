using LearnAtHomeApi.StudentTask.Dto;
using LearnAtHomeApi.StudentTask.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnAtHomeApi.StudentTask.Controller;

[Route("student-tasks")]
[ApiController]
[Authorize]
public class StudentTaskController(IStudentTaskService service) : ControllerBase
{
    [HttpGet("user/{userId:int}")]
    public IActionResult GetAllByUserId(int userId)
    {
        var tasks = service.GetAllByUserId(userId);
        return Ok(tasks);
    }

    [HttpGet("{taskId:int}")]
    public IActionResult Get(int taskId)
    {
        var tasks = service.Get(taskId);
        return Ok(tasks);
    }

    [HttpPost]
    public IActionResult Create(StudentTaskDto task)
    {
        var createdTask = service.Add(task);
        return Ok(createdTask);
    }

    [HttpPatch("{taskId:int}")]
    public IActionResult Update(int taskId, StudentTaskDto task)
    {
        task.Id = taskId;
        var updated = service.Update(task);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return Ok(service.Remove(id).ToString());
    }
}
