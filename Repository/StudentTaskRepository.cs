using LearnAtHomeApi._Core.Exceptions.Entity;
using LearnAtHomeApi.Models;

namespace LearnAtHomeApi.Repository;

public interface IStudentTaskRepository : IRepositoryBase<StudentTaskModel>
{
    IEnumerable<StudentTaskModel> GetAllByUserId(int id);
}

public class StudentTaskRepositoryImp(AppDbContext context) : IStudentTaskRepository
{
    public IEnumerable<StudentTaskModel> GetAllByUserId(int id)
    {
        return context.StudentTasks.Where(p => p.AttributedUserId == id);
    }

    public StudentTaskModel Get(int id)
    {
        var item = context.StudentTasks.Find(id);
        if (item == null)
            throw new EntityNotFoundException("Task", id);

        return item;
    }

    public bool Exists(int id)
    {
        return context.StudentTasks.Any(item => item.Id == id);
    }

    public StudentTaskModel Add(StudentTaskModel item)
    {
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        // TODO : add auth to implement this
        item.CreatedByUserId = 0;
        item.UpdatedByUserId = 0;

        context.StudentTasks.Add(item);
        context.SaveChanges();
        return item;
    }

    public int Remove(int id)
    {
        var entity = context.StudentTasks.Find(id);
        if (entity == null)
            throw new EntityNotFoundException("Task", id);

        context.StudentTasks.Remove(entity);
        context.SaveChanges();
        return entity.Id;
    }

    public StudentTaskModel Update(StudentTaskModel item)
    {
        var existing = context.StudentTasks.Find(item.Id);
        if (existing == null)
            throw new EntityNotFoundException("Task", item.Id);

        item.CreatedAt = existing.CreatedAt;
        item.UpdatedAt = DateTime.Now;
        // TODO : add auth to implement this
        item.CreatedByUserId = existing.CreatedByUserId;
        item.UpdatedByUserId = 0;

        context.StudentTasks.Update(item);
        context.SaveChanges();
        return item;
    }
}
