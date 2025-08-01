using LearnAtHomeApi._Core.Exceptions.Entity;
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
        var item = repo.Get(id);
        if (item == null)
            throw new EntityNotFoundException("Task", id);

        return ToDto(item);
    }

    public StudentTaskDto Add(StudentTaskDto item)
    {
        return ToDto(repo.Add(ToModel(item)));
    }

    public int Remove(int id)
    {
        if (!repo.Exists(id))
            throw new EntityNotFoundException("Task", id);

        return repo.Remove(id);
    }

    public StudentTaskDto Update(StudentTaskDto item)
    {
        if (!repo.Exists(item.Id))
            throw new EntityNotFoundException("Task", item.Id);

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
        if (dto.Id == null)
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

        var item = repo.Get(dto.Id);
        if (item == null)
            throw new EntityNotFoundException("Task", dto.Id);

        return item;
    }
}
