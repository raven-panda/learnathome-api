using LearnAtHomeApi._Core.Exceptions.Entity;
using LearnAtHomeApi.Models;

namespace LearnAtHomeApi.Repository;

public interface IUserRepository : IRepositoryBase<RpUserModel>
{
}

public class UserRepository(AppDbContext context) : IUserRepository
{
    public RpUserModel Get(int id)
    {
        var item = context.Users.Find(id);
        if (item == null)
            throw new EntityNotFoundException("User", id);
            
        return item;
    }

    public bool Exists(int id)
    {
        return context.Users.Any(item => item.Id == id);
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
        var entity = context.Users.Find(id);
        if (entity == null)
            throw new EntityNotFoundException("User", id);

        context.Users.Remove(entity);
        context.SaveChanges();
        return entity.Id;
    }

    public RpUserModel Update(RpUserModel item)
    {
        var existing = context.Users.Find(item.Id);
        if (existing == null)
            throw new EntityNotFoundException("User", item.Id);
        
        item.CreatedAt = existing.CreatedAt;
        item.UpdatedAt = DateTime.Now;

        context.Users.Update(item);
        context.SaveChanges();
        return item;
    }
}