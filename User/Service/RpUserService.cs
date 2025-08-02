using LearnAtHomeApi._Core.Exceptions.Entity;
using LearnAtHomeApi._Core.Service;
using LearnAtHomeApi.User.Dto;
using LearnAtHomeApi.User.Model;
using LearnAtHomeApi.User.Repository;

namespace LearnAtHomeApi.User.Service;

public interface IRpUserService : IService<UserDto, RpUserModel> { }

public class RpUserService(IUserRepository repo) : IRpUserService
{
    public UserDto Get(int id)
    {
        var item = repo.Get(id);
        if (item == null)
            throw new EntityNotFoundException("User", id);

        return ToDto(item);
    }

    public UserDto Add(UserDto item)
    {
        if (repo.ExistsByEmail(item.Email))
            throw new EntityUniqueConstraintViolationException("User", "Email");

        return ToDto(repo.Add(ToModel(item)));
    }

    public int Remove(int id)
    {
        if (!repo.Exists(id))
            throw new EntityNotFoundException("User", id);

        return repo.Remove(id);
    }

    public UserDto Update(UserDto item)
    {
        if (!repo.Exists(item.Id))
            throw new EntityNotFoundException("User", item.Id);
        if (repo.ExistsByEmail(item.Email))
            throw new EntityUniqueConstraintViolationException("User", "Email");

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
        if (dto.Id == null)
            return new RpUserModel
            {
                Role = dto.Role,
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
            };

        var item = repo.Get(dto.Id);
        if (item == null)
            throw new EntityNotFoundException("User", dto.Id);

        return item;
    }
}
