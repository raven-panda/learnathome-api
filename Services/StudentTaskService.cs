using LearnAtHomeApi.Dto;
using LearnAtHomeApi.Models;
using LearnAtHomeApi.Repository;

namespace LearnAtHomeApi.Services;

public interface IStudentTaskService : IService<StudentTaskDto, StudentTaskModel>
{
    IEnumerable<StudentTaskDto> GetAllByUserId(int id);
}

public class StudentTaskService(IStudentTaskRepository repo) : IStudentTaskService
{
    public IEnumerable<StudentTaskDto> GetAllByUserId(int id)
    {
        return repo.GetAllByUserId(id).Select(ToDto).ToList();
    }

    public StudentTaskDto Get(int id)
    {
        return ToDto(repo.Get(id));
    }

    public StudentTaskDto Add(StudentTaskDto item)
    {
        return ToDto(repo.Add(ToModel(item)));
    }

    public int Remove(int id)
    {
        return repo.Remove(id);
    }

    public StudentTaskDto Update(StudentTaskDto item)
    {
        return ToDto(repo.Update(ToModel(item)));
    }

    public StudentTaskDto ToDto(StudentTaskModel model)
    {
        return new StudentTaskDto
        {
            Id = model.Id,
            AttributedUserId = model.AttributedUserId,
            Name = model.Name,
            Description = model.Description,
            CreatedAt = model.CreatedAt,
            CreatedByUserId = model.CreatedByUserId,
            UpdatedAt = model.UpdatedAt,
            UpdatedByUserId = model.UpdatedByUserId,
            EndDate = model.EndDate,
        };
    }

    public StudentTaskModel ToModel(StudentTaskDto dto)
    {
        if (dto.Id != null && repo.Exists(dto.Id.Value))
            return repo.Get(dto.Id.Value);

        return new StudentTaskModel
        {
            AttributedUserId = dto.AttributedUserId,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt,
            CreatedByUserId = dto.CreatedByUserId,
            UpdatedAt = dto.UpdatedAt,
            UpdatedByUserId = dto.UpdatedByUserId,
            EndDate = dto.EndDate,
        };
    }
}
