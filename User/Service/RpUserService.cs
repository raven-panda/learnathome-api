using LearnAtHomeApi._Core.Exceptions;
using LearnAtHomeApi._Core.Service;
using LearnAtHomeApi.User.Dto;
using LearnAtHomeApi.User.Model;
using LearnAtHomeApi.User.Repository;

namespace LearnAtHomeApi.User.Service;

public interface IRpUserService : IService<UserDto, RpUserModel>
{
    public UserDto? TryGetByEmail(string email);
}

internal sealed class RpUserService(IUserRepository repo) : IRpUserService
{
    public UserDto Get(int id)
    {
        var item = repo.Get(id);
        if (item == null)
            throw new EntityNotFoundException("User", id);

        return ToDto(item);
    }

    public UserDto? TryGetByEmail(string email)
    {
        var item = repo.GetByEmail(email);

        return item != null ? ToDto(item) : null;
    }

    public UserDto Add(UserDto item)
    {
        if (item.Role == UserRole.Mentor && item.MentorId != null)
            throw new BadHttpRequestException(
                "User with role Mentor cannot have a mentor himself."
            );
        if (item.Role == UserRole.Student && item.MentorId == null)
            throw new BadHttpRequestException(
                "User with role Mentor cannot have a mentor himself."
            );
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
        throw new AccessViolationException("Cannot update user");
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
            MentorId = model.Mentor?.Id,
        };
    }

    public RpUserModel ToModel(UserDto dto)
    {
        return new RpUserModel
        {
            Role = dto.Role,
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Mentor = dto.MentorId != null ? repo.Get(dto.MentorId) : null,
        };
    }
}
