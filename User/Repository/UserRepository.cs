using LearnAtHomeApi._Core.Repository;
using LearnAtHomeApi.User.Model;

namespace LearnAtHomeApi.User.Repository;

public interface IUserRepository : IRepositoryBase<RpUserModel>
{
    bool ExistsByEmail(string email);
}

public class UserRepository(AppDbContext context) : IUserRepository
{
    public RpUserModel? Get(int? id)
    {
        return context.Users.Find(id);
    }

    public bool Exists(int? id)
    {
        return context.Users.Any(item => item.Id == id);
    }

    public bool ExistsByEmail(string email)
    {
        return context.Users.Any(item => item.Email == email);
    }

    public RpUserModel Add(RpUserModel item)
    {
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;

        context.Users.Add(item);
        context.SaveChanges();
        return item;
    }

    public int Remove(int id)
    {
        var entity = context.Users.Find(id)!;
        context.Users.Remove(entity);
        context.SaveChanges();
        return entity.Id;
    }

    public RpUserModel Update(RpUserModel item)
    {
        var existing = context.Users.Find(item.Id)!;

        item.CreatedAt = existing.CreatedAt;
        item.UpdatedAt = DateTime.Now;

        context.Users.Update(item);
        context.SaveChanges();
        return item;
    }
}
