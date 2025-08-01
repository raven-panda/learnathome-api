using LearnAtHomeApi.Dto;
using LearnAtHomeApi.Models;
using LearnAtHomeApi.Repository;

namespace LearnAtHomeApi.Services;

public interface IRpUserService : IService<UserDto, RpUserModel> { }

public class RpUserService(IUserRepository repo) : IRpUserService
{
    public UserDto Get(int id)
    {
        return ToDto(repo.Get(id));
    }

    public UserDto Add(UserDto item)
    {
        return ToDto(repo.Add(ToModel(item)));
    }

    public int Remove(int id)
    {
        return repo.Remove(id);
    }

    public UserDto Update(UserDto item)
    {
        return ToDto(repo.Update(ToModel(item)));
    }

    public UserDto ToDto(RpUserModel model)
    {
        return new UserDto
        {
            Id = model.Id,
            Role = model.Role,
            Username = model.Username,
            Email = model.Email,
            Password = model.Password,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
        };
    }

    public RpUserModel ToModel(UserDto dto)
    {
        if (dto.Id != null && repo.Exists(dto.Id.Value))
            return repo.Get(dto.Id.Value);

        return new RpUserModel
        {
            Role = dto.Role,
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
        };
    }
}
