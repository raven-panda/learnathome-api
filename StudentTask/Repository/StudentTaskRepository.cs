using LearnAtHomeApi._Core.Repository;
using LearnAtHomeApi.StudentTask.Model;

namespace LearnAtHomeApi.StudentTask.Repository;

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

    public StudentTaskModel? Get(int? id)
    {
        return context.StudentTasks.Find(id);
    }

    public bool Exists(int? id)
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
        var entity = context.StudentTasks.Find(id)!;

        context.StudentTasks.Remove(entity);
        context.SaveChanges();
        return entity.Id;
    }

    public StudentTaskModel Update(StudentTaskModel item)
    {
        var existing = context.StudentTasks.Find(item.Id)!;

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
