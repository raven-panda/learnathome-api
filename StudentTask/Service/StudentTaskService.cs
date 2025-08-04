using LearnAtHomeApi._Core.Exceptions;
using LearnAtHomeApi._Core.Service;
using LearnAtHomeApi.StudentTask.Dto;
using LearnAtHomeApi.StudentTask.Model;
using LearnAtHomeApi.StudentTask.Repository;
using LearnAtHomeApi.User.Dto;

namespace LearnAtHomeApi.StudentTask.Service;

public interface IStudentTaskService
    : IAuditableService<
        StudentTaskDto,
        StudentTaskModel,
        CreateStudentTaskDto,
        UpdateStudentTaskDto
    >
{
    IEnumerable<StudentTaskDto> GetAllByMentorId(int id);
    IEnumerable<StudentTaskDto> GetAllByAttributedStudentId(int id);
}

internal sealed class StudentTaskService(IStudentTaskRepository repo) : IStudentTaskService
{
    public IEnumerable<StudentTaskDto> GetAllByMentorId(int id)
    {
        return repo.GetAllByMentorId(id).Select(ToDto).ToList();
    }

    public IEnumerable<StudentTaskDto> GetAllByAttributedStudentId(int id)
    {
        return repo.GetAllByAttributedStudentId(id).Select(ToDto).ToList();
    }

    public StudentTaskDto Get(int id)
    {
        var item = repo.Get(id);
        if (item == null)
            throw new EntityNotFoundException("Task", id);

        return ToDto(item);
    }

    public StudentTaskDto Add(CreateStudentTaskDto item, int createdById)
    {
        return ToDto(repo.Add(ToModelForCreation(item), createdById));
    }

    public int Remove(int id)
    {
        if (!repo.Exists(id))
            throw new EntityNotFoundException("Task", id);

        return repo.Remove(id);
    }

    public StudentTaskDto Update(UpdateStudentTaskDto item, int updatedById)
    {
        if (!repo.Exists(item.Id))
            throw new EntityNotFoundException("Task", item.Id);

        return ToDto(repo.Update(ToModelForUpdate(item), updatedById));
    }

    public StudentTaskDto ToDto(StudentTaskModel model)
    {
        return new StudentTaskDto
        {
            Id = model.Id,
            AttributedUserId = model.AttributedUserId,
            Name = model.Name,
            Description = model.Description,
            MentorId = model.Mentor.Id,
            CreatedAt = model.CreatedAt,
            CreatedBy = new UserSummaryDto
            {
                Id = model.CreatedBy.Id,
                Role = model.CreatedBy.Role,
                Username = model.CreatedBy.Username,
            },
            UpdatedAt = model.UpdatedAt,
            UpdatedBy = new UserSummaryDto
            {
                Id = model.UpdatedBy.Id,
                Role = model.UpdatedBy.Role,
                Username = model.UpdatedBy.Username,
            },
            EndDate = model.EndDate,
        };
    }

    public StudentTaskModel ToModel(StudentTaskDto dto)
    {
        throw new NotImplementedException();
    }

    public StudentTaskModel ToModelForCreation(CreateStudentTaskDto dto)
    {
        return new StudentTaskModel
        {
            AttributedUserId = dto.AttributedUserId,
            Name = dto.Name,
            Description = dto.Description,
            EndDate = dto.EndDate,
        };
    }

    public StudentTaskModel ToModelForUpdate(UpdateStudentTaskDto dto)
    {
        var item = repo.Get(dto.Id);
        if (item == null)
            throw new EntityNotFoundException("Task", dto.Id);

        return item;
    }
}
