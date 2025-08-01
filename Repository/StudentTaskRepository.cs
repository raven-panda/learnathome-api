using LearnAtHomeApi.Models;

namespace LearnAtHomeApi.Repository;

public interface IStudentTaskRepository
{
    IEnumerable<StudentTaskModel> GetAllByUserId(int id);
    StudentTaskModel Get(int id);
    bool Exists(int id);
    StudentTaskModel Add(StudentTaskModel item);
    int Remove(int id);
    StudentTaskModel Update(StudentTaskModel item);
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
            throw new InvalidOperationException($"Entity with id {id} not found");
            
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
            throw new InvalidOperationException($"Entity with id {id} not found");

        context.StudentTasks.Remove(entity);
        context.SaveChanges();
        return entity.Id;
    }

    public StudentTaskModel Update(StudentTaskModel item)
    {
        var existing = context.StudentTasks.Find(item.Id);
        if (existing == null)
            throw new InvalidOperationException();
        
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